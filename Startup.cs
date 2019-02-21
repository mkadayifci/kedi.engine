using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(kedi.engine.Startup))]
namespace kedi.engine
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app
            .UseCLRRuntimeEngineLocker()
            .UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll)
            .MapSignalR()
            .UseWebApi(GetWebApiConfiguration())
            .UseFileServer(new FileServerOptions()
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem(@".\webapp")
            });


        }

        private static HttpConfiguration GetWebApiConfiguration()
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                          name: "DefaultApi",
                          routeTemplate: "api/{controller}/{id}",
                          defaults: new { id = RouteParameter.Optional }
                      );

            config.Filters.Add(new GeneralExceptionFilter());
            return config;
        }
    }


}