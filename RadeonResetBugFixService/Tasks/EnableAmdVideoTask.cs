namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class EnableAmdVideoTask : AbstractDriverTask
    {
        public override string TaskName => "Enabling AMD video";

        protected override bool ShouldEnable(DeviceInfo deviceInfo) => KnownDevices.IsAmdVideo(deviceInfo);

    }
}
