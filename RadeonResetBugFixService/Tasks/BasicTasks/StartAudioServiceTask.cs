namespace RadeonResetBugFixService.Tasks.BasicTasks
{
    using System;
    using System.ServiceProcess;

    class StartAudioServiceTask : AbstractServiceTask
    {
        public override string TaskName => "Stopping audio service";

        protected override bool ShouldStart(ServiceController serviceInfo)
        {
            return serviceInfo.ServiceName.Equals("audiosrv", StringComparison.OrdinalIgnoreCase);
        }
    }
}
