namespace RadeonResetBugFixService.Tasks
{
    using System.Linq;
    using Contracts;
    using Devices;

    class ListDevicesTask : ITask
    {
        string ITask.TaskName => "Listing devices";

        void ITask.Run(ILogger logger)
        {
            foreach (var device in DeviceHelper.GetDevices().ToArray())
            {
                logger.Log($"Found device {device.Description}: manufacturer='{device.Manufacturer}', service='{device.Service}', class='{device.ClassName}'");
            }
        }
    }
}
