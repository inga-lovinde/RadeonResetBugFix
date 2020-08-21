namespace RadeonResetBugFixService
{
    using System;
    using System.IO;
    using Contracts;
    using Logging;
    using Tasks;

    class MainHandler
    {
        private string LogFilename { get; }

        private DevicesStatus StartupDevicesStatus { get; } = new DevicesStatus();

        private DevicesStatus ShutdownDevicesStatus { get; } = new DevicesStatus();

        private readonly object Mutex = new object();

        public MainHandler()
        {
            var date = DateTime.Now;
            this.LogFilename = Path.Combine(
                Constants.LogDirectory,
                $"radeonfix_{date:yyyyMMdd}_{date:HHmmss}.log");
        }

        public void HandleEntryPoint(string name, Action<ILogger> handle)
        {
            using (var fileLogger = new FileLogger(this.LogFilename))
            {
                using (ILogger logger = new TaskLoggerWrapper(fileLogger, name))
                {
                    logger.Log($"Build date: {EnvironmentHelper.GetServiceBuildDate()}");

                    try
                    {
                        lock (this.Mutex)
                        {
                            handle(logger);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.ToString());
                    }
                }
            }
        }
    }
}
