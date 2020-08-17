namespace RadeonResetBugFixService.Devices
{
    using System;
    using Contracts;

    static class KnownDevices
    {
        public static bool IsAmdVideo(DeviceInfo device)
        {
            return ((device.Manufacturer.Equals("AMD", StringComparison.OrdinalIgnoreCase) || device.Manufacturer.IndexOf("Advanced Micro Devices", StringComparison.OrdinalIgnoreCase) >= 0) &&
                (device.Service.Equals("hdaudbus", StringComparison.OrdinalIgnoreCase) || device.ClassName.Equals("display", StringComparison.OrdinalIgnoreCase)));
        }

        public static bool IsVirtualVideo(DeviceInfo device)
        {
            return (
                device.Service.Equals("hypervideo", StringComparison.OrdinalIgnoreCase) || // Hyper-V video adapter
                device.Service.Equals("qxldod", StringComparison.OrdinalIgnoreCase) // virtio/libvirt for Win8+
            );
        }
    }
}
