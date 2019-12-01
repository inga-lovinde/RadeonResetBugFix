namespace RadeonResetBugFixService.Tasks
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Devices;

    class FixMonitorTask : ITask
    {
        public string TaskName => "Fixing monitor";

        public void Run(ILogger logger)
        {
            var hypervDisplays = new List<ThirdParty.MonitorChanger.DISPLAY_DEVICE>();
            var realDisplays = new List<ThirdParty.MonitorChanger.DISPLAY_DEVICE>();
            foreach (var display in ThirdParty.MonitorChanger.Display.GetDisplayList())
            {
                logger.Log($"Found display(ID='{display.DeviceID}' Key='{display.DeviceKey}', Name='{display.DeviceName}', String='{display.DeviceString}')");
                if (display.DeviceID.ToLowerInvariant().StartsWith(@"monitor\mhs062e"))
                {
                    hypervDisplays.Add(display);
                }
                else
                {
                    realDisplays.Add(display);
                }
            }

            logger.Log($"Found {hypervDisplays.Count} virtual displays and {realDisplays.Count} real displays");
            if (hypervDisplays.Count > 0 && realDisplays.Count > 0)
            {
                var newPrimaryDisplay = realDisplays[0];
                logger.Log($"Setting default display to {newPrimaryDisplay.DeviceID}");
                ThirdParty.MonitorChanger.Display.SetAsPrimaryMonitor(newPrimaryDisplay);
            }
        }
    }
}
