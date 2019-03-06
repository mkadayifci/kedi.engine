using Serilog;
using System;
using System.Windows.Forms;

namespace kedi.engine
{
    public partial class StartForm : Form
    {
        ILogger logger = ContainerManager.Container.Resolve<ILogger>();
        string baseUrl = string.Empty;
        public StartForm(string baseUrl)
        {

            InitializeComponent();
            this.baseUrl = baseUrl;
        }

        private void openWebApp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"{this.baseUrl}");
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            //this.Text = Properties.LocalSettings.Default["UserToken"].ToString();
            //Properties.LocalSettings.Default["UserToken"] = "Saved";
            //Properties.LocalSettings.Default.Save();

            logger.Information("Application Started");
            infoLabel.Text = $"kedi runs in web browser window.You can manually browse {this.baseUrl} or just click the button below";
        }
    }
}
