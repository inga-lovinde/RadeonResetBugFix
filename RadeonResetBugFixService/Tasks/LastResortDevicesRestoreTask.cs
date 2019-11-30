namespace RadeonResetBugFixService.Tasks
{
    using System;
    using Contracts;
    using Devices;

    class LastResortDevicesRestoreTask : ITask
    {
        string ITask.TaskName => "Attempting to restore devices state";

        private DevicesStatus StartupDevicesStatus { get; }

        public LastResortDevicesRestoreTask(DevicesStatus startupDevicesStatus)
        {
            if (startupDevicesStatus == null)
            {
                throw new ArgumentNullException(nameof(startupDevicesStatus));
            }

            this.StartupDevicesStatus = startupDevicesStatus;
        }

        void ITask.Run(ILogger logger)
        {
            foreach (var device in this.StartupDevicesStatus.EnabledDevices)
            {
                logger.Log($"Processing {device.Description}; should be disabled");
                try
                {
                    var disabledStatus = DeviceHelper.IsDeviceCurrentlyDisabled(device);
                    if (!disabledStatus.HasValue)
                    {
                        logger.Log("Device not present");
                    }
                    else if (!disabledStatus.Value)
                    {
                        logger.Log("Device enabled; attempting to disable...");
                        DeviceHelper.DisableDevice(device);
                        logger.Log("Disabled device; checking status...");
                        var newStatus = DeviceHelper.IsDeviceCurrentlyDisabled(device);
                        if (!newStatus.HasValue)
                        {
                            logger.LogError("Device not present");
                        }
                        else if (!newStatus.Value)
                        {
                            logger.LogError("Device is enabled but should be disabled");
                        }
                        else
                        {
                            logger.Log("Successfully checked device status");
                        }
                    }
                    else
                    {
                        logger.Log("Device is disabled");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
            }

            foreach (var device in this.StartupDevicesStatus.DisabledDevices)
            {
                logger.Log($"Processing {device.Description}; should be enabled");
                try
                {
                    var disabledStatus = DeviceHelper.IsDeviceCurrentlyDisabled(device);
                    if (!disabledStatus.HasValue)
                    {
                        logger.Log("Device not present");
                    }
                    else if (disabledStatus.Value)
                    {
                        logger.Log("Device disabled; attempting to enable...");
                        DeviceHelper.EnableDevice(device);
                        logger.Log("Enabled device; checking status...");
                        var newStatus = DeviceHelper.IsDeviceCurrentlyDisabled(device);
                        if (!newStatus.HasValue)
                        {
                            logger.LogError("Device not present");
                        } else if (newStatus.Value)
                        {
                            logger.LogError("Device is disabled but should be enabled");
                        }
                        else
                        {
                            logger.Log("Successfully checked device status");
                        }
                    }
                    else
                    {
                        logger.Log("Device is enabled");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
            }
        }
    }
}
