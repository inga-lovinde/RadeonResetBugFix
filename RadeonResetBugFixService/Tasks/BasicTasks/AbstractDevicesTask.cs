namespace RadeonResetBugFixService.Tasks.BasicTasks
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
                        if (device.ErrorCode != 0)
                        {
                            logger.LogError($"Device is in error state: {device.ErrorCode}");
                        }

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

                        if (device.ErrorCode != 0)
                        {
                            logger.LogError($"Device is in error state: {device.ErrorCode}");
                        }
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
                    if (device.IsDisabled)
                    {
                        logger.Log($"Successfully checked {device.Description} status");
                    }
                    else if (device.ErrorCode != 0)
                    {
                        logger.LogError($"Device is in error state: {device.ErrorCode}");
                    }
                    else
                    {
                        logger.LogError($"{device.Description} is enabled but should be disabled");
                    }
                }
                else if (this.ShouldEnable(device))
                {
                    if (device.IsDisabled)
                    {
                        logger.Log($"{device.Description} is disabled but should be enabled");
                    }
                    else if (device.ErrorCode != 0)
                    {
                        logger.LogError($"Device is in error state: {device.ErrorCode}");
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
