using RadeonResetBugFixService.Contracts;

namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class DisableAmdVideoTask : AbstractDriverTask
    {
        public override string TaskName => "Disabling AMD video";
        protected override bool ShouldDisable(DeviceInfo deviceInfo) => KnownDevices.IsAmdVideo(deviceInfo);
    }
}
