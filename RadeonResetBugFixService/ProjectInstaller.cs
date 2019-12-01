namespace RadeonResetBugFixService
{
    using System.ComponentModel;
    using System.Configuration.Install;
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
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
