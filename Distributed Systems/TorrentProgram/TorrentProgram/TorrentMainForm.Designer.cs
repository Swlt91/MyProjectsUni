namespace TorrentProgram
{
    partial class TorrentMainForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonFileSelect = new System.Windows.Forms.Button();
            this.buttonCreateTorrent = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Peers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonRequest = new System.Windows.Forms.Button();
            this.dataGridViewLatestTorrent = new System.Windows.Forms.DataGridView();
            this.buttonRefreshTorrent = new System.Windows.Forms.Button();
            this.dataGridViewConnectedPeers = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uploaded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonShowInFolder = new System.Windows.Forms.Button();
            this.NameCell = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonDownloadFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLatestTorrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConnectedPeers)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonFileSelect
            // 
            this.buttonFileSelect.Location = new System.Drawing.Point(487, 311);
            this.buttonFileSelect.Name = "buttonFileSelect";
            this.buttonFileSelect.Size = new System.Drawing.Size(89, 22);
            this.buttonFileSelect.TabIndex = 0;
            this.buttonFileSelect.Text = "Open Torrent";
            this.buttonFileSelect.UseVisualStyleBackColor = true;
            this.buttonFileSelect.Click += new System.EventHandler(this.buttonFileSelect_Click);
            // 
            // buttonCreateTorrent
            // 
            this.buttonCreateTorrent.Location = new System.Drawing.Point(487, 353);
            this.buttonCreateTorrent.Name = "buttonCreateTorrent";
            this.buttonCreateTorrent.Size = new System.Drawing.Size(89, 24);
            this.buttonCreateTorrent.TabIndex = 4;
            this.buttonCreateTorrent.Text = "Create Torrent";
            this.buttonCreateTorrent.UseVisualStyleBackColor = true;
            this.buttonCreateTorrent.Click += new System.EventHandler(this.buttonCreateTorrent_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Peers,
            this.Column4});
            this.dataGridView1.Location = new System.Drawing.Point(12, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(564, 250);
            this.dataGridView1.TabIndex = 5;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Size";
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Percentage";
            this.Column3.Name = "Column3";
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Peers
            // 
            this.Peers.HeaderText = "Peers";
            this.Peers.Name = "Peers";
            this.Peers.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column4.HeaderText = "Status";
            this.Column4.Name = "Column4";
            this.Column4.Width = 62;
            // 
            // buttonRequest
            // 
            this.buttonRequest.Location = new System.Drawing.Point(232, 12);
            this.buttonRequest.Name = "buttonRequest";
            this.buttonRequest.Size = new System.Drawing.Size(95, 22);
            this.buttonRequest.TabIndex = 6;
            this.buttonRequest.Text = "Refresh Tracker";
            this.buttonRequest.UseVisualStyleBackColor = true;
            this.buttonRequest.Click += new System.EventHandler(this.buttonRequest_Click);
            // 
            // dataGridViewLatestTorrent
            // 
            this.dataGridViewLatestTorrent.AllowUserToResizeRows = false;
            this.dataGridViewLatestTorrent.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridViewLatestTorrent.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewLatestTorrent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLatestTorrent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCell});
            this.dataGridViewLatestTorrent.Location = new System.Drawing.Point(601, 42);
            this.dataGridViewLatestTorrent.Name = "dataGridViewLatestTorrent";
            this.dataGridViewLatestTorrent.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewLatestTorrent.RowHeadersVisible = false;
            this.dataGridViewLatestTorrent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewLatestTorrent.Size = new System.Drawing.Size(187, 250);
            this.dataGridViewLatestTorrent.TabIndex = 7;
            this.dataGridViewLatestTorrent.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewLatestTorrent_CellMouseDown);
            // 
            // buttonRefreshTorrent
            // 
            this.buttonRefreshTorrent.Location = new System.Drawing.Point(601, 302);
            this.buttonRefreshTorrent.Name = "buttonRefreshTorrent";
            this.buttonRefreshTorrent.Size = new System.Drawing.Size(89, 31);
            this.buttonRefreshTorrent.TabIndex = 8;
            this.buttonRefreshTorrent.Text = "Refresh List";
            this.buttonRefreshTorrent.UseVisualStyleBackColor = true;
            this.buttonRefreshTorrent.Click += new System.EventHandler(this.buttonRefreshTorrent_Click);
            // 
            // dataGridViewConnectedPeers
            // 
            this.dataGridViewConnectedPeers.AllowUserToResizeRows = false;
            this.dataGridViewConnectedPeers.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridViewConnectedPeers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewConnectedPeers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConnectedPeers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Uploaded,
            this.Column5});
            this.dataGridViewConnectedPeers.Location = new System.Drawing.Point(12, 311);
            this.dataGridViewConnectedPeers.Name = "dataGridViewConnectedPeers";
            this.dataGridViewConnectedPeers.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewConnectedPeers.RowHeadersVisible = false;
            this.dataGridViewConnectedPeers.Size = new System.Drawing.Size(469, 127);
            this.dataGridViewConnectedPeers.TabIndex = 9;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "IP";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Downloaded";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Uploaded
            // 
            this.Uploaded.HeaderText = "Uploaded";
            this.Uploaded.Name = "Uploaded";
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Status";
            this.Column5.Name = "Column5";
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(117, 12);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(89, 22);
            this.buttonPause.TabIndex = 10;
            this.buttonPause.Text = "Pause Torrent";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonShowInFolder
            // 
            this.buttonShowInFolder.Location = new System.Drawing.Point(12, 12);
            this.buttonShowInFolder.Name = "buttonShowInFolder";
            this.buttonShowInFolder.Size = new System.Drawing.Size(89, 22);
            this.buttonShowInFolder.TabIndex = 11;
            this.buttonShowInFolder.Text = "Show In Folder";
            this.buttonShowInFolder.UseVisualStyleBackColor = true;
            this.buttonShowInFolder.Click += new System.EventHandler(this.buttonShowInFolder_Click);
            // 
            // NameCell
            // 
            this.NameCell.HeaderText = "Name";
            this.NameCell.Name = "NameCell";
            this.NameCell.Width = 185;
            // 
            // buttonDownloadFile
            // 
            this.buttonDownloadFile.Location = new System.Drawing.Point(696, 302);
            this.buttonDownloadFile.Name = "buttonDownloadFile";
            this.buttonDownloadFile.Size = new System.Drawing.Size(89, 31);
            this.buttonDownloadFile.TabIndex = 12;
            this.buttonDownloadFile.Text = "Download File";
            this.buttonDownloadFile.UseVisualStyleBackColor = true;
            this.buttonDownloadFile.Click += new System.EventHandler(this.buttonDownloadFile_Click);
            // 
            // TorrentMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonDownloadFile);
            this.Controls.Add(this.buttonShowInFolder);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.dataGridViewConnectedPeers);
            this.Controls.Add(this.buttonRefreshTorrent);
            this.Controls.Add(this.dataGridViewLatestTorrent);
            this.Controls.Add(this.buttonRequest);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonCreateTorrent);
            this.Controls.Add(this.buttonFileSelect);
            this.Name = "TorrentMainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLatestTorrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConnectedPeers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonFileSelect;
        private System.Windows.Forms.Button buttonCreateTorrent;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonRequest;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Peers;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridView dataGridViewLatestTorrent;
        private System.Windows.Forms.Button buttonRefreshTorrent;
        private System.Windows.Forms.DataGridView dataGridViewConnectedPeers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Uploaded;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonShowInFolder;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCell;
        private System.Windows.Forms.Button buttonDownloadFile;
    }
}

