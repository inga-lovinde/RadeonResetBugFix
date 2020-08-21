namespace RadeonResetBugFixService.Tasks.ComplexTasks
{
    using BasicTasks;
    using Contracts;
    using System;

    class ShutdownTask : AbstractSequentialTask
    {
        private DevicesStatus ShutdownDevicesStatus { get; } = new DevicesStatus();

        public override string TaskName => "Shutdown";

        protected override ITask[] Subtasks => new ITask[]
        {
            new EnableBasicDisplayStartupTask(),
            new SleepTask(TimeSpan.FromSeconds(40)),
            new EnableAmdVideoTask(this.ShutdownDevicesStatus),
            new DisableVirtualVideoTask(this.ShutdownDevicesStatus),
            new SleepTask(TimeSpan.FromSeconds(20)),
            new FixMonitorTask(),
            new DisableVirtualVideoTask(this.ShutdownDevicesStatus),
            new FixMonitorTask()
        };
    }
}
