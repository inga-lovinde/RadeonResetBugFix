namespace RadeonResetBugFixService.Tasks.ComplexTasks
{
    using BasicTasks;
    using Contracts;
    using System;

    class StartupTask : AbstractSequentialTask
    {
        private DevicesStatus StartupDevicesStatus { get; } = new DevicesStatus();

        public override string TaskName => "Startup";

        protected override ITask[] Subtasks => new ITask[]
        {
            new EnableBasicDisplayStartupTask(),
            new SleepTask(TimeSpan.FromSeconds(40)),
            new EnableAmdVideoTask(this.StartupDevicesStatus),
            new DisableVirtualVideoTask(this.StartupDevicesStatus),
            new SleepTask(TimeSpan.FromSeconds(20)),
            new FixMonitorTask(),
            new DisableVirtualVideoTask(this.StartupDevicesStatus),
            new FixMonitorTask()
        };
    }
}
