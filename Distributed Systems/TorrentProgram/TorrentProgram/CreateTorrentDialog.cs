using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorrentProgram
{
    public partial class CreateTorrentDialog : Form
    {
        bool showFile;

        public CreateTorrentDialog(string fileName)
        {
            InitializeComponent();
            showFile = false;
            labelMessage.Text = "Are you sure you want to create this torrent?";
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            showFile = checkBoxShowFile.Checked;
            this.DialogResult = DialogResult.Yes;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool ShowFile()
        {
            return showFile;
        }
    }
}
