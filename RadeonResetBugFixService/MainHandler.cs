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
                    lock (this.Mutex)
                    {
                        TasksProcessor.ProcessTasks(
                            logger,
                            new ITask[]
                            {
                            new DisableVirtualVideoTask(),
                            new EnableAmdVideoTask(),
                            });
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
                    lock (this.Mutex)
                    {
                        TasksProcessor.ProcessTasks(
                            logger,
                            new ITask[]
                            {
                                new StopAudioServiceTask(),
                                new DisableAmdVideoTask(),
                                new EnableVirtualVideoTask(),
                            });
                    }
                }
            }
        }
    }
}
