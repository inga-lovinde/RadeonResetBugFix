namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class EnableAmdVideoTask : AbstractDevicesTask
    {
        public override string TaskName => "Enabling AMD video";

        public EnableAmdVideoTask(DevicesStatus devicesStatus) : base(devicesStatus)
        {
        }

        protected override bool ShouldEnable(DeviceInfo deviceInfo) => KnownDevices.IsAmdVideo(deviceInfo);

    }
}
