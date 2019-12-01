using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RadeonResetBugFixService
{
    public partial class RadeonResetBugFixService : ServiceBase
    {
        private MainHandler Handler { get; } = new MainHandler();

        public RadeonResetBugFixService()
        {
            InitializeComponent();
        }

        protected override void OnShutdown()
        {
            SystemEvents.SessionEnding -= this.OnSessionEnding;
            this.RequestAdditionalTime(300000);
            this.Handler.HandleShutdown("ServiceBase.OnShutdown");
        }

        protected override void OnStart(string[] args)
        {
            this.Handler.HandleStartup("ServiceBase.OnStart");
            this.RequestAdditionalTime(300000);
            SystemEvents.SessionEnding += this.OnSessionEnding;
        }

        protected override void OnStop()
        {
            SystemEvents.SessionEnding -= this.OnSessionEnding;
            this.RequestAdditionalTime(300000);
            this.Handler.HandleShutdown("ServiceBase.OnStop");
        }

        private void OnSessionEnding(object sender, SessionEndingEventArgs args)
        {
            if (args.Reason == SessionEndReasons.SystemShutdown)
            {
                this.Handler.HandleShutdown("SystemEvents.SessionEnding");
            }
        }
    }
}
