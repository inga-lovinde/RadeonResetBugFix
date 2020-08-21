namespace RadeonResetBugFixService
{
    using System;
    using System.Reflection;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using Contracts;
    using Tasks.ComplexTasks;

    public partial class RadeonResetBugFixService : ServiceBase
    {
        private ServiceContext Context { get; } = new ServiceContext();

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

        protected override void OnShutdown()
        {
            this.Handler.HandleEntryPoint(
                "ServiceBase.OnShutdown",
                (logger) => this.CallStop()
            );
        }

        protected override void OnStart(string[] args)
        {
            this.Handler.HandleEntryPoint(
                "ServiceBase.OnStart",
                (logger) =>
                {
                    this.RequestAdditionalTime((int)Constants.ServiceTimeout.TotalMilliseconds);
                    TasksProcessor.ProcessTask(logger, new StartupTask(this.Context));
                    this.EnablePreshutdown();
                    SystemEvents.SessionEnding += this.OnSessionEnding;
                });
        }

        protected override void OnStop()
        {
            this.Handler.HandleEntryPoint(
                "ServiceBase.OnStop",
                (logger) =>
                {
                    this.RequestAdditionalTime((int)Constants.ServiceTimeout.TotalMilliseconds);
                    TasksProcessor.ProcessTask(logger, new ShutdownTask(this.Context));
                    SystemEvents.SessionEnding -= this.OnSessionEnding;
                });
        }

        protected override void OnCustomCommand(int command)
        {
            const int SERVICE_CONTROL_PRESHUTDOWN = 0xf;

            this.Handler.HandleEntryPoint(
                "ServiceBase.OnCustomCommand",
                (logger) =>
                {
                    if (command == SERVICE_CONTROL_PRESHUTDOWN)
                    {
                        logger.Log("Custom command: preshutdown");
                        this.CallStop();
                    }
                    else
                    {
                        logger.Log("Unknown custom command: {command}");
                    }
                });
        }

        private void OnSessionEnding(object sender, SessionEndingEventArgs args)
        {
            this.Handler.HandleEntryPoint(
                "SystemEvents.OnSessionEnding",
                (logger) =>
                {
                    logger.Log($"Session end reason: ${args.Reason}");

                    if (args.Reason == SessionEndReasons.SystemShutdown)
                    {
                        TasksProcessor.ProcessTask(logger, new ShutdownTask(this.Context));
                    }
                });
        }
    }
}
