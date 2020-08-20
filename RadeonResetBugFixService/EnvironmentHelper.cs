namespace RadeonResetBugFixService
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    static class EnvironmentHelper
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hWnd);
        }

        private static Version VistaVersion { get; } = new Version(6, 0);

        private static Version Windows8Version { get; } = new Version(6, 2);

        // Code taken from https://stackoverflow.com/a/53716169
        public static bool IsConsoleVisibleOnWindows() => NativeMethods.IsWindowVisible(NativeMethods.GetConsoleWindow());

        private static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        // Code taken from https://stackoverflow.com/a/2679654
        public static bool HasAdministratorPrivileges()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsWindows8OrNewer() => IsWindows() && Environment.OSVersion.Version >= Windows8Version;

        public static bool IsVistaOrNewer() => IsWindows() && Environment.OSVersion.Version >= VistaVersion;
    }
}
