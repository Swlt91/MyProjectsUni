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
    public partial class VerifyTorrentDialog : Form
    {

        public VerifyTorrentDialog()
        {
            InitializeComponent();
        }

        private void VerifyTorrentDialog_Load(object sender, EventArgs e)
        {
            label1.Invalidate();
            Application.DoEvents();
            // this.Close();
        }

        public void SetFile(string inFile)
        {
            label1.BeginInvoke((MethodInvoker)(() =>
            label1.Text = inFile));
        }
    }
}
