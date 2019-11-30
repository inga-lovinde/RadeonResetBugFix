namespace RadeonResetBugFixService
{
    using System;
    using System.Security.Principal;
    using System.ServiceProcess;
    using ThirdParty.ServiceHelpers;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static int Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (!HasAdministratorPrivileges())
                {
                    Console.Error.WriteLine("Access Denied.");
                    Console.Error.WriteLine("Administrator permissions are  needed to use the selected options.");
                    Console.Error.WriteLine("Use an administrator command prompt to complete these tasks.");
                    return 740; // ERROR_ELEVATION_REQUIRED
                }

                MainConsole(args);
                return 0;
            }
            else
            {
                return MainService();
            }
        }

        private static int MainService()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RadeonResetBugFixService()
            };
            ServiceBase.Run(ServicesToRun);

            return 0;
        }

        private static void MainConsole(string[] args)
        {
            if (args.Length != 1)
            {
                ShowHelp();
                return;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "install":
                    DoInstall();
                    return;
                case "uninstall":
                    DoUninstall();
                    return;
                case "startup":
                    DoStartup();
                    return;
                case "shutdown":
                    DoShutdown();
                    return;
                default:
                    ShowHelp();
                    return;
            }
        }

        private static void ShowHelp()
        {
            var exeName = Environment.GetCommandLineArgs()[0];
            Console.WriteLine("Usage:");
            Console.WriteLine($"\t{exeName} install");
            Console.WriteLine("\t\tInstalls service");
            Console.WriteLine($"\t{exeName} uninstall");
            Console.WriteLine("\t\tUninstalls service");
            Console.WriteLine($"\t{exeName} startup");
            Console.WriteLine("\t\tPerforms startup sequence");
            Console.WriteLine($"\t{exeName} shutdown");
            Console.WriteLine("\t\tPerforms shutdown sequence");
        }

        private static void DoInstall()
        {
            ServiceHelpers.InstallService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
            ServiceHelpers.StartService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
        }

        private static void DoUninstall()
        {
            ServiceHelpers.StopService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
            ServiceHelpers.UninstallService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
        }

        private static void DoStartup()
        {
            new MainHandler().HandleStartup();
        }

        private static void DoShutdown()
        {
            new MainHandler().HandleShutdown();
        }

        // Code taken from https://stackoverflow.com/a/2679654
        private static bool HasAdministratorPrivileges()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
