using Microsoft.Owin.Hosting;
using System.Windows.Forms;

namespace kedi.engine
{
    public class Program
    {
        public const string APP_URL = "http://localhost:5334";

        static void Main(string[] args)
        {
            RegisterDI();

            using (WebApp.Start<Startup>(Program.APP_URL))
            {
                Application.Run(new StartForm(Program.APP_URL));
            }
        }

        static void RegisterDI()
        {
            ContainerManager.Register();
        }
    }
}
