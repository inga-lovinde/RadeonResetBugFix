namespace RadeonResetBugFixService.Contracts
{
    class ServiceContext
    {
        public DevicesStatus StartupDevicesStatus { get; } = new DevicesStatus();

        public DevicesStatus ShutdownDevicesStatus { get; } = new DevicesStatus();
    }
}
