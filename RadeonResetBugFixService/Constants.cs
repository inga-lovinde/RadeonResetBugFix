namespace RadeonResetBugFixService
{
    using System;
    using System.IO;
    using System.Reflection;

    static class Constants
    {
        public static TimeSpan ServiceTimeout { get; } = TimeSpan.FromMinutes(5);

        public static string ServiceName { get; } = "RadeonResetBugFixService";

        public static string LogDirectory { get; } = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "logs");
    }
}
