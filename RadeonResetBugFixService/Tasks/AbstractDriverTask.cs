namespace RadeonResetBugFixService.Tasks
{
    using System.Linq;
    using Contracts;
    using Devices;

    abstract class AbstractDriverTask : ITask
    {
        protected virtual bool ShouldDisable(DeviceInfo deviceInfo) => false;

        protected virtual bool ShouldEnable(DeviceInfo deviceInfo) => false;

        public abstract string TaskName { get; }

        void ITask.Run(ILogger logger)
        {
            var devices = DeviceHelper.GetDevices().ToArray();
            foreach (var device in devices)
            {
                //logger.Log($"Present({device.IsPresent}) ErrorCode({device.ErrorCode}) Disabled({device.IsDisabled}) ClassName({device.ClassName}) Service({device.Service}) Manufacturer({device.Manufacturer}) Name({device.Name}) Id({device.DeviceId}) ClassGuid({device.ClassGuid})");
                var deviceDescription = $"{device.Name} ({device.DeviceId}, {device.ClassGuid})";

                if (this.ShouldDisable(device))
                {
                    if (device.IsDisabled)
                    {
                        logger.Log($"{deviceDescription} already disabled");
                    }
                    else
                    {
                        logger.Log($"Disabling {deviceDescription}");
                        DeviceHelper.DisableDevice(device);
                        logger.Log($"Disabled {deviceDescription}");
                    }
                }
                else if (this.ShouldEnable(device))
                {
                    if (!device.IsDisabled)
                    {
                        logger.Log($"{deviceDescription} already enabled");
                    }
                    else
                    {
                        logger.Log($"Enabling {deviceDescription}");
                        DeviceHelper.EnableDevice(device);
                        logger.Log($"Enabled {deviceDescription}");
                    }
                }
            }
        }
    }
}
