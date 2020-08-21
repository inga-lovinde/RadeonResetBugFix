namespace RadeonResetBugFixService.Tasks.BasicTasks
{
    using System;
    using System.ServiceProcess;

    class StopAudioServiceTask : AbstractServiceTask
    {
        public override string TaskName => "Stopping audio service";

        protected override bool ShouldStop(ServiceController serviceInfo)
        {
            return serviceInfo.ServiceName.Equals("audiosrv", StringComparison.OrdinalIgnoreCase);
        }
    }
}
