namespace RadeonResetBugFixService
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Linq;
    using System.Management;
    using Microsoft.Win32;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            ManagementObject wmiService = null;
            ManagementBaseObject InParam = null;
            try
            {
                wmiService = new ManagementObject($"Win32_Service.Name='{this.serviceInstaller1.ServiceName}'");
                InParam = wmiService.GetMethodParameters("Change");
                InParam["DesktopInteract"] = true;
                wmiService.InvokeMethod("Change", InParam, null);
            }
            finally
            {
                if (InParam != null)
                    InParam.Dispose();
                if (wmiService != null)
                    wmiService.Dispose();
            }

            ThirdParty.ServicePreshutdownHelpers.ServicePreshutdownHelpers.SetPreShutdownTimeOut(this.serviceInstaller1.ServiceName, (uint)Constants.ServiceTimeout.TotalMilliseconds);

            var preshutdownOrder = GetPreshutdownOrder();
            if (!preshutdownOrder.Contains(this.serviceInstaller1.ServiceName))
            {
               SetPreshutdownOrder(new[] { this.serviceInstaller1.ServiceName }.Concat(preshutdownOrder).ToArray());
            }
        }

        private void serviceInstaller1_AfterUninstall(object sender, InstallEventArgs e)
        {
            var preshutdownOrder = GetPreshutdownOrder();
            if (preshutdownOrder.Contains(this.serviceInstaller1.ServiceName))
            {
                SetPreshutdownOrder(preshutdownOrder.Where((name) => name != this.serviceInstaller1.ServiceName).ToArray());
            }
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private string[] GetPreshutdownOrder()
        {
            return (string[])Registry.GetValue(Constants.RegistryKeySystemControl, "PreshutdownOrder", new string[0]);
        }

        private void SetPreshutdownOrder(string[] data)
        {
            Registry.SetValue(Constants.RegistryKeySystemControl, "PreshutdownOrder", data, RegistryValueKind.MultiString);
        }
    }
}
