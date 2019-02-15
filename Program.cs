using Microsoft.Owin.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace kedi.engine
{
    class Program
    {
        const string url = "http://localhost:5334";
        static void Main(string[] args)
        {

            RegisterDI();
            
            using (WebApp.Start<Startup>(url))
            {
                Application.Run(new StartForm(url));
            }
        }

        static void RegisterDI()
        {
            ContainerManager.Register();
        }
    }
}
