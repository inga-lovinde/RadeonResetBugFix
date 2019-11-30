namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class EnableVirtualVideoTask : AbstractDevicesTask
    {
        public override string TaskName => "Enabling virtual video";

        public EnableVirtualVideoTask(DevicesStatus devicesStatus) : base(devicesStatus)
        {
        }

        protected override bool ShouldEnable(DeviceInfo deviceInfo) => KnownDevices.IsVirtualVideo(deviceInfo);
    }
}
