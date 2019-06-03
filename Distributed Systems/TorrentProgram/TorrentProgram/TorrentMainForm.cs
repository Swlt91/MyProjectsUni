using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorrentProgram
{
    public partial class TorrentMainForm : Form
    {
        TorrentFile torrentFile;
        UIUpdater uIUpdater;
        private ConnManager manager;
        string id;
        string fileToDownload;
        List<UIPeer> peerConnectedList;
        // Initialise the program to a known state
        // defaults to a client

        public TorrentMainForm()
        {
            InitializeComponent();
            this.manager = new ConnManager(11000, false);

            // Open the Id form to gather an id from the user
            using (IdForm idForm = new IdForm())
            {
                idForm.StartPosition = FormStartPosition.CenterParent;
                idForm.ShowDialog(this);

                id = idForm.GetID();            
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                this.Close();
            }
            peerConnectedList = new List<UIPeer>();
            uIUpdater = new UIUpdater(dataGridView1, dataGridViewConnectedPeers, dataGridViewLatestTorrent);
        }

        private void buttonFileSelect_Click(object sender, EventArgs e)
        {
            string path;

            // Open a file dialog to select a torrent file
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;

                // Create a new instance of a torrentfile
                torrentFile = new TorrentFile(id, manager.ip, manager.port, uIUpdater);

                // If the file is read
                if (torrentFile.Read(path))
                {
                    // Start the listener if it is not already
                    if (!manager.connected)
                    {
                        startServerMode();
                    }

                    else
                    {
                        // Check to make sure this torrent file is not already in use in the program
                        if (!manager.CheckTorrentFileDownloading(torrentFile))
                        {
                            // If not add to the list and verify what parts of the file are downloaded
                            manager.torrentFiles.Add(torrentFile);
                            VerifyFile(torrentFile);
                        }

                        else
                        {
                            MessageBox.Show("File is already downloading");
                        }
                    }
                }
            }
        }


        private void startServerMode()
        {
            // debugging if you need to find out the IP address the server is connected to
            System.Net.IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine(ipAddress);
            Console.WriteLine(ipAddress.MapToIPv4());

            // start listener
            manager.startListener();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the form is closing, send a request to the tracker to remove this peer
            foreach (TorrentFile torrent in manager.torrentFiles)
            {
                torrent.SendStopRequestAll();
            }
            Environment.Exit(0);
        }

        private void buttonCreateTorrent_Click(object sender, EventArgs e)
        {
            string path;

            // Open the file dialog
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                // Get the path
                path = file.FileName;

                // Create and show the create torrent dialog
                CreateTorrentDialog createTorrentDialog = new CreateTorrentDialog(path);
                createTorrentDialog.ShowDialog();

                // If yes is clicked
                if (createTorrentDialog.DialogResult == DialogResult.Yes)
                {
                    // Variable for the user whether they want to show the torrent file on completion
                    bool showFile = createTorrentDialog.ShowFile();
                    createTorrentDialog.Close();

                    // Create and show the loading form
                    LoadingForm loadingForm = new LoadingForm(showFile);
                    loadingForm.StartPosition = FormStartPosition.CenterParent;
                    loadingForm.id = id;
                    loadingForm.path = path;
                    loadingForm.ShowDialog(this);

                    // If the loading form completed as normal
                    if (loadingForm.DialogResult == DialogResult.OK)
                    {
                        // Close the form and create a new torrentfile instance
                        string filePath = loadingForm.createdFilePath;
                        loadingForm.Close();
                        torrentFile = new TorrentFile(id, manager.ip, manager.port, uIUpdater);

                        // Open the torrent file and add to the list
                        if (torrentFile.Read(filePath))
                        {
                            manager.torrentFiles.Add(torrentFile);
                            VerifyFile(torrentFile);
                        }
                    }

                    // else the process failed
                    else
                    {
                        loadingForm.Close();
                        MessageBox.Show("Failed to create torrent file");
                    }
                }
            }
        }

        public void SendRequest(TorrentFile inTorrentFile)
        {
            // If the torrent file is not complete, send a request at a more regular interval
            if (!inTorrentFile.completed)
            {
                // Set up and send the request, connect to peers if any are found
                if (inTorrentFile.SetUpRequest())
                {
                    manager.ConnectPeers(inTorrentFile);
                }

                // Create a timer
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

                // Set the interval to 30 seconds
                t.Interval = 30000; // specify interval time as you want
                t.Tick += new EventHandler(timer_Tick);
                t.Start();

                // Send a new request after 30 seconds
                void timer_Tick(object sender, EventArgs e)
                {
                    SendRequest(inTorrentFile);
                }
            }

            // else the file is complete
            else
            {
                // Set up and send the request
                if (inTorrentFile.SetUpRequest())
                {
                    manager.ConnectPeers(inTorrentFile);
                }

                // Create a timer and set it's time to 5 minutes
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                t.Interval = 300000; // specify interval time as you want
                t.Tick += new EventHandler(timer_Tick);
                t.Start();

                // Send a request after 5 minutes
                void timer_Tick(object sender, EventArgs e)
                {
                    SendRequest(inTorrentFile);
                }
            }


        }
        public void CheckFiles()
        {
            startServerMode();

            VerifyTorrentDialog verifyTorrent = new VerifyTorrentDialog();
           

            while (!manager.connected)
            {

            }

            // Set the path to find torrent files within the ID directory of this user
            string path = System.IO.Directory.GetCurrentDirectory();
            path = path + "\\" + id + "\\TorrentFiles";

            // If the directory exists, then check for files
            if (Directory.Exists(path))
            {
                // For each file in the directory

                verifyTorrent.StartPosition = FormStartPosition.CenterParent;
                verifyTorrent.Show(this);               

                foreach (string file in Directory.EnumerateFiles(path, "*.txt"))
                {
                    verifyTorrent.Refresh();
                    // Create a torrentFile instance
                    TorrentFile torrentFile21 = new TorrentFile(id, manager.ip, manager.port, uIUpdater);
                    verifyTorrent.SetFile(torrentFile21.fileName);
                    // read the file into the instance
                    if (torrentFile21.Read(file))
                    {
                        // Add to the torrentfile list and check what parts of the file have been downloaded
                        manager.torrentFiles.Add(torrentFile21);
                        VerifyFile(torrentFile21);

                    }
                }
                
                verifyTorrent.Close();
            }
        }

        public void VerifyFile(TorrentFile torrent)
        {
            // Check what parts of the file have been downloaded and then send a request
            torrent.Verify();
            SendRequest(torrent);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Check what torrent files were previously active
            CheckFiles();
        }

        private void buttonRequest_Click(object sender, EventArgs e)
        {

            foreach (TorrentFile torr in manager.torrentFiles)
            {
                if (torr.SetUpRequest())
                {
                    manager.ConnectPeers(torr);
                }
            }
        }

        private void buttonRefreshTorrent_Click(object sender, EventArgs e)
        {
            // Re populate the torrent list
            uIUpdater.PopulateLatestTorrentList();
        }      

        public void DownloadTorrentFile(Object sender, EventArgs e)
        {
            // Create new torrent file
            torrentFile = new TorrentFile(id, manager.ip, manager.port, uIUpdater);

            // Attempt to download the file
            if (torrentFile.DownloadFile(fileToDownload))
            {
                // If succcessful, add this torrent to the list
                manager.torrentFiles.Add(torrentFile);
             
                // If the listener is not running, run it
                if (!manager.connected)
                {
                    startServerMode();
                }

                else
                {
                    // Check what parts of the file have been acquired
                    VerifyFile(torrentFile);
                }
            }
           
        }

        private void dataGridViewLatestTorrent_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // If the left or right button of the mouse is clicked
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {

                // find which row has been selected
                int selectedrowindex = dataGridViewLatestTorrent.SelectedCells[0].RowIndex;

                // Set the fileTodownload to the value in the data grid
                fileToDownload = dataGridViewLatestTorrent.Rows[selectedrowindex].Cells[0].Value.ToString();
                fileToDownload = fileToDownload.Substring(1);

                // Open a context menu to give the option of downloading
                ContextMenu cm = new ContextMenu();
                cm.MenuItems.Add(new MenuItem("Download",DownloadTorrentFile));

                dataGridViewLatestTorrent.ContextMenu = cm;
                    dataGridViewLatestTorrent.ContextMenu.Show(dataGridViewLatestTorrent, new Point(e.RowIndex, e.ColumnIndex));
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int listId = dataGridView1.CurrentCell.RowIndex;
                manager.PauseTorrent(listId);
            }
        }

        private void buttonShowInFolder_Click(object sender, EventArgs e)
        {
            // Find the selected row id from the datagrid
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Once found, go into the manager and iterate through the torrent list
                int listId = dataGridView1.CurrentCell.RowIndex;
                manager.ShowTorrentInFolder(listId);
            }
        }

        private void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            if(dataGridViewLatestTorrent.SelectedRows.Count > 0)
            {
                int selectedrowindex = dataGridViewLatestTorrent.SelectedCells[0].RowIndex;

                // Set the fileTodownload to the value in the data grid
                fileToDownload = dataGridViewLatestTorrent.Rows[selectedrowindex].Cells[0].Value.ToString();
                fileToDownload = fileToDownload.Substring(1);

                DownloadTorrentFile(null, null);
            }
        }
    }
}
