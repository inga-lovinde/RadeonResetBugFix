namespace RadeonResetBugFixService.Tasks
{
    using Microsoft.Win32;
    using Contracts;

    class EnableBasicDisplayStartupTask : ITask
    {
        string ITask.TaskName => "Enabling basic display automatic start";

        void ITask.Run(ILogger logger)
        {
            var originalValue = Registry.GetValue(Constants.RegistryKeyBasicDisplay, "Start", -1);
            logger.Log($"Original start value {originalValue}");
            Registry.SetValue(Constants.RegistryKeyBasicDisplay, "Start", 3);
        }
    }
}
