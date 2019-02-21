using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;

namespace kedi.engine
{
    public class CLRRuntimeEngineLocker : OwinMiddleware
    {
        public static object clrRuntimeLockObject = new object();

        public CLRRuntimeEngineLocker(OwinMiddleware next) :
            base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);
        }
    }

    public static class CLRRuntimeEngineLockerExtensions
    {
        public static IAppBuilder UseCLRRuntimeEngineLocker(this IAppBuilder appBuilder)
        {
            return appBuilder.Use<CLRRuntimeEngineLocker>();
        }
    }
}
