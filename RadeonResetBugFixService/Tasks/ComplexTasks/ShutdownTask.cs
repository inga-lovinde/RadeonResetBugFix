namespace RadeonResetBugFixService.Tasks.ComplexTasks
{
    using BasicTasks;
    using Contracts;
    using System;

    class ShutdownTask : AbstractSequentialTask
    {
        public ShutdownTask(ServiceContext context)
        {
            this.Context = context;
        }

        private ServiceContext Context { get; }

        public override string TaskName => "Shutdown";

        protected override ITask[] Subtasks => new ITask[]
        {
            new StopAudioServiceTask(),
            new EnableVirtualVideoTask(this.Context.ShutdownDevicesStatus),
            new DisableAmdVideoTask(this.Context.ShutdownDevicesStatus),
            new LastResortDevicesRestoreTask(this.Context.StartupDevicesStatus),
            new LastResortDevicesRestoreTask(this.Context.StartupDevicesStatus), // just in case
            new StartAudioServiceTask(),
            new DisableBasicDisplayStartupTask(this.Context.StartupDevicesStatus),
        };
    }
}
