using kedi.engine.Services.Sessions;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace kedi.engine
{
    public class GeneralExceptionFilter : ExceptionFilterAttribute

    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Exception currentException = actionExecutedContext.Exception;
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
