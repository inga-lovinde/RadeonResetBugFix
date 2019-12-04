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
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

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

            var command = args[0];
            if (command.Equals("install", StringComparison.OrdinalIgnoreCase)) {
                DoInstall();
            }
            else if (command.Equals("uninstall", StringComparison.OrdinalIgnoreCase))
            {
                DoUninstall();
            }
            else if (command.Equals("startup", StringComparison.OrdinalIgnoreCase))
            {
                DoStartup();
            }
            else if (command.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
            {
                DoShutdown();
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
            Console.WriteLine("Setting registry values...");

            Console.WriteLine("Installing service...");
            ServiceHelpers.InstallService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
            Console.WriteLine("Starting service...");
            ServiceHelpers.StartService(nameof(RadeonResetBugFixService));
            Console.WriteLine("Started service");
        }

        private static void DoUninstall()
        {
            Console.WriteLine("Stopping service...");
            ServiceHelpers.StopService(nameof(RadeonResetBugFixService));
            Console.WriteLine("Uninstalling service...");
            ServiceHelpers.UninstallService(nameof(RadeonResetBugFixService), typeof(RadeonResetBugFixService));
            Console.WriteLine("Uninstalled");
        }

        private static void DoStartup()
        {
            new MainHandler().HandleStartup("Program.DoStartup");
        }

        private static void DoShutdown()
        {
            new MainHandler().HandleShutdown("Program.DoShutdown");
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
