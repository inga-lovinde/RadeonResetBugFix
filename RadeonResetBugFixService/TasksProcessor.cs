namespace RadeonResetBugFixService
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Logging;
    using Tasks;

    static class TasksProcessor
    {
        public static void ProcessTask(ILogger logger, ITask task)
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

        public static void ProcessTasks(ILogger logger, IEnumerable<ITask> tasks)
        {
            foreach (var task in tasks)
            {
                ProcessTask(logger, task);
            }
        }
    }
}
