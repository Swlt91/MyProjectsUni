using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorrentProgram
{
    public class UIUpdater
    {
        DataGridView dataGridTorrentDownloadingList;
        DataGridView dataGridConnectedPeers;
        DataGridView dataGridTrackerTorrentList;
        int listViewID;
        List<UIPeer> peerConnectedList;


        public UIUpdater(DataGridView inDownloadList, DataGridView inConnectedPeer, DataGridView inTrackerTorrentList)
        {
            listViewID = 0;
            dataGridTorrentDownloadingList = inDownloadList;
            dataGridConnectedPeers = inConnectedPeer;
            dataGridTrackerTorrentList = inTrackerTorrentList;
            peerConnectedList = new List<UIPeer>();
        }

        public void UpdateGridView(int id, string percentage, int peersConnected, string status)
        {
            // Update the torrent grid view
            dataGridTorrentDownloadingList.Rows[id].Cells[2].Value = percentage;
            dataGridTorrentDownloadingList.Rows[id].Cells[3].Value = peersConnected;
            dataGridTorrentDownloadingList.Rows[id].Cells[4].Value = status;
        }


        public void AddToDataGrid(TorrentFile inTorrent)
        {
            // Add a torrent file to the grid view
            // Set the torrent listview ID to the current listViewID count
            inTorrent.listViewID = listViewID;
            dataGridTorrentDownloadingList.Rows.Add(inTorrent.fileName, inTorrent.fileSizeUI, "Verifying");
            listViewID++;
        }


        public void UpdatePeerList(string inIp, int inDownloaded, int inUploaded, bool inUpdate, bool delete)
        {
            DataGridView.CheckForIllegalCrossThreadCalls = false;

            // If the incoming data is not an update
            if (!inUpdate)
            {
                bool found = false;

                // Check to see if this peer has previously connected
                // If so there is no need to add this address to the list
                foreach (UIPeer peer in peerConnectedList)
                {
                    if (peer.IpAddress.Equals(inIp))
                    {
                        found = true;
                    }
                }

                // If the address was not found, then add a new entry to the list
                if (!found)
                {
                    peerConnectedList.Add(new UIPeer(inIp, peerConnectedList.Count));

                    //Update the grid view
                    dataGridConnectedPeers.Invoke(new MethodInvoker(() =>
                    {
                        dataGridConnectedPeers.Rows.Add(inIp, 0, 0, "");
                    }));
                }
            }

            // Else it is an update
            else
            {
                // Find the peer by ipaddress
                foreach (UIPeer peer in peerConnectedList)
                {
                    if (peer.IpAddress.Contains(inIp))
                    {
                        // Once found update the values and update the gridview
                        peer.downloaded += inDownloaded;
                        peer.uploaded += inUploaded;
                        peer.UIUpdate();
                        dataGridConnectedPeers.Invoke(new MethodInvoker(() =>
                        {
                            dataGridConnectedPeers.Rows[peer.id].Cells[1].Value = peer.uiDownloaded;
                            dataGridConnectedPeers.Rows[peer.id].Cells[2].Value = peer.uiUploaded;
                        }));
                        break;
                    }
                }
            }
        }

        public void PopulateLatestTorrentList()
        {
            try
            {
                // Clear the torrent file list
                dataGridTrackerTorrentList.Rows.Clear();

                // Prepare a request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://seanthomas1991.000webhostapp.com/uploads");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // Get the directory listings from the server, in the uploads page
                        string html = reader.ReadToEnd();
                        Regex regex = new Regex(GetDirectoryListingRegexForUrl("https://seanthomas1991.000webhostapp.com/uploads"));
                        MatchCollection matches = regex.Matches(html);

                        int count = 0;

                        // If entries were found
                        if (matches.Count > 0)
                        {
                            List<string> fileList = new List<string>();

                            // For each entry found, add it to a string list with the name
                            foreach (Match match in matches)
                            {
                                if (match.Success && count > 0)
                                {
                                    string fileName = match.Groups["name"].ToString();
                                    fileList.Add(fileName);
                                }

                                count++;
                            }

                            // Reverse the list to show the latest torrent at the top
                            fileList.Reverse();

                            // Iterate through the string list and add each entry to the data grid torrent file List
                            foreach (string file in fileList)
                            {
                                dataGridTrackerTorrentList.BeginInvoke(new MethodInvoker(() =>
                                {
                                    dataGridTrackerTorrentList.Rows.Add(file);
                                }));
                            }
                        }
                    }
                }
            }

            catch
            {

            }
        }

        // Returns directory listing from the server
        public static string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals("https://seanthomas1991.000webhostapp.com/uploads"))
            {
                return "<a href=\".*\">(?<name>.*)</a>";
            }
            throw new NotSupportedException();
        }

    }
}
