namespace RadeonResetBugFixService
{
    using System;
    using System.Reflection;
    using System.ServiceProcess;
    using Microsoft.Win32;

    public partial class RadeonResetBugFixService : ServiceBase
    {
        private MainHandler Handler { get; } = new MainHandler();

        public RadeonResetBugFixService()
        {
            InitializeComponent();
            this.EnablePreshutdown();
        }

        private void EnablePreshutdown()
        {
            const int SERVICE_ACCEPT_PRESHUTDOWN = 0x100;

            var acceptedCommandsFieldInfo = typeof(ServiceBase).GetField("acceptedCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            if (acceptedCommandsFieldInfo == null)
            {
                throw new Exception("acceptedCommands field not found");
            }

            var value = (int)acceptedCommandsFieldInfo.GetValue(this);
            acceptedCommandsFieldInfo.SetValue(this, value | SERVICE_ACCEPT_PRESHUTDOWN);
        }

        private void CallStop()
        {
            var deferredStopMethodInfo = typeof(ServiceBase).GetMethod("DeferredStop", BindingFlags.Instance | BindingFlags.NonPublic);
            deferredStopMethodInfo.Invoke(this, null);
        }

        private void Process(string reason, Action<string> handle)
        {
            this.Handler.HandleLog($"{reason} initiated");
            try
            {
                handle(reason);
                this.Handler.HandleLog($"{reason} successfully finished");
            }
            catch (Exception e)
            {
                this.Handler.HandleLog($"{reason} error: {e}");
            }
        }

        protected override void OnShutdown()
        {
            this.Process(
                "ServiceBase.OnShutdown",
                (string reason) =>
                {
                    this.CallStop();
                });
        }

        protected override void OnStart(string[] args)
        {
            this.Process(
                "ServiceBase.OnStart",
                (string reason) =>
                {
                    this.RequestAdditionalTime((int)Constants.ServiceTimeout.TotalMilliseconds);
                    this.Handler.HandleStartup(reason);
                    this.EnablePreshutdown();
                    SystemEvents.SessionEnding += this.OnSessionEnding;
                });
        }

        protected override void OnStop()
        {
            this.Process(
                "ServiceBase.OnStop",
                (string reason) =>
                {
                    this.RequestAdditionalTime((int)Constants.ServiceTimeout.TotalMilliseconds);
                    this.Handler.HandleShutdown(reason);
                    SystemEvents.SessionEnding -= this.OnSessionEnding;
                });
        }

        protected override void OnCustomCommand(int command)
        {
            const int SERVICE_CONTROL_PRESHUTDOWN = 0xf;

            this.Process(
                "ServiceBase.OnCustomCommand",
                (string reason) =>
                {
                    if (command == SERVICE_CONTROL_PRESHUTDOWN)
                    {
                        this.Handler.HandleLog($"Custom command: preshutdown");
                        this.CallStop();
                    }
                    else
                    {
                        this.Handler.HandleLog($"Unknown custom command: {command}");
                    }
                });
        }

        private void OnSessionEnding(object sender, SessionEndingEventArgs args)
        {
            this.Process(
                "SystemEvents.OnSessionEnding",
                (string reason) =>
                {
                    this.Handler.HandleLog($"Session end reason: ${args.Reason}");

                    if (args.Reason == SessionEndReasons.SystemShutdown)
                    {
                        this.Handler.HandleShutdown(reason);
                    }
                });
        }
    }
}
