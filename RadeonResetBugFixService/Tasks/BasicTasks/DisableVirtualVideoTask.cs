namespace RadeonResetBugFixService.Tasks.BasicTasks
{
    using Contracts;
    using Devices;

    class DisableVirtualVideoTask : AbstractDevicesTask
    {
        public override string TaskName => "Disabling virtual video";

        public DisableVirtualVideoTask(DevicesStatus devicesStatus) : base(devicesStatus)
        {
        }

        protected override bool ShouldDisable(DeviceInfo deviceInfo) => KnownDevices.IsVirtualVideo(deviceInfo);
    }
}
