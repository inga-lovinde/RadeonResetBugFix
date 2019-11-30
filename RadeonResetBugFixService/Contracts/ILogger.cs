namespace RadeonResetBugFixService.Contracts
{
    using System;

    interface ILogger : IDisposable
    {
        void Log(string message);
        void LogError(string message);
    }
}
