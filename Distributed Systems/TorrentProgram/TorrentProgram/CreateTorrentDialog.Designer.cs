namespace TorrentProgram
{
    partial class CreateTorrentDialog
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonYes = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxShowFile = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(89, 60);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(215, 13);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Are you sure you want to create this torrent?";
            // 
            // buttonYes
            // 
            this.buttonYes.Location = new System.Drawing.Point(101, 109);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(75, 23);
            this.buttonYes.TabIndex = 1;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(211, 109);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxShowFile
            // 
            this.checkBoxShowFile.AutoSize = true;
            this.checkBoxShowFile.Location = new System.Drawing.Point(125, 156);
            this.checkBoxShowFile.Name = "checkBoxShowFile";
            this.checkBoxShowFile.Size = new System.Drawing.Size(190, 17);
            this.checkBoxShowFile.TabIndex = 3;
            this.checkBoxShowFile.Text = "Show Torrent File when complete?";
            this.checkBoxShowFile.UseVisualStyleBackColor = true;
            // 
            // CreateTorrentDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 207);
            this.Controls.Add(this.checkBoxShowFile);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonYes);
            this.Controls.Add(this.labelMessage);
            this.Name = "CreateTorrentDialog";
            this.Text = "CreateTorrentDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxShowFile;
    }
}