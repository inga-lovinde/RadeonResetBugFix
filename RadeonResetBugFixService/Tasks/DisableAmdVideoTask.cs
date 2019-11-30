using RadeonResetBugFixService.Contracts;

namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class DisableAmdVideoTask : AbstractDevicesTask
    {
        public override string TaskName => "Disabling AMD video";

        public DisableAmdVideoTask(DevicesStatus devicesStatus) : base(devicesStatus)
        {
        }

        protected override bool ShouldDisable(DeviceInfo deviceInfo) => KnownDevices.IsAmdVideo(deviceInfo);
    }
}
