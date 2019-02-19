using kedi.engine.Services.Sessions;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace kedi.engine
{
    public class GeneralExceptionFilter : ExceptionFilterAttribute

    {
        ILogger logger = ContainerManager.Container.Resolve<ILogger>();

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            
            Exception currentException = actionExecutedContext.Exception;
            logger.Error(currentException, string.Empty);
            logger.Warning("This is first name: {name} and this is last : {last} ", "Mehmet", "Kadayıfçı");
            Type exceptionType = currentException.GetType();

            HttpStatusCode status = HttpStatusCode.InternalServerError;
            String message = currentException.Message;
            int subCode = 0;

            if (exceptionType == typeof(SessionNotFoundException))
            {
                subCode = 100;
            }

            actionExecutedContext.Response = new HttpResponseMessage()
            {

                Content = new StringContent($"{{\"message\":\"{message}\",\"subCode\":{subCode.ToString()}}}", System.Text.Encoding.UTF8, "application/json"),
                StatusCode = status
            };

            base.OnException(actionExecutedContext);
        }
    }
}
