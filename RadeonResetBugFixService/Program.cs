namespace RadeonResetBugFixService
{
    using System;
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

            if (!EnvironmentHelper.IsVistaOrNewer())
            {
                Console.Error.WriteLine("This program only runs on Windows Vista or newer");
                return -1;
            }

            if (EnvironmentHelper.IsConsoleVisibleOnWindows())
            {
                if (!EnvironmentHelper.HasAdministratorPrivileges())
                {
                    Console.Error.WriteLine("Access Denied.");
                    Console.Error.WriteLine("Administrator permissions are needed to use this tool.");
                    Console.Error.WriteLine("Run the command again from an administrator command prompt.");
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Windows service initialization")]
        private static int MainService()
        {
            ServiceBase.Run(new RadeonResetBugFixService());

            return 0;
        }

        private static void MainConsole(string[] args)
        {
            var command = args.Length == 1 ? args[0] : string.Empty;

            if (command.Equals("install", StringComparison.OrdinalIgnoreCase)) {
                DoInstall();
            }
            else if (command.Equals("uninstall", StringComparison.OrdinalIgnoreCase))
            {
                DoUninstall();
            }
            else if (command.Equals("reinstall", StringComparison.OrdinalIgnoreCase))
            {
                DoReinstall();
            }
            else if (command.Equals("startup", StringComparison.OrdinalIgnoreCase))
            {
                DoStartup();
            }
            else if (command.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
            {
                DoShutdown();
            }
            else
            {
                ShowHelp();
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
            Console.WriteLine($"\t{exeName} reinstall");
            Console.WriteLine("\t\tReinstalls service (might be useful for some upgrades)");
            Console.WriteLine($"\t{exeName} startup");
            Console.WriteLine("\t\tPerforms startup sequence (development command, does not affect services)");
            Console.WriteLine($"\t{exeName} shutdown");
            Console.WriteLine("\t\tPerforms shutdown sequence (development command, does not affect services)");
        }

        private static void DoInstall()
        {
            Console.WriteLine("Setting registry values...");

            Console.WriteLine("Installing service...");
            ServiceHelpers.InstallService(Constants.ServiceName, typeof(RadeonResetBugFixService));
            Console.WriteLine("Starting service...");
            ServiceHelpers.StartService(Constants.ServiceName);
            Console.WriteLine("Should restart service now; stopping service...");
            ServiceHelpers.StopService(Constants.ServiceName);
            Console.WriteLine("Starting service...");
            ServiceHelpers.StartService(Constants.ServiceName);
            Console.WriteLine("Service started");
        }

        private static void DoUninstall()
        {
            Console.WriteLine("Stopping service...");
            ServiceHelpers.StopService(Constants.ServiceName);
            Console.WriteLine("Uninstalling service...");
            ServiceHelpers.UninstallService(Constants.ServiceName, typeof(RadeonResetBugFixService));
            Console.WriteLine("Uninstalled");
        }

        private static void DoReinstall()
        {
            Console.WriteLine("Attempting to uninstall...");
            DoUninstall();

            Console.WriteLine("Attempting to install...");
            DoInstall();
        }

        private static void DoStartup()
        {
            new MainHandler().HandleStartup("Program.DoStartup");
        }

        private static void DoShutdown()
        {
            new MainHandler().HandleShutdown("Program.DoShutdown");
        }
    }
}
