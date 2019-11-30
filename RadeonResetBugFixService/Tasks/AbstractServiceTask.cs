namespace RadeonResetBugFixService.Tasks
{
    using System;
    using System.ServiceProcess;
    using Contracts;

    abstract class AbstractServiceTask : ITask
    {
        protected virtual bool ShouldStart(ServiceController serviceInfo) => false;

        protected virtual bool ShouldStop(ServiceController serviceInfo) => false;

        public abstract string TaskName { get; }

        void ITask.Run(ILogger logger)
        {
            foreach (var originalService in ServiceController.GetServices())
            {
                string serviceDescription = $"{originalService.DisplayName} ({originalService.ServiceName})";

                if (this.ShouldStart(originalService))
                {
                    var service = new ServiceController(originalService.ServiceName);
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        logger.Log($"{serviceDescription} is already running");
                    }
                    else
                    {
                        if (service.Status != ServiceControllerStatus.StartPending)
                        {
                            logger.Log($"Starting service {serviceDescription}");
                            service.Start();
                            logger.Log($"Initiated service start for");
                        }

                        logger.Log($"Waiting for service {serviceDescription} to start");
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            logger.Log($"Service is running");
                        }
                        else
                        {
                            logger.Log($"Failed; service state is {service.Status}");
                        }
                    }
                }
                else if (this.ShouldStop(originalService))
                {
                    var service = new ServiceController(originalService.ServiceName);
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        logger.Log($"{serviceDescription} is already stopped");
                    }
                    else
                    {
                        if (service.Status != ServiceControllerStatus.StopPending)
                        {
                            logger.Log($"Stopping service {serviceDescription}");
                            service.Stop();
                            logger.Log($"Initiated service stop");
                        }

                        logger.Log($"Waiting for service {serviceDescription} to stop");
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            logger.Log($"Service is stopped");
                        }
                        else
                        {
                            logger.Log($"Failed; service state is {service.Status}");
                        }
                    }
                }
            }
        }
    }
}
