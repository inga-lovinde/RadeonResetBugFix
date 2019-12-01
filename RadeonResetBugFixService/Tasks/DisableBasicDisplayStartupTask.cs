namespace RadeonResetBugFixService.Tasks
{
    using System;
    using Microsoft.Win32;
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
                    var originalValue = Registry.GetValue(Constants.RegistryKeyBasicDisplay, "Start", -1);
                    logger.Log($"Original start value {originalValue}");
                    Registry.SetValue(Constants.RegistryKeyBasicDisplay, "Start", 4);
                }
            }
        }
    }
}
