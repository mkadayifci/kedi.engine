namespace kedi.engine
{
    partial class StartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.infoLabel = new System.Windows.Forms.Label();
            this.openWebApp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(12, 23);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(571, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "kedi-anlayzer runs in web browser window. You can manualy browse http://localhost" +
    ":8000 or just click the button nelow";
            // 
            // openWebApp
            // 
            this.openWebApp.Location = new System.Drawing.Point(50, 62);
            this.openWebApp.Name = "openWebApp";
            this.openWebApp.Size = new System.Drawing.Size(472, 100);
            this.openWebApp.TabIndex = 1;
            this.openWebApp.Text = "Open Kedi App";
            this.openWebApp.UseVisualStyleBackColor = true;
            this.openWebApp.Click += new System.EventHandler(this.openWebApp_Click);
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 190);
            this.Controls.Add(this.openWebApp);
            this.Controls.Add(this.infoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "kedi-analyzer Host";
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button openWebApp;
    }
}