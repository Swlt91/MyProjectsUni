using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class Listener
    {
        private ConnManager manager;
        public int port = 11000;
        public IPEndPoint localEndPoint;
        public IPAddress ipAddress;

        public Listener(ConnManager parent, int port)
        {
            manager = parent;
            this.port = port;
        }

        public void Start()
        {
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            // Finds the Ip address the allows multiple machines on a LAN to connect
            foreach (IPAddress ipAdd in ipHostInfo.AddressList)
            {
                if (ipAdd.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ipAdd;
                }
            }
            
            localEndPoint = new IPEndPoint(ipAddress, port);

            manager.ip = ipAddress;
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                manager.connected = true;

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    manager.add(handler);

                }
            }
            catch (Exception e)
            {
                // If the exception is a socket exception, then add one to the port number and try again

                manager.port++;
                manager.startListener();
                Console.WriteLine(e.ToString());

            }  
        }
    }
}
