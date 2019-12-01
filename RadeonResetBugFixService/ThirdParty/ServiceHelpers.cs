namespace RadeonResetBugFixService.ThirdParty.ServiceHelpers
{
    // Code taken from https://stackoverflow.com/a/1195621 and lightly modified

    using System;
    using System.Collections;
    using System.Configuration.Install;
    using System.ServiceProcess;

    class ServiceHelpers
    {
        public static bool IsInstalled(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public static bool IsRunning(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (!IsInstalled(serviceName)) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        public static AssemblyInstaller GetInstaller(Type serviceType)
        {
            AssemblyInstaller installer = new AssemblyInstaller(serviceType.Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        public static void InstallService(string serviceName, Type serviceType)
        {
            if (IsInstalled(serviceName))
            {
                Console.WriteLine("Already installed");
                return;
            }

            using (AssemblyInstaller installer = GetInstaller(serviceType))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                    Console.WriteLine("installed");
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch { }
                    throw;
                }
            }
        }

        public static void UninstallService(string serviceName, Type serviceType)
        {
            if (!IsInstalled(serviceName))
            {
                Console.WriteLine("Service not installed");
                return;
            }

            using (AssemblyInstaller installer = GetInstaller(serviceType))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Uninstall(state);
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void StartService(string serviceName, Type serviceType)
        {
            if (!IsInstalled(serviceName)) return;

            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(6));
                }
            }
        }

        public static void StopService(string serviceName, Type serviceType)
        {
            if (!IsInstalled(serviceName)) return;
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(6));
                }
            }
        }
    }
}
