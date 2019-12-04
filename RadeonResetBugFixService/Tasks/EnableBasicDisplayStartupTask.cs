namespace RadeonResetBugFixService.Tasks
{
    using Contracts;

    class EnableBasicDisplayStartupTask : ITask
    {
        string ITask.TaskName => "Enabling basic display automatic start";

        void ITask.Run(ILogger logger)
        {
            var originalValue = RegistryHelper.GetBasicDisplayStartType();
            logger.Log($"Original start value {originalValue}");
            RegistryHelper.SetBasicDisplayStartType(3);
        }
    }
}
