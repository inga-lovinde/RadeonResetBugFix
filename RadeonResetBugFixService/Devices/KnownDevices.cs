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
                "VBoxVideo", // virtualbox for some windows versions?
                "VBoxWDDM", // virtualbox for Win8+ (or for Win7 too?)
                "vm3dmp", // one of these supports "VMWare SVGA 3D"
                "vm3dmp-debug", // one of these supports "VMWare SVGA 3D"
                "vm3dmp-stats", // one of these supports "VMWare SVGA 3D"
                "vm3dmp_loader", // one of these supports "VMWare SVGA 3D"
                "VM3DService ", // one of these supports "VMWare SVGA 3D"
            }.Contains(device.Service, StringComparer.OrdinalIgnoreCase);
        }
    }
}
