namespace RadeonResetBugFixService.Tasks
{
    using Contracts;

    interface ITask
    {
        string TaskName { get; }

        void Run(ILogger logger);
    }
}
