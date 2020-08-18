namespace RadeonResetBugFixService
{
    using System;
    using System.Runtime.InteropServices;

    static class ConsoleHelper
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hWnd);
        }

        // Code taken from https://stackoverflow.com/a/53716169
        public static bool HaveVisibleConsole()
        {
            return NativeMethods.IsWindowVisible(NativeMethods.GetConsoleWindow());
        }
    }
}
