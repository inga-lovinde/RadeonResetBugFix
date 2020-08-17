namespace RadeonResetBugFixService
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.IO;
    using System.Linq;
    using System.Management;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            Console.WriteLine($"Creating log directory ({Constants.LogDirectory})");
            Directory.CreateDirectory(Constants.LogDirectory);

            Console.WriteLine("Preventing Windows from killing services that take up to 300 seconds to shutdown");
            RegistryHelper.SetWaitToKillServiceTimeout((int)Constants.ServiceTimeout.TotalMilliseconds);

            Console.WriteLine("Disabling fast reboot");
            RegistryHelper.SetFastRebootStatus(false);

            Console.WriteLine("Allowing interactive services");
            RegistryHelper.SetInteractiveServicesStatus(true);

            Console.WriteLine("Configuring service as interactive");
            using (var wmiService = new ManagementObject($"Win32_Service.Name='{this.serviceInstaller1.ServiceName}'"))
            {
                using (var InParam = wmiService.GetMethodParameters("Change"))
                {
                    InParam["DesktopInteract"] = true;
                    wmiService.InvokeMethod("Change", InParam, null);
                }
            }

            Console.WriteLine("Setting preshutdown timeout for service");
            ThirdParty.ServicePreshutdownHelpers.ServicePreshutdownHelpers.SetPreShutdownTimeOut(this.serviceInstaller1.ServiceName, (uint)Constants.ServiceTimeout.TotalMilliseconds);

            Console.WriteLine("Adding service to preshutdown order");
            var preshutdownOrder = RegistryHelper.GetPreshutdownOrder();
            if (!preshutdownOrder.Contains(this.serviceInstaller1.ServiceName))
            {
                RegistryHelper.SetPreshutdownOrder(new[] { this.serviceInstaller1.ServiceName }.Concat(preshutdownOrder).ToArray());
            }
        }

        private void serviceInstaller1_AfterUninstall(object sender, InstallEventArgs e)
        {
            Console.WriteLine("Removing service from preshutdown order");
            var preshutdownOrder = RegistryHelper.GetPreshutdownOrder();
            if (preshutdownOrder.Contains(this.serviceInstaller1.ServiceName))
            {
                RegistryHelper.SetPreshutdownOrder(preshutdownOrder.Where((name) => name != this.serviceInstaller1.ServiceName).ToArray());
            }
        }
    }
}
