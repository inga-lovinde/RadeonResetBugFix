namespace RadeonResetBugFixService.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    using System.Threading.Tasks;
    using Contracts;

    class DeviceHelper
    {
        private static T GetProperty<T>(PropertyDataCollection properties, string key)
        {
            try
            {
                return (T)properties[key].Value;
            }
            catch (Exception)
            {
                return default;
            }
        }

        private static Guid GuidTryParse(string input)
        {
            if (Guid.TryParse(input, out var result))
            {
                return result;
            }

            return default;
        }

        private static DeviceInfo ConvertDeviceInfo(PropertyDataCollection deviceProperties)
        {
            return new DeviceInfo
            {
                ClassGuid = GuidTryParse(GetProperty<string>(deviceProperties, "ClassGuid")),
                ClassName = GetProperty<string>(deviceProperties, "PNPClass") ?? string.Empty,
                DeviceId = GetProperty<string>(deviceProperties, "PNPDeviceId") ?? string.Empty,
                ErrorCode = GetProperty<UInt32>(deviceProperties, "ConfigManagerErrorCode"),
                IsPresent = GetProperty<bool>(deviceProperties, "Present"),
                Manufacturer = GetProperty<string>(deviceProperties, "Manufacturer") ?? string.Empty,
                Name = GetProperty<string>(deviceProperties, "Name") ?? string.Empty,
                Service = GetProperty<string>(deviceProperties, "Service") ?? string.Empty,
            };
        }

        public static IEnumerable<DeviceInfo> GetDevices()
        {
            ManagementPath path = new ManagementPath
            {
                Server = ".",
                NamespacePath = @"root\CIMV2",
                RelativePath = @"Win32_PnPentity",
            };

            using (var devs = new ManagementClass(new ManagementScope(path), path, new ObjectGetOptions(null, TimeSpan.FromMinutes(1), false)))
            {
                ManagementObjectCollection moc = devs.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    /*Console.WriteLine("===================================");
                    Console.WriteLine("New device: " + mo.Path.Path);
                    PropertyDataCollection devsProperties = mo.Properties;
                    foreach (PropertyData devProperty in devsProperties)
                    {
                        if (devProperty.Type != CimType.DateTime)
                        {
                            Console.WriteLine("Property = {0}\tValue = {1}\tType={2}", devProperty.Name, devProperty.Value, devProperty.Value?.GetType()?.Name);
                        }
                    }*/

                    yield return ConvertDeviceInfo(mo.Properties);
                }
            }
        }

        public static void DisableDevice(DeviceInfo deviceInfo)
        {
            RunWithTimeout(
                () => ThirdParty.DisableDevice.DeviceHelper.SetDeviceEnabled(deviceInfo.ClassGuid, deviceInfo.DeviceId, false),
                TimeSpan.FromSeconds(50));
        }

        public static void EnableDevice(DeviceInfo deviceInfo)
        {
            RunWithTimeout(
                () => ThirdParty.DisableDevice.DeviceHelper.SetDeviceEnabled(deviceInfo.ClassGuid, deviceInfo.DeviceId, true),
                TimeSpan.FromSeconds(50));
        }

        public static bool? IsDeviceCurrentlyDisabled(DeviceInfo deviceInfo)
        {
            return ThirdParty.DisableDevice.DeviceHelper.IsDeviceDisabled(deviceInfo.ClassGuid, deviceInfo.DeviceId);
        }

        private static void RunWithTimeout(Action action, TimeSpan timeout)
        {
            Task.WaitAny(
                Task.Run(action),
                Task.Delay(timeout));
        }
    }
}
