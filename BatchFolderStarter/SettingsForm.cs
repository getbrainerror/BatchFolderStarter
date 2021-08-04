using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchFolderStarter
{
    public partial class SettingsForm : Form
    {
        const string RK_NAME = "BatchFolderStarter";
        RegistryKey rkStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            

            if (rkStartup.GetValue(RK_NAME) == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                checkBoxStartup.Checked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                checkBoxStartup.Checked = true;
            }
        }

        private void checkBoxStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxStartup.Checked)
            {
                rkStartup.SetValue(RK_NAME, Application.ExecutablePath);

            } else
            {
                rkStartup.SetValue(RK_NAME, false);

            }
        }

        private void buttonSelectBatchFolder_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            if(folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBoxBatchFolder.Text = folderBrowser.SelectedPath;
            }
        }
    }
}
