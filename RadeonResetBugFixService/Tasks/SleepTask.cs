namespace RadeonResetBugFixService.Tasks
{
    using System;
    using System.Threading;
    using Contracts;

    class SleepTask : ITask
    {
        private TimeSpan SleepTime { get; }

        public string TaskName => "Sleep";

        public SleepTask(TimeSpan sleepTime)
        {
            this.SleepTime = sleepTime;
        }

        public void Run(ILogger logger)
        {
            Thread.Sleep(this.SleepTime);
        }
    }
}
