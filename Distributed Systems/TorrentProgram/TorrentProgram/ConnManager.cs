using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class ConnManager
    {
        // Connection Tracking
        private List<Connection> connections;
        public List<TorrentFile> torrentFiles;
        public bool connected;
        public int port;
        public Listener listener = null;
        public IPAddress ip;

        public ConnManager(int port, bool tryRandom)
        {
            connected = false;
            connections = new List<Connection>();
            torrentFiles = new List<TorrentFile>();
            this.port = port;
        }

        public void SendRequest(int inId)
        {         
            // Find the torrent file by list id and send a request
            foreach(TorrentFile torrent in torrentFiles)
            {
                if(inId == torrent.listViewID)
                {
                  
                    if (torrent.SetUpRequest())
                    {
                        ConnectPeers(torrent);
                    }
                }
            }
            
        }

        public void startListener()
        {
            listener = new Listener(this, port);
            Thread listenThread = new Thread(new ThreadStart(listener.Start));
            listenThread.Start();
        }

        public void add(Socket sock)
        {
            Connection conn;

            conn = new Connection(sock, this, processPeer, null, port, false);

            Thread connThread = new Thread(new ThreadStart(conn.start));
            connThread.Start();
            connections.Add(conn);
        }

        public int processPeer(Connection conn)
        {
            if (conn._state.hasRead())
            {
                string message = conn._state.dequeueRead();

                #region DEBUG Printout
                Console.ForegroundColor = conn._state.cc;
                Console.WriteLine(conn.g.ToString() + " " + message + " " + message.Length);
                Console.ResetColor();
                #endregion
                conn.peer.HandleMessage(message);


            }
            return 1;
        }

        public Connection ConnectTo(string inIpAddress, int port, Func<Connection, int> processingFunction, TorrentFile inTorrentFile)
        {

            #region Connect to the specified address
            System.Net.IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse(inIpAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(localEndPoint);
            #endregion

            Connection connection = new Connection(sender, this, processingFunction, inTorrentFile, port, true);
            connections.Add(connection);
           

            Thread clientThread = new Thread(new ThreadStart(connection.start));
            clientThread.Start();
            return connection;
        }


        public void ConnectPeers(TorrentFile torrent)
        {
            //connects to each peer in the tracker response

            //if the torrent file is not complete, then attempt to connect
            if (!torrent.completed)
            {
                foreach (PeerResponse peer in torrent.peerList.ToList())
                {
                    try
                    {
                        //a check is made to ensure the peer from the tracker response is not already connected
                        bool found = false;

                        foreach(Connection con in connections)
                        {
                            if(con.peer.torrentFile.fileName.Contains(torrent.fileName) && con.ipAddress.Contains(peer.ipAddress) && con.port == peer.port)
                            {
                                found = true;
                                break;
                            }
                        }

                        //if no existing connection is found, then a new connection is made
                        if (!found)
                        {
                            Connection conn = ConnectTo(peer.ipAddress, peer.port, processPeer, torrent);
                        }
                    }

                    catch (Exception e)
                    {
                        // If the connection can not be made, remove this peer entry from the peer list within the torrent file
                        torrent.peerList.Remove(peer);
                        Console.WriteLine("Could not connect to " + peer);
                    }
                }
            }
        }

        public void RemovePeer(Connection inConnection)
        {
            //removes a connection from the list
            connections.Remove(inConnection);
            inConnection.peer = null;
            inConnection = null;
            GC.Collect();
        }

        public bool CheckTorrentFileDownloading(TorrentFile torrent)
        {
            //checks to see if the torrent file is already opened by the program
            foreach(TorrentFile torr in torrentFiles)
            {
                if(torrent.fileName.Equals(torr.fileName) && torrent.hashString.Equals(torr.hashString))
                {
                    return true;
                }
            }

            return false;
        }

        public void PauseTorrent(int listID)
        {
            //finds the paused torrent from the ui list id and pauses network on the torrent
            foreach(TorrentFile torrent in torrentFiles)
            {
                if (listID == torrent.listViewID)
                {
                    // Ends communication on any peers
                    torrent.PauseTorrent();
                    break;
                }
            }
        }


        public void ShowTorrentInFolder(int listID)
        {
            // Search through the torrent list and open the selected file
            foreach (TorrentFile torrent in torrentFiles)
            {
                if (listID == torrent.listViewID)
                {
                    // Opens the file in folder view
                    torrent.OpenExplorer();
                    break;
                }
            }
        }
    }
}
