using Castle.MicroKernel.Registration;
using Castle.Windsor;
using kedi.engine.Services.Analyze;
using kedi.engine.Services.Sessions;
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
