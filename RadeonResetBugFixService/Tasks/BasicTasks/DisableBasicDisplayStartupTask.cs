namespace RadeonResetBugFixService.Tasks.BasicTasks
{
    using System;
    using Contracts;
    using Devices;

    class DisableBasicDisplayStartupTask : ITask
    {
        string ITask.TaskName => "Disabling basic display automatic start";

        private DevicesStatus StartupDevicesStatus { get; }

        public DisableBasicDisplayStartupTask(DevicesStatus startupDevicesStatus)
        {
            if (startupDevicesStatus == null)
            {
                throw new ArgumentNullException(nameof(startupDevicesStatus));
            }

            this.StartupDevicesStatus = startupDevicesStatus;
        }

        void ITask.Run(ILogger logger)
        {
            foreach (var device in this.StartupDevicesStatus.DisabledDevices)
            {
                logger.Log($"Processing {device.Description}; should be enabled");
                var disabledStatus = DeviceHelper.IsDeviceCurrentlyDisabled(device);

                if (disabledStatus.HasValue && !disabledStatus.Value)
                {
                    logger.Log($"Device is enabled; disabling BasicDisplay service");
                    var originalValue = RegistryHelper.GetBasicDisplayStartType();
                    logger.Log($"Original start value {originalValue}");
                    RegistryHelper.SetBasicDisplayStartType(4);
                }
                else
                {
                    logger.Log($"Device is not enabled");
                }
            }
        }
    }
}
