using System;
using System.Windows.Forms;

namespace kedi.engine
{
    public partial class StartForm : Form
    {
        string baseUrl = string.Empty;
        public StartForm(string baseUrl)
        {

            InitializeComponent();
            this.baseUrl = baseUrl;
        }

        private void openWebApp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"{this.baseUrl}/#/blocking-objects-analyzer/30230bf96a884830a0b96805cf173717");
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            infoLabel.Text = $"kedi - anlayzer runs in web browser window.You can manualy browse {this.baseUrl} or just click the button below";
        }
    }
}
