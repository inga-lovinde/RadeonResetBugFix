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
            this.Handler.HandleShutdown();
        }

        protected override void OnStart(string[] args)
        {
            this.Handler.HandleStartup();
        }

        protected override void OnStop()
        {
            this.Handler.HandleShutdown();
        }
    }
}
