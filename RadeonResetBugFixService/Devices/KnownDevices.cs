namespace RadeonResetBugFixService.Devices
{
    using System;
    using System.Linq;
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
            return new[]
            {
                "hypervideo", // Hyper-V video adapter
                "qxldod", // virtio/libvirt for Win8+
                "qxl", // virtio/libvirt for Win7
            }.Contains(device.Service, StringComparer.OrdinalIgnoreCase);
        }
    }
}
