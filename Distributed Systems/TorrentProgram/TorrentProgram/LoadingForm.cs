using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorrentProgram
{
    public partial class LoadingForm : Form
    {
        public string id;
        public string path;
        public string createdFilePath;
        public bool showFile;

        public LoadingForm(bool inshowFile)
        {
            InitializeComponent();
            showFile = inshowFile;
            progressBar1.Maximum = 200;
        }


        public void UpdateForm(string message, int progress)
        {
            // Update the form
            labelStatus.Invoke(new MethodInvoker(() =>
            {
                labelStatus.Text = message;
            }));
            progressBar1.Invoke(new MethodInvoker(() =>
            {
                progressBar1.Value += progress;
            }
            ));
        }

        public void CreateTorrent()
        {
            // Create torrentcreator
            TorrentCreator torrentCreate = new TorrentCreator(id, this, showFile);

            // If the torrent creator was successful, return dialog result OK
            if (torrentCreate.CreateTorrentFile(path))
            {
                createdFilePath = torrentCreate.createdFilePath;
                this.DialogResult = DialogResult.OK;
            }

            // else cancel
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        public string CreateFilePath()
        {
            return createdFilePath;
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            // Run method to create the torrent file
           Task.Run(()=> CreateTorrent());
        }
    }
}
