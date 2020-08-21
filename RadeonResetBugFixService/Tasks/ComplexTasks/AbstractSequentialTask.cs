namespace RadeonResetBugFixService.Tasks.ComplexTasks
{
    using Contracts;

    abstract class AbstractSequentialTask : ITask
    {
        public abstract string TaskName { get; }

        protected abstract ITask[] Subtasks { get; }

        void ITask.Run(ILogger logger) => TasksProcessor.ProcessTasks(logger, this.Subtasks);
    }
}
