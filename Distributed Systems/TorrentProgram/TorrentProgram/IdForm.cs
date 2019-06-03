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
    public partial class IdForm : Form
    {
        public IdForm()
        {
            InitializeComponent();
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBoxId.Text))
            {
                this.Close();
            }

            else
            {
                labelWarning.Visible = true;
            }
        }

        public string GetID()
        {
            return textBoxId.Text;
        }

        private void IdForm_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
