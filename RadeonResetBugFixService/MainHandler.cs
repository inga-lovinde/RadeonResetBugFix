namespace RadeonResetBugFixService
{
    using System;
    using System.IO;
    using System.Reflection;
    using Contracts;
    using Logging;
    using Tasks;

    class MainHandler
    {
        private string LogFilename { get; }

        private DevicesStatus StartupDevicesStatus { get; } = new DevicesStatus();

        private DevicesStatus ShutdownDevicesStatus { get; } = new DevicesStatus();

        private object Mutex = new object();

        public MainHandler()
        {
            var date = DateTime.Now;
            this.LogFilename = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "logs",
                $"radeonfix_{date:yyyyMMdd}_{date:HHmmss}.log");
        }

        public void HandleStartup()
        {
            using (var fileLogger = new FileLogger(this.LogFilename))
            {
                using (ILogger logger = new TaskLoggerWrapper(fileLogger, "Startup"))
                {
                    try
                    {
                        lock (this.Mutex)
                        {
                            TasksProcessor.ProcessTasks(
                                logger,
                                new ITask[]
                                {
                                    new SleepTask(TimeSpan.FromSeconds(20)),
                                    new DisableVirtualVideoTask(this.StartupDevicesStatus),
                                    new EnableAmdVideoTask(this.StartupDevicesStatus),
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

        public void HandleShutdown()
        {
            using (var fileLogger = new FileLogger(this.LogFilename))
            {
                using (ILogger logger = new TaskLoggerWrapper(fileLogger, "Shutdown"))
                {
                    try
                    {
                        lock (this.Mutex)
                        {
                            TasksProcessor.ProcessTasks(
                                logger,
                                new ITask[]
                                {
                                    new StopAudioServiceTask(),
                                    new DisableAmdVideoTask(this.ShutdownDevicesStatus),
                                    new EnableVirtualVideoTask(this.ShutdownDevicesStatus),
                                    new LastResortDevicesRestoreTask(this.StartupDevicesStatus),
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
