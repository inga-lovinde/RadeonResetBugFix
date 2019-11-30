namespace RadeonResetBugFixService.Logging
{
    using System;
    using System.IO;
    using Contracts;

    class FileLogger : ILogger
    {
        private string Filename { get; }

        public FileLogger(string filename)
        {
            this.Filename = filename;
        }

        private void LogString(string message) => File.AppendAllLines(this.Filename, new[] { $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}" });

        void ILogger.Log(string message) => LogString(message);

        void ILogger.LogError(string message) => LogString($"Error: {message}");

        void IDisposable.Dispose()
        {
        }
    }
}
