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
    class Connection
    {

        public ConnectionState _state;
        public ConnManager _manager;
        public Reader _reader;
        private Writer _writer;
        public Peer peer;
        TorrentFile torrentFile;
        public string ipAddress;
        public int port;

        public int serverPort;
        public Guid g;
        private Func<Connection, int> processingFunction;


        public Connection(Socket sock, ConnManager manager, Func<Connection, int> processingFunction, TorrentFile inTorrentFile, int inPort, bool didConnect)
        {
            
            _state = new ConnectionState();

            // Set a timeout on the socket, as communication should be constant
            sock.ReceiveTimeout = 10000;
            sock.SendTimeout = 10000;
        
            _state.sock = sock;
            
            g = Guid.NewGuid();

            IPEndPoint remoteIpEndPoint = sock.RemoteEndPoint as IPEndPoint;
            ipAddress = remoteIpEndPoint.Address.ToString();

            _manager = manager;

            _reader = new Reader(_state);
            _writer = new Writer(_state);

            // Set the port and torrent file
            port = inPort;
            torrentFile = inTorrentFile;

            Console.WriteLine("CONNECTION CREATED!");
            peer = new Peer("", port, didConnect, torrentFile, this);

            if (processingFunction != null)
            {
                this.processingFunction = processingFunction;
            }
            else
            {
                this.processingFunction = this.defaultProcessing;
            }
        }

        public void start()
        {
            Thread readerThread = new Thread(new ThreadStart(_reader.start));
            Thread writerThread = new Thread(new ThreadStart(_writer.start));
            readerThread.Start();
            writerThread.Start();

            // kill loop:

            while (!_state.kill)
            {
                processingFunction(this);
            }

            // Update the UI 
            if (peer.torrentFile != null)
            {
                peer.torrentFile.UpdatePeer(-1);
                Console.WriteLine("PEER MINUSED");
            }

            Console.WriteLine("CONNECTION CLOSING!");

            // Remove this connection from the connections list and close the socket.
            _manager.RemovePeer(this);         
            _state.sock.Shutdown(SocketShutdown.Both);
            _state.sock.Close();
        }

        public bool dead()
        {
            return _state.kill;
        }
        public void setKill()
        {
            _state.kill = true;
        }


        public int defaultProcessing(Connection conn)
        {
            ProcessRead();
            return 1;
        }

        public bool ProcessRead()
        {
            if (_state.hasRead())
            {
                string message = _state.dequeueRead();
                Console.WriteLine(message);
            }
            else
            {
                Thread.Sleep(1);
            }
            return true;
        }

        public bool ProcessWrite()
        {
            return true;
        }

        public void write(string message)
        {
            Console.WriteLine(message);
            if (ProcessWrite())
            {
                _state.enqueueWrite(message);
            }
        }
    }
}
