namespace RadeonResetBugFixService.Tasks
{
    using Microsoft.Win32;
    using Contracts;

    class DisableBasicDisplayStartupTask : ITask
    {
        string ITask.TaskName => "Disabling basic display automatic start";

        void ITask.Run(ILogger logger)
        {
            var originalValue = Registry.GetValue(Constants.BasicDisplayRegistryKey, "Start", -1);
            logger.Log($"Original start value {originalValue}");
            Registry.SetValue(Constants.BasicDisplayRegistryKey, "Start", 4);
        }
    }
}
