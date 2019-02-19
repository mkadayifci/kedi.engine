using Castle.MicroKernel.Registration;
using Castle.Windsor;
using kedi.engine.Services.Analyze;
using kedi.engine.Services.Sessions;
using Serilog;
using Exceptionless;

using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine
{
    public static class ContainerManager
    {
        private static IWindsorContainer container = null;
        public static void Register()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ISessionManager>().ImplementedBy<SessionManager>().LifestyleSingleton());
            container.Register(Component.For<IAnalyzeOrchestrator>().ImplementedBy<AnalyzeOrchestrator>().LifestyleSingleton());

            var logger = new LoggerConfiguration().WriteTo.Exceptionless("80I0edYzAusnEtY57jVIMf1lfYee7ZmvSG2kmAgn").CreateLogger();
            container.Register(Component.For<ILogger>().Instance(logger));



            ContainerManager.container = container;
        }
        public static IWindsorContainer Container
        {
            get
            {
                if (ContainerManager.container == null)
                {
                    Register();
               
                }
                return ContainerManager.container;
            }

        }
    }
}
