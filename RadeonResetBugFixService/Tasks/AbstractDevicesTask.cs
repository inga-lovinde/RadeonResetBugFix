namespace RadeonResetBugFixService.Tasks
{
    using System;
    using System.Linq;
    using Contracts;
    using Devices;

    abstract class AbstractDevicesTask : ITask
    {
        public abstract string TaskName { get; }

        private DevicesStatus DevicesStatus { get; }

        protected AbstractDevicesTask(DevicesStatus devicesStatus)
        {
            if (devicesStatus == null)
            {
                throw new ArgumentNullException(nameof(devicesStatus));
            }

            this.DevicesStatus = devicesStatus;
        }

        protected virtual bool ShouldDisable(DeviceInfo deviceInfo) => false;

        protected virtual bool ShouldEnable(DeviceInfo deviceInfo) => false;

        void ITask.Run(ILogger logger)
        {
            foreach (var device in DeviceHelper.GetDevices().ToArray())
            {
                if (this.ShouldDisable(device))
                {
                    this.DevicesStatus.DisabledDevices.Add(device);

                    if (device.IsDisabled)
                    {
                        logger.Log($"{device.Description} is already disabled");
                    }
                    else
                    {
                        logger.Log($"Disabling {device.Description}");
                        DeviceHelper.DisableDevice(device);
                        logger.Log($"Disabled {device.Description}");
                    }
                }
                else if (this.ShouldEnable(device))
                {
                    this.DevicesStatus.EnabledDevices.Add(device);

                    if (!device.IsDisabled)
                    {
                        logger.Log($"{device.Description} is already enabled");
                    }
                    else
                    {
                        logger.Log($"Enabling {device.Description}");
                        DeviceHelper.EnableDevice(device);
                        logger.Log($"Enabled {device.Description}");
                    }
                }
            }

            foreach (var device in DeviceHelper.GetDevices().ToArray())
            {
                if (this.ShouldDisable(device))
                {
                    if (!device.IsDisabled)
                    {
                        logger.LogError($"{device.Description} is enabled but should be disabled");
                    }
                    else
                    {
                        logger.Log($"Successfully checked {device.Description} status");
                    }
                }
                else if (this.ShouldEnable(device))
                {
                    if (device.IsDisabled)
                    {
                        logger.Log($"{device.Description} is disabled but should be enabled");
                    }
                    else
                    {
                        logger.Log($"Successfully checked {device.Description} status");
                    }
                }
            }
        }
    }
}
