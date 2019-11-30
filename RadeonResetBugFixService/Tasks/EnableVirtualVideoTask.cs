namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class EnableVirtualVideoTask : AbstractDriverTask
    {
        public override string TaskName => "Enabling virtual video";

        protected override bool ShouldEnable(DeviceInfo deviceInfo) => KnownDevices.IsVirtualVideo(deviceInfo);
    }
}
