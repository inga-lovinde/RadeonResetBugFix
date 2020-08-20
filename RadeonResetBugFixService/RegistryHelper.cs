namespace RadeonResetBugFixService
{
    using System;
    using System.Globalization;
    using Microsoft.Win32;

    static class RegistryHelper
    {
        private class RegistryValuePath
        {
            public string KeyName { get; }

            public string ValueName { get; }

            public RegistryValuePath(string keyName, string valueName)
            {
                this.KeyName = keyName;
                this.ValueName = valueName;
            }
        }

        private static string BasicDisplayServiceName { get; } = (
            EnvironmentHelper.IsWindows8OrNewer()
                ? "BasicDisplay"
                : "vga"
        );

        private static RegistryValuePath PreshutdownOrderPath { get; } = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "PreshutdownOrder");

        private static RegistryValuePath WaitToKillServiceTimeoutPath { get; } = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "WaitToKillServiceTimeout");

        private static RegistryValuePath FastRebootPath { get; } = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");

        private static RegistryValuePath NoInteractiveServicesPath { get; } = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoInteractiveServices");

        private static RegistryValuePath BasicDisplayStartTypePath { get; } = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + BasicDisplayServiceName, "Start");

        private static T GetValue<T>(RegistryValuePath path, T defaultValue = default) => (T)Registry.GetValue(path.KeyName, path.ValueName, defaultValue);

        private static void SetValue<T>(RegistryValuePath path, T value, RegistryValueKind valueKind) => Registry.SetValue(path.KeyName, path.ValueName, value, valueKind);

        private static void SetValue(RegistryValuePath path, int value) => SetValue(path, value, RegistryValueKind.DWord);

        private static void SetValue(RegistryValuePath path, string value) => SetValue(path, value, RegistryValueKind.String);

        private static void SetValue(RegistryValuePath path, string[] value) => SetValue(path, value, RegistryValueKind.MultiString);

        public static string[] GetPreshutdownOrder() => GetValue(PreshutdownOrderPath, Array.Empty<string>());

        public static void SetPreshutdownOrder(string[] value) => SetValue(PreshutdownOrderPath, value);

        public static void SetWaitToKillServiceTimeout(int milliseconds) => SetValue(WaitToKillServiceTimeoutPath, milliseconds.ToString(CultureInfo.InvariantCulture));

        public static void SetFastRebootStatus(bool isEnabled) => SetValue(FastRebootPath, isEnabled ? 1 : 0);

        public static void SetInteractiveServicesStatus(bool areInteractiveServicesAllowed) => SetValue(NoInteractiveServicesPath, areInteractiveServicesAllowed ? 0 : 1);

        public static int GetBasicDisplayStartType() => GetValue(BasicDisplayStartTypePath, -1);

        public static void SetBasicDisplayStartType(int startType) => SetValue(BasicDisplayStartTypePath, startType);
    }
}
