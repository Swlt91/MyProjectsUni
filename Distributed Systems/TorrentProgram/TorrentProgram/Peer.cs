using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class Peer
    {
        public int downloaded = 0;
        public int uploaded = 0;
        public string ipAddress;
        public int port;
        public bool handshakeSent;
        public bool handshakeRecieved;
        public Connection connection;
        public int pieceDownloading;
        public bool downloading;
        List<int> pieceNeedList;
        int pieceNeedListOriginalCount;
        List<int> pieceHaveList;
        int pieceHaveListOriginalCount;
        public bool paused;
        bool allsent = false;
        public TorrentFile torrentFile;
        int pieceRequested;
        bool didConnect = false;
        int randomPieceSelected;
        Random rand;

        public Peer(string inIpAdress, int inClient, bool indidConnect, TorrentFile inTorrentFile, Connection conn)
        {
            rand = new Random();
            paused = false;
            Console.WriteLine("Peer created" + " " + inClient);
            downloading = false;
            ipAddress = inIpAdress;
            port = inClient;
            handshakeSent = false;
            handshakeRecieved = false;
            torrentFile = inTorrentFile;
            connection = conn;


            //if the peer is requesting the connection, the peer must send the handshake first
            didConnect = indidConnect;
            if(didConnect)
            {
                SendHandshake(torrentFile.fileName);
                handshakeSent = true;
            }
        }


        public void SetConnection(Connection conn)
        {
            connection = conn;
        }


        public void SendHandshake(string fileName)
        {
            // The connection has been made, send an update to UI to show another peer is conneccted

            torrentFile.UpdatePeer(1);
            torrentFile.UpdatePeerDataGrid(connection.ipAddress, downloaded, uploaded, false, false);

            // Send a handshake message, sending the entire file hash as a unique identifier
            connection.write("HANDSHAKE:" + torrentFile.hashString + ":" + connection.serverPort.ToString());
            handshakeSent = true;
        }

        public void ReceiveHandshake(string fileName)
        {
            bool found = false;

            // If the peers was created from the listner, the handshake recieved will send which file it is requesting.
            // This peer then checks through the list of open torrents and checks if the requested one is on the list
            if (torrentFile == null)
            {
                foreach(TorrentFile torrent in connection._manager.torrentFiles)
                {
                    if(fileName.Equals(torrent.hashString))
                    {
                        torrentFile = torrent;
                        found = true;
                        Console.WriteLine("PEER CONNECT");             
                    }
                }
            }

            // Else the peer initiated the connection and knows which torrent is in use for this communication
            else
            {
                if (fileName.Equals(torrentFile.hashString))
                {
                    found = true;
                    Console.WriteLine("PEER CONNECT TORRENT");
                  
                }
            }

            // Sends a handshake back and copies the piece need and have list to this peer instance
            // This is so the peer can document which pieces it has sent and recieved within this connection
            if (found && !handshakeSent)
            {
                SendHandshake(torrentFile.fileName);
                handshakeSent = true;              
                handshakeRecieved = true;
                pieceNeedList = new List<int>(torrentFile.pieceNeedList);
                pieceNeedListOriginalCount = pieceNeedList.Count;
                pieceHaveList = new List<int>(torrentFile.pieceHaveList);
                pieceHaveListOriginalCount = pieceHaveList.Count;
                torrentFile.CalculatePercentage();

                // If the peer did not create the connected, send the file status to the connected peer
                if (!didConnect)
                {
                    SendFileStatus();
                }
            }
            else
            {
                pieceNeedList = new List<int>(torrentFile.pieceNeedList);
                pieceNeedListOriginalCount = pieceNeedList.Count;
                pieceHaveList = new List<int>(torrentFile.pieceHaveList);
                pieceHaveListOriginalCount = pieceHaveList.Count;
                torrentFile.CalculatePercentage();


                // If the torrent request was not found, 
                if (!handshakeRecieved && !found)
                {
                    connection.write("DISCONNECT");
                    connection._state.kill = true;
                }
          
            }
        }

        public void ResetNeedList()
        {
            // Updates the piece need List to check if more pieces of the torrent are available
            pieceNeedList = new List<int>(torrentFile.pieceNeedList);
            pieceNeedListOriginalCount = pieceNeedList.Count;
        }

        public void SendInterest()
        {
            // Send interest to connected Peer, to indicate pieces are still needed
            if (torrentFile.pieceNeedList.Count > 0)
            {
                connection.write("INTERESTED");
            }

            else
            { 
                connection.write("DISCONNECT");
            }
        }

        public void GetFileInfo(string fileInfo)
        {
            string[] components = fileInfo.Split(':');
         
            if(torrentFile.pieceNeedList.Count > 0)
            {
                SendInterest();
            }
        }

        public void HandleMessage(string message)
        {
            if (downloading)
            {
              //  DownloadPiece(message);
            }

            else
            {
                if (message.StartsWith("HANDSHAKE"))
                {
                  
                    string[] components = message.Split(':');

                    if (!handshakeRecieved)
                    {
                        ReceiveHandshake(components[1]);
                    }

                }
                else if (message.StartsWith("FILEINFO"))
                {
                    GetFileInfo(message);
                }
                else if (message.StartsWith("BITFIELD"))
                {

                }

                else if (message.StartsWith("REQUEST"))
                {
                    ProcessRequest(message);
                }

                else if (message.StartsWith("READY"))
                {
                   SendPiece();
                }

                else if(message.StartsWith("INTERESTED"))
                {
                    SendHave();
                }
                else if(message.StartsWith("DISCONNECT"))
                {
                    connection._state.kill = true;
                }

                else if(message.StartsWith("HAVE"))
                {
                    ProcessHave(message);
                }

                else if (message.StartsWith("ALLREQUESTED"))
                {
                    // If peer has requested all, check if every piece available has been sent
                    // If not let the connected Peer other pieces are still available
                    if (!torrentFile.completed || !allsent)
                    {
                        SendHave();
                    }

                    else
                    {
                        // If every available piece has been sent, alert the connect peer
                        if (torrentFile.completed)
                        {
                            connection.write("COMPLETED");
                        }

                        else
                        {
                            connection.write("DISCONNECT");
                        }
                    }
                }

                else if (message.StartsWith("ALLSENT"))
                {
                    // If the all sent message has been sent and recieved, then there are no more pieces to be shared
                    if(allsent)
                    {
                        connection.write("DISCONNECT");
                    }

                    else
                    {
                        SendHave();
                    }
                  
                }

                else if(message.StartsWith("NOPIECE"))
                {
                    SendInterest();
                }

                else if (message.StartsWith("COMPLETE"))
                {
                    // If both the connected peer and host peer are complete, disconnect
                    if(torrentFile.completed)
                    {
                        connection.write("DISCONNECT");
                    }
                    
                    // Else one peer is complete and one is not, therefor one peer can help the completion of the file
                    // Continue requesting pieces and reset the need list
                    else
                    {
                        ResetNeedList();
                        ProcessHave("HAVE:0");
                    }
                }

                else if (message.StartsWith("PIECE"))
                {
                    string[] components = message.Split(':');
                    pieceDownloading = int.Parse(components[1]);

                    // A further check to ensure the piece requested has not been downloaded from another peer while this piece was being requested
                    if (torrentFile.pieceNeedList.Contains(pieceDownloading))
                    {
                        downloading = true;
                        connection._reader.downloading = true;
                        PrepareForPiece(message);
                    }

                    else
                    {
                        SendInterest();
                    }
                }
            }
        }

        public void ProcessHave(string message)
        {
            string[] components = message.Split(':');
            int havePiece = int.Parse(components[1]);
            bool sendHave = true;
            paused = torrentFile.paused;
            torrentFile.CalculatePercentage();

            if (!paused)
            {
                // If the peer does not need the piece from the have message, find a piece that is needed and send a request
                if (!torrentFile.pieceNeedList.Contains(havePiece))
                {

                    sendHave = false;
                   
                    // Ensure a piece is needed
                    if (pieceNeedList.Count > 0)
                    {
                        // Get a index of a piece from the piece need list
                        int randNum = rand.Next(0, pieceNeedList.Count);

                        // Once a piece is found, remove it from the need list to avoid multiple downloads of the same piece in other threads
                        havePiece = pieceNeedList[randNum];
                        pieceNeedList.RemoveAt(randNum);

                        if (torrentFile.pieceNeedList.Count < 1)
                        {
                            FileComplete();
                        }

                        // If the piece is not currently downloading and is still needed, download
                        if (!torrentFile.piecesDownloading.Contains(havePiece) && torrentFile.pieceNeedList.Contains(havePiece))
                        {
                            sendHave = true;
                        }
                    }
                }

                // If a piece to be downloaded has been found, request it
                if (sendHave)
                {
                    SendPieceRequest(havePiece);
                }

                // If every piece has been requested, send the all requested message
                if (!sendHave)
                {

                    connection.write("ALLREQUESTED");
                }
            }

            else
            {
                connection.write("DISCONNECT");
            }   
        }

        public void SendHave()
        {
            // Ensure the peer has pieces to share
            if (pieceHaveList.Count > 0)
            {
               // Select a piece at random and send a have message to the connected peer
                randomPieceSelected = rand.Next(0, pieceHaveList.Count);
                int havePiece = pieceHaveList[randomPieceSelected];

                // Remove the selected piece from this peers have list, preventing the same piece being sent twice
                pieceHaveList.RemoveAt(randomPieceSelected);
                connection.write("HAVE:" + havePiece);

                // If the list is now empty
                if (pieceHaveList.Count < 1)
                {
                    // First check to see if more pieces of the torrent have been found by checking the torrent file have list
                    // Then minus the original peer have list count, if the answer is above 0, more pieces have been acquired
                    int sum = torrentFile.pieceHaveList.Count - pieceHaveListOriginalCount;

                    for (int i = 0; i < sum; i++)
                    {
                        // Add each new piece to the peer piece have list
                        pieceHaveList.Add(torrentFile.pieceHaveList[pieceHaveListOriginalCount]);
                        pieceHaveListOriginalCount++;
                    }
                }
            }

            // Else there are no more pieces to be sent, send the all sent message
            else
            {
                allsent = true;
                connection.write("ALLSENT:");      
            }
        }

        public void SendFileStatus()
        {
            // Prepare a message constaining the file information 
            byte[] messageByte = new byte[32 + torrentFile.fileSize.ToString().Length + torrentFile.fileName.Length + torrentFile.pieces.ToString().Length];
            int count = 0;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes("FILEINFO:"), 0, messageByte, 0, 9);
            count += 9;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes("FILENAME:"), 0, messageByte, 9, 9);
            count += 9;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(torrentFile.fileName + ":"), 0, messageByte, 18, torrentFile.fileName.Length + 1);
            count += torrentFile.fileName.Length + 1;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes("SIZE:"), 0, messageByte, count, 5);
            count += 5;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(torrentFile.fileSize.ToString()), 0, messageByte, count, torrentFile.fileSize.ToString().Length);
            count += torrentFile.fileSize.ToString().Length;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(":PIECES"), 0, messageByte, count, 7);
            count += 7;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(":" + torrentFile.pieces.ToString()), 0, messageByte, count, torrentFile.pieces.ToString().Length + 1);


            string message = Encoding.ASCII.GetString(messageByte) + ":";
            connection.write(message);
        }

        public void SendPieceRequest(int havePiece)
        {
            // Send a request for the piece
            string request = "REQUEST:";    
            string piece = havePiece.ToString();
            connection.write(request + piece);
          
        }

        public void SendPiece()
        {           
            Console.WriteLine("PIECE SEND " + pieceRequested + " " + port);

            // Find the starting point of where the piece is within the file
            int fileChunk = torrentFile.pieceSize;
            int position = 0;

            // if the piece requested is not zero, work out the position in the file to start from
            if (pieceRequested > 0)
            {
               position = pieceRequested * torrentFile.pieceSize;
            }

            // if the position + the piece size is greatter than the size of the file
            // calculate the size of this piece
            if(position + fileChunk > torrentFile.fileSize)
            {
                fileChunk = (int)torrentFile.fileSize - position;
            }

           
            // First send the size of this message so the connected peer can prepare
            int sum = 0;
            int count = 0;
            int amountToSend = 256;
            byte[] length = BitConverter.GetBytes(fileChunk);
            connection._state.sock.Send(length, 0, 4, 0);
       
            try
            {
                using (FileStream fs = new FileStream(torrentFile.parentPath + torrentFile.fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Put the file stream pointer to the area of data that is requested
                    fs.Position = position;

                    // While the amount sent is less than the amount of the piece
                    while (sum < fileChunk)
                    {
                        byte[] bytes = new byte[256];

                        // Read and send part of the file
                        count = fs.Read(bytes, 0, amountToSend);
                        connection._state.sock.Send(bytes, 0, count, 0);
                        sum += count;

                        // Prevents reading more data than is needed
                        if (sum + amountToSend > fileChunk)
                        {
                            amountToSend = fileChunk - sum;
                        }
                    }
                }

                // Add to the amount uploaded and update the UI
                uploaded = +fileChunk;
                Task.Run(() => torrentFile.UpdatePeerDataGrid(connection.ipAddress, 0, uploaded, true, false));
                Task.Run(() => torrentFile.CalculatePercentage());
            }

            catch(Exception e)
            {

            }

            GC.Collect();
        }
        public void ProcessRequest(string message)
        {
            string[] components = message.Split(':');
            pieceRequested = int.Parse(components[1]);

            // Check if the piece requested is on this peers piece have list
            if (torrentFile.pieceHaveList.Contains(pieceRequested) && !paused)
            {
                paused = torrentFile.paused;
              
                // Piece found, send confirmation back to the connected Peer
                string write = "PIECE:" + pieceRequested.ToString() + ":5000:";
                connection.write(write);     
            }

            else
            {
                // No piece found
                if (!paused)
                {
                    connection.write("NOPIECE:");            
                }

                else
                {
                    connection.write("DISCONNECT");
                }
                //connection.write(""
            }             
        }

        public void PrepareForPiece(string message)
        {
            // Prepare the program for piece downloading
            string[] components = message.Split(':');
            pieceDownloading = int.Parse(components[1]);

            // set downloading to true and add the piece to the downloading piece list in the torrent instance
            downloading = true;
            torrentFile.piecesDownloading.Add(pieceDownloading);
            connection.write("READY");
            DownloadPiece();
        }


        public void DownloadPiece()
        {

            int amountMessageRecieved = 0;

            // First read from the socket the size of the message
            byte[] bytes = new byte[4];
            int bytesRead = connection._state.sock.Receive(bytes, 0, 4, 0);

            int messageSize = BitConverter.ToInt32(bytes, 0);
            int amountToRecieve = 256;

            // Prepare the method for the size of the piece
            if (messageSize < amountToRecieve)
            {
                amountToRecieve = messageSize;
            }

            // work out the area of the file to place the download into
            int fileArea = 0;

            if (pieceDownloading > 0)
            {
                fileArea = pieceDownloading * torrentFile.pieceSize;
            }

            try
            {

                using (FileStream fs = new FileStream(torrentFile.parentPath + torrentFile.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Move the file stream pointer to where the data is to be written
                    fs.Position = fileArea;
                    while (amountMessageRecieved < messageSize)
                    {
                        bytes = new byte[amountToRecieve];

                        // Read data from the socket
                        bytesRead = connection._state.sock.Receive(bytes, 0, amountToRecieve, 0);

                        // Write the data to the file
                        fs.Write(bytes, 0, bytesRead);

                        // add to the amount downloaded
                        downloaded += bytesRead;

                        amountMessageRecieved += bytesRead;

                        // Prevent reading more data from the socket than has been sent
                        if (amountMessageRecieved + bytesRead > messageSize)
                        {
                            amountToRecieve = messageSize - amountMessageRecieved;

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }


            // Check the hash of the piece downloaded with the hash found in the file
            torrentFile.CheckSinglePiece(pieceDownloading, connection.ipAddress, messageSize);
            GC.Collect();
            downloading = false;
            connection._reader.downloading = false;

            // Check if the file is complete, if not send interest
            if (torrentFile.pieceNeedList.Count < 1)
            {
                FileComplete();
            }

            else
            {
                SendInterest();
            }
        }


        public void FileComplete()
        {
            // Send a request to the tracker to indicate completetion
            // Send the a complete message to other peer
            connection._manager.SendRequest(torrentFile.listViewID);
            connection.write("COMPLETE");
        }
    }
}
