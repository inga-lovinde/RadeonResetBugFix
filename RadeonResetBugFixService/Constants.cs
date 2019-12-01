namespace RadeonResetBugFixService
{
    using System;

    static class Constants
    {
        public static TimeSpan ServiceTimeout { get; } = TimeSpan.FromMinutes(5);

        public static string RegistryKeyBasicDisplay { get; } = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\BasicDisplay";

        public static string RegistryKeySystemControl { get; } = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control";
    }
}
