using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TorrentProgram
{
   public class TorrentFile
    {
        public string fileName;
        public string fileNameWithout;
        public byte[] hash;
        public string hashString;
        public string createdFilePath;
        string status;
        public long fileSize;
        public int pieces;
        public int pieceSize;
        public string tracker;
        public bool sentRequest;
        public bool paused;
        public int peersConnected = 0;
        public long amountOfFileAquired;
        public string id;
        public List<PeerResponse> peerList;
        public List<int> pieceHaveList;
        public List<int> pieceNeedList;
        public List<int> piecesDownloading;
        public bool completed = false;
        public string percentage;
        public string torrentFilePath;
        public int listViewID;
        public string parentPath;
        bool firstTime;
        int port;
        string[] hashArray;
        public string fileSizeUI;
        IPAddress ip;
        UIUpdater uiUpdate;


        public TorrentFile(string inId, IPAddress inIp, int inPort, UIUpdater inUiUpdate)
        {
            paused = false;
            uiUpdate = inUiUpdate;
            ip = inIp;
            port = inPort;
            percentage = "0";
            firstTime = true;
            id = inId;
            fileName = "";
            fileNameWithout = "";
            hash = null;
            hashString = "";
            fileSize = 0;
            pieces = 0;
            pieceSize = 0;
            amountOfFileAquired = 0;
            piecesDownloading = new List<int>();
            peerList = new List<PeerResponse>();
            parentPath = System.IO.Directory.GetCurrentDirectory();
            parentPath = parentPath + "\\" + id + "\\";
        }

        public bool Read(string path)
        {
            // Read the torrent file into the program
            // Create a list to store each file part
            List<string> fileParts = new List<string>();

            // Set the torrent file path to the incoming path variable
            torrentFilePath = path;

            try
            {
                // Read the file and add each line to the list
                using (var fileStream = File.OpenRead(path))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
                    {
                        String line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            fileParts.Add(line);
                        }
                    }
                }

                // Parse each line of the file into the appropriate variables
                fileName = fileParts[0]; 
                
                // Convert hash of file into a bytes
                hash = fileParts[1].Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
                hashString = BitConverter.ToString(hash);
                fileSize = long.Parse(fileParts[2]);

                // Convert the file size to a readable format
                fileSizeUI = Utility.SetSizeUI(fileSize);
                pieces = int.Parse(fileParts[3]);
                pieceSize = int.Parse(fileParts[4]);
                tracker = fileParts[5];
              
                // Create directories for the torrent file and content
                CreateFileDirectory(path);

                // Create an array of each hash entry
                hashArray = SpliceText(fileParts[6], 40);
              
                // Clear the list
                fileParts.Clear();

                // Add this file to the UI
                uiUpdate.AddToDataGrid(this);
              
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show("The file selected was not a recognised torrent file");
                return false;
            }
        }
        
        public bool Verify()
        {
            // Check how much of the file has been aqcuired
            CheckPiecesDownloaded(parentPath + fileName);
            
            CalculatePercentage();

            return true;
        }

       


        public string[] SpliceText(string text, int lineLength)
        {
            // Slice the incoming string by each 40 charavters and return in array form.
            return Regex.Matches(text, ".{1," + lineLength + "}").Cast<Match>().Select(m => m.Value).ToArray();
        }

        public void CheckPiecesDownloaded(string path)
        {
            // Create each list and
            pieceHaveList = new List<int>();
            pieceNeedList = new List<int>();
            piecesDownloading = new List<int>();
            amountOfFileAquired = 0;

            // For loop for each piece
            for (int i = 0; i < pieces; i++)
            {
                // Get the relevant piece by the for loop i counter
                string fileHashPiece = hashArray[i];

                try
                {
                    // Check if the piece already exists within the file
                    if (fileHashPiece.Contains(HashPiece(i, path)))
                    {
                        // The piece is acquired
                        pieceHaveList.Add(i);

                        // If the piece is the last in the list, get the correct size in case of a difference
                        if (i + 1 == pieces)
                        {
                            // If the piece amount is odd, then the last piece is a different size
                            if (pieces % 2 != 0)
                            {
                                amountOfFileAquired += fileSize % (pieces - 1);
                            }

                            // else use the piece size to add to the amount acquired
                            else
                            {
                                amountOfFileAquired += pieceSize;
                            }
                        }

                        else
                        {
                            amountOfFileAquired += pieceSize;
                        }
                    }

                    // Else the piece has not been downloaded, add this piece index to the need list
                    else
                    {
                        pieceNeedList.Add(i);
                    }
                }

                // If there was a problem in checking the piece, add it to the need list
                catch
                {
                    pieceNeedList.Add(i);
                }
            }

            // If the need list is empty, the file is complete
            if (pieceNeedList.Count < 1)
            {
                completed = true;
            }
            hashArray = null;
        }

        public void CreateFileDirectory(string path)
        {
            // Create a directory for the torrent files
            System.IO.Directory.CreateDirectory(parentPath + "TorrentFiles").ToString();

            // Check if the torrent file is within the torrent files directory, if not copy it in
            string destinationFile = System.IO.Path.Combine(parentPath + "TorrentFiles", Path.GetFileName(path));

            if (!File.Exists(destinationFile))
            {
                System.IO.File.Copy(path, destinationFile, true);
            }

            amountOfFileAquired = 0;

            // Alter the parent path to point to the path of the content being downloaded
            parentPath = parentPath + Path.GetFileNameWithoutExtension(fileName) + "\\";

            // Check to see if a directory for the content exists, if not create it
            if (!File.Exists(parentPath + fileName))
            {
                System.IO.Directory.CreateDirectory(parentPath);
            }
        }

        public string GetHashFromFile(int piece)
        {
            // Create file parts list
            List<string> fileParts = new List<string>();
     
            string line;
           
            // Read the last line of the torrent file
            using (var fileStream = File.OpenRead(torrentFilePath))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
                {
                    for(int i = 0; i < 6; i++)                  
                    {
                        streamReader.ReadLine();
                    }

                    line = streamReader.ReadLine();
                }
            }

            // If the piece is the first one, take the first 40 characters only
            if(piece == 0)
            {
                return line.Substring(0, 40);
            }

            // else calculate where the requested piece hash resides in the file
            return line.Substring(piece * 40, 40);
        }

        public void CheckSinglePiece(int piece, string inIp, int inSize)
        {
            // Checks to avoid the same piece being checked concurrently
            if (!pieceHaveList.Contains(piece) && piecesDownloading.Contains(piece) && pieceNeedList.Contains(piece))
            {
                // Hash the piece in the content file
                string pieceHash = HashPiece(piece, parentPath + fileName);

                // Get the hash from the torrent file
                string hashFromFile = GetHashFromFile(piece);

                // Further check to avoid same piece being checked
                if (hashFromFile.Contains(pieceHash) && piecesDownloading.Contains(piece) && pieceNeedList.Contains(piece))
                {
                    try
                    {
                        // if successful, remove the piece from the piece need list and add to have list
                        pieceNeedList.Remove(piece);
                        pieceHaveList.Add(piece);
                        CalculatePercentage();
                        piecesDownloading.Remove(piece);
                        UpdatePeerDataGrid(inIp, inSize, 0, true, false);
                    }
                    catch(Exception e)
                    {

                    }

                }
            }
        }

        public string HashPiece(int position, string path)
        {
            string sendCheckSum;

            // Create byte array for the piece
            byte[] pieceToHash = new byte[pieceSize];

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {         
                // If the position is above 0, work out the starting position
                if(position > 0)
                {
                    // Work this out by multiplying the position by the piece size
                    stream.Position = position * pieceSize;
                }

                // Gather the data
                stream.Read(pieceToHash, 0, pieceSize);

                // Hash the piece and return as a string
                using (SHA1Managed sha = new SHA1Managed())
                {
                    pieceToHash = sha.ComputeHash(pieceToHash);
                   
                   return sendCheckSum = BitConverter.ToString(pieceToHash).Replace("-", string.Empty);
                }
            }
        }

        public bool DownloadFile(string fileToDownload)
        {
            try
            {
                // Create the torrent file directory if it does not exist
                System.IO.Directory.CreateDirectory(parentPath + "TorrentFiles").ToString();
                string path = parentPath + "TorrentFiles\\";

                // Create a webclient and download the file requested
                WebClient Client = new WebClient();
                Client.DownloadFile("https://seanthomas1991.000webhostapp.com/uploads/" + Uri.EscapeUriString(fileToDownload), @path + fileToDownload);

                // Read the downloaded file and return its result
               return Read(path + fileToDownload);
             
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public void SendStopRequest()
        {
            // Removes the entry from the tracker related to this torrent
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tracker + "/user?removeSingle=yes&id=" + id + "&ip_address=" + Uri.EscapeDataString(ip.ToString())+ "&hash=" + hashString);
            request.ContentType = "application/xml";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string responseString;

            // Read the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
            }
        }

        public void SendStopRequestAll()
        {
            // Removes every peer entry in the tracker from this instance, using the ID
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tracker + "/user?remove=yes&id=" + id + "&ip_address=" + Uri.EscapeDataString(ip.ToString()));
            request.ContentType = "application/xml";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string responseString;

            // Read the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
            }
        }

        public bool SetUpRequest()
        {
            // Calculate how much of the file is needed
            long left = fileSize - amountOfFileAquired;

            // Set up the request string
            string request = "/user?hash=" + hashString + "&id=" + id + "&ip_address=" + Uri.EscapeDataString(ip.ToString()) + "&port=" + port.ToString() + "&left=" + left.ToString();

            // Send the request
            if(SendTrackerRequest(tracker + request))
            {
                return true;
            }

            return false;
        }

        public bool SendTrackerRequest(string uri)
        {
            // If the torrent is not paused
            if (!paused)
            {
                // Create a new list for any peers found
                peerList = new List<PeerResponse>();

                try
                {
                    // Prepare variables to send a request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.ContentType = "application/xml";
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    string responseString;

                    // Get the response
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        responseString = reader.ReadToEnd();
                    }

                    // Create an xml document to read the response
                    XDocument doc = XDocument.Parse(responseString);

                    // Create a list from all ip_address variables
                    var ipList = doc.Root.Elements("ip_address")
                                       .Select(element => element.Value)
                                       .ToList();
                    // Create a list from all port variables
                    var portList = doc.Root.Elements("port")
                                     .Select(element => element.Value)
                                     .ToList();

                    // Create a list from all amount_left variables
                    var amount_leftList = doc.Root.Elements("amount_left")
                                    .Select(element => element.Value)
                                    .ToList();

                    // Create a list from all id variables
                    var id_List = doc.Root.Elements("id")
                                    .Select(element => element.Value)
                                    .ToList();

                    // variable to index each list
                    int count = 0;

                    // If the torrent is not complete
                    if (pieceNeedList.Count > 0)
                    {
                        // Iterate through each entry on the list
                        foreach (string portString in portList)
                        {
                            // For same machine use, only connect to ports that are not the same
                            // For multi machine the statement would check ip addresses instead
                          
                            if (!portString.Contains(port.ToString()))
                            {
                                peerList.Add(new PeerResponse(ipList[count], int.Parse(portList[count]), int.Parse(amount_leftList[count])));
                            }                  
                            count++;
                        }
                    }
                  
                    // If there are peers, return true
                    if (peerList.Count > 0)
                    {
                        return true;
                    }

                    return false;
                }

                catch
                {
                    return false;
                }
            }
            return false;
        }

      
        public void CalculatePercentage()
        {
            // Get the previous percentage
            int previousPercent = int.Parse(percentage);
            long percentageCalculation;

            // If there are pieces still needed, calculate the percentage
            if (pieceNeedList.Count > 0)
            {
                percentageCalculation = (100 * amountOfFileAquired) / fileSize;
                
            }

            // Else the percentage is going to 100
            else
            {
                percentageCalculation = 100;

                completed = true;
            }
            percentage = percentageCalculation.ToString();

            // If there is a change in the percentage, update the UI
            if (percentageCalculation > previousPercent || firstTime)
            {
                uiUpdate.UpdateGridView(listViewID, percentage + "%", peersConnected, status);
                firstTime = false;
            }
        }

        public void UpdatePeer(int count)
        {
            // Update the number of peers connected
            peersConnected += count;
            status = "Seeding";
            
            // Update the UI appropriatly
            if (pieceNeedList.Count > 0)
            {
                status = "Downloading";
            }

            if (peersConnected == 0)
            {
                status = "";
            }

            if(paused)
            {
                status = "Paused";
            }
            uiUpdate.UpdateGridView(listViewID, percentage + "%", peersConnected, status);
        }


        public void UpdatePeerDataGrid(string inIp, int inDownloaded, int inUploaded, bool update, bool delete)
        {
            // Update the amount of file downloaded with the incoming downloaded variable
            amountOfFileAquired += inDownloaded;
            
            // Send the incoming variables to the UI update class
            uiUpdate.UpdatePeerList(inIp, inDownloaded, inUploaded, update, delete);
        }

        public void PauseTorrent()
        {
            // Set the puased variable to the opposite
            paused = !paused;

            string stat = "";

            // If the torrent has been paused, send a stop request and update the UI to show pause
            if(paused)
            {
                SendStopRequest();
                stat = "Paused";
            }

            // If the torrent has been unpaused, send a request to the tracker
            if(!paused)
            {
                SetUpRequest();
            }

            // Update the UI
            uiUpdate.UpdateGridView(listViewID, percentage + "%", 0, stat);
        }


        public void OpenExplorer()
        {
            // Open the content directory in the explorer
            Process.Start(parentPath);
        }
    }
}