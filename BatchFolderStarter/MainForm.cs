using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchFolderStarter
{
    public partial class MainForm : Form
    {


        public MainForm()
        {
            InitializeComponent();
            Hide();
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void toolStripMenuItemReload_Click(object sender, EventArgs e)
        {
            CreateContextMenu();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            CreateContextMenu();

        }
        private void ShowSettings()
        {
            var settings = new SettingsForm();
            settings.ShowDialog();
            CreateContextMenu();
        }

        private void CreateContextMenu()
        {

            contextMenuStripNotify.Items.Clear();
            contextMenuStripNotify.Items.Add("Settings", null, toolStripMenuItemSettings_Click);
            contextMenuStripNotify.Items.Add("Reload", null, toolStripMenuItemReload_Click);
            contextMenuStripNotify.Items.Add("Close", null, toolStripMenuItemClose_Click);

            if (String.IsNullOrEmpty(Properties.Settings.Default.SelectedFolder) || !Directory.Exists(Properties.Settings.Default.SelectedFolder))
            {
                return;
            }

            contextMenuStripNotify.Items.Add("-");
            contextMenuStripNotify.Items.AddRange(GetDirectoryMenuItems(Properties.Settings.Default.SelectedFolder, Properties.Settings.Default.IncludeSubfolders));

        }


        private void toolStripMenuItemFile_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var filePath = menuItem.Tag.ToString();
            var process = new Process();
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
            process.StartInfo.FileName = filePath;
            process.Start();
        }

        private ToolStripMenuItem[] GetDirectoryMenuItems(string path, bool subfolder, int n = 0, int depth = 5)
        {
            var toolStripMenuItems = new List<ToolStripMenuItem>();
            if(n < depth && subfolder)
            {
                //Max depth not reached 
                foreach (var item in Directory.GetDirectories(path))
                {
                    var toolStripItem = new ToolStripMenuItem();
                    toolStripItem.Text = Path.GetFileName(item);
                    toolStripItem.Image = Utils.GetIcon(item).ToBitmap();
                    toolStripItem.Tag = item;
                    toolStripItem.DropDownItems.AddRange(GetDirectoryMenuItems(item, subfolder));
                    toolStripMenuItems.Add(toolStripItem);

                }

            }
         
            foreach (var item in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(item);
                if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    continue;
                }
                if(Path.GetExtension(item) != ".bat")
                {
                    continue;
                }
                string iconPath = Path.GetDirectoryName(item) + Path.DirectorySeparatorChar + "." + Path.GetFileNameWithoutExtension(item) + ".ico";

                var toolStripItem = new ToolStripMenuItem();
                toolStripItem.Text = Path.GetFileName(item);

                //Try to get .filename.ico
                if (File.Exists(iconPath))
                {
                    toolStripItem.Image = new Icon(iconPath, new Size(16, 16)).ToBitmap();

                   
                } else
                {
                    toolStripItem.Image = Utils.GetIcon(item).ToBitmap();
                }
                toolStripItem.Click += toolStripMenuItemFile_Click;
                toolStripItem.Tag = item;
                toolStripMenuItems.Add(toolStripItem);


            }
            return toolStripMenuItems.ToArray();
        }


        protected override CreateParams CreateParams
        {
            get
            {
                // Turn on WS_EX_TOOLWINDOW style bit
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
    }
}
