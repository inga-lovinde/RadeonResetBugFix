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

        private static RegistryValuePath PreshutdownOrderPath = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "PreshutdownOrder");

        private static RegistryValuePath WaitToKillServiceTimeoutPath = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "WaitToKillServiceTimeout");

        private static RegistryValuePath FastRebootPath = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled");

        private static RegistryValuePath NoInteractiveServicesPath = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoInteractiveServices");

        private static RegistryValuePath BasicDisplayStartTypePath = new RegistryValuePath(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\BasicDisplay", "Start");

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
