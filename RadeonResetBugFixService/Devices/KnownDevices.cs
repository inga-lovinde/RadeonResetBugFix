namespace RadeonResetBugFixService.Devices
{
    using Contracts;

    static class KnownDevices
    {
        public static bool IsAmdVideo(DeviceInfo device)
        {
            return (device.Manufacturer.ToLowerInvariant() == "amd" || device.Manufacturer.ToLowerInvariant().Contains("advanced micro devices")) &&
                (device.Service.ToLowerInvariant() == "hdaudbus" || device.ClassName.ToLowerInvariant() == "display");
        }

        public static bool IsVirtualVideo(DeviceInfo device)
        {
            return device.Service.ToLowerInvariant() == "hypervideo";
        }
    }
}
