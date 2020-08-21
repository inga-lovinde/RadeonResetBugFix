namespace RadeonResetBugFixService.Tasks.ComplexTasks
{
    using BasicTasks;
    using Contracts;
    using System;

    class DiagnoseTask : AbstractSequentialTask
    {
        public override string TaskName => "Diagnose";

        protected override ITask[] Subtasks => new ITask[]
        {
            new ListDevicesTask(),
        };
    }
}
