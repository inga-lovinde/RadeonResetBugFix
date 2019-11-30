using System.ServiceProcess;

namespace RadeonResetBugFixService.Tasks
{
    class StopAudioServiceTask : AbstractServiceTask
    {
        public override string TaskName => "Stopping audio service";

        protected override bool ShouldStop(ServiceController serviceInfo)
        {
            return serviceInfo.ServiceName.ToLowerInvariant() == "audiosrv";
        }
    }
}
