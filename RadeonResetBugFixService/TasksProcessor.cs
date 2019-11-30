namespace RadeonResetBugFixService
{
    using System;
    using Contracts;
    using Logging;
    using Tasks;

    static class TasksProcessor
    {
        private static void ProcessTask(ILogger logger, ITask task)
        {
            using (ILogger taskLogger = new TaskLoggerWrapper(logger, task.TaskName))
            {
                try
                {
                    task.Run(taskLogger);
                }
                catch (Exception e)
                {
                    taskLogger.LogError(e.ToString());
                }
            }
        }

        public static void ProcessTasks(ILogger logger, ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                ProcessTask(logger, task);
            }
        }
    }
}
