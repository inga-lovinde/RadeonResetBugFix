namespace RadeonResetBugFixService.Contracts
{
    using System.Collections.Generic;

    class DevicesStatus
    {
        public List<DeviceInfo> EnabledDevices { get; } = new List<DeviceInfo>();

        public List<DeviceInfo> DisabledDevices { get; } = new List<DeviceInfo>();
    }
}
