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

        public void HandleLog(string message)
        {
            using (ILogger fileLogger = new FileLogger(this.LogFilename))
            {
                fileLogger.Log(message);
            }
        }

        public void HandleStartup(string reason)
        {
            using (var fileLogger = new FileLogger(this.LogFilename))
            {
                using (ILogger logger = new TaskLoggerWrapper(fileLogger, "Startup"))
                {
                    logger.Log($"Reason: {reason}");
                    try
                    {
                        lock (this.Mutex)
                        {
                            TasksProcessor.ProcessTasks(
                                logger,
                                new ITask[]
                                {
                                    new EnableBasicDisplayStartupTask(),
                                    new SleepTask(TimeSpan.FromSeconds(40)),
                                    new EnableAmdVideoTask(this.StartupDevicesStatus),
                                    new DisableVirtualVideoTask(this.StartupDevicesStatus),
                                    new SleepTask(TimeSpan.FromSeconds(20)),
                                    new FixMonitorTask(),
                                    new DisableVirtualVideoTask(this.StartupDevicesStatus),
                                    new FixMonitorTask(),
                                });
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.ToString());
                    }
                }
            }
        }

        public void HandleShutdown(string reason)
        {
            using (var fileLogger = new FileLogger(this.LogFilename))
            {
                using (ILogger logger = new TaskLoggerWrapper(fileLogger, "Shutdown"))
                {
                    logger.Log($"Reason: {reason}");
                    try
                    {
                        lock (this.Mutex)
                        {
                            TasksProcessor.ProcessTasks(
                                logger,
                                new ITask[]
                                {
                                    new StopAudioServiceTask(),
                                    new EnableVirtualVideoTask(this.ShutdownDevicesStatus),
                                    new DisableAmdVideoTask(this.ShutdownDevicesStatus),
                                    new LastResortDevicesRestoreTask(this.StartupDevicesStatus),
                                    new LastResortDevicesRestoreTask(this.StartupDevicesStatus), // just in case
                                    new DisableBasicDisplayStartupTask(this.StartupDevicesStatus),
                                });
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
