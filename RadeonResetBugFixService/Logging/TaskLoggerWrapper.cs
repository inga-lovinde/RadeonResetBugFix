namespace RadeonResetBugFixService.Logging
{
    using System;
    using Contracts;

    class TaskLoggerWrapper : ILogger
    {
        private ILogger InnerLogger { get; }

        private string Prefix { get; }

        public TaskLoggerWrapper(ILogger innerLogger, string taskName)
        {
            this.InnerLogger = innerLogger;
            this.Prefix = $"[{taskName}]";

            innerLogger.Log($"{this.Prefix} begin");
        }

        void ILogger.Log(string message) => this.InnerLogger.Log($"{this.Prefix} {message}");

        void ILogger.LogError(string message) => this.InnerLogger.LogError($"{this.Prefix} {message}");

        void IDisposable.Dispose() => this.InnerLogger.Log($"{this.Prefix} end");
    }
}
