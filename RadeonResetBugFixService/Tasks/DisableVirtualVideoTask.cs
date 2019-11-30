namespace RadeonResetBugFixService.Tasks
{
    using Contracts;
    using Devices;

    class DisableVirtualVideoTask : AbstractDriverTask
    {
        public override string TaskName => "Disabling virtual video";

        protected override bool ShouldDisable(DeviceInfo deviceInfo) => KnownDevices.IsVirtualVideo(deviceInfo);
    }
}
