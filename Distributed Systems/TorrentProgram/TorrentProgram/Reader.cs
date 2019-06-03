using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class Reader
    {
        ConnectionState state;
        const int bufferSize = 4096;
        byte[] bytes = new byte[bufferSize];
        public bool downloading;

        public Reader(ConnectionState state)
        {
            this.state = state;
            downloading = false;
        }


        public void start()
        {
            string message = "";
            int bytesRead = 0;
            int messageSize = 0;
            int amountMessageRecieved = 0;
            int count = 0;
            int amountToRecieve = 0 ;

            while (!state.kill)
            {
              
                try
                {
                    // If file data is not being sent
                    if (!downloading)
                    {                     
                        count = 0;
                        amountMessageRecieved = 0;

                        bytes = new byte[bufferSize];
                        
                        // First read the size of the incoming message
                        bytesRead = state.sock.Receive(bytes, 0, 4, 0);
                        messageSize = BitConverter.ToInt32(bytes, 0); 
                        
                        // Set the amount to recieve to the message size
                        amountToRecieve = messageSize;
                        bytes = new byte[bufferSize];

                        // While the amount recieved is less than the message size, continue to read from the socket
                        while (amountMessageRecieved < messageSize)
                        {
                            count = state.sock.Receive(bytes, amountMessageRecieved, amountToRecieve, 0);

                            // Update each variable accordingly
                            amountMessageRecieved += count;
                            amountToRecieve -= count;

                            // Ensure that we do not try to read more data from the socket than has been sent
                            if (amountMessageRecieved + count > messageSize)
                            {
                                amountToRecieve = messageSize - amountMessageRecieved;

                            }
                        }
                        message = Encoding.ASCII.GetString(bytes, 0, messageSize);
                        string result = message.Substring(0, 5);

                        // If the message contains the word piece, prepare for downloading file data
                        if (result.Contains("PIECE"))
                        {
                            downloading = true;
                        }
                        state.enqueueRead(message);
                        message = "";
                        bytesRead = 0;
                    }                

                }
                catch (Exception e)
                {                 
                    Console.WriteLine(e.ToString());
                    state.kill = true;
                    break;
                }
            }

            if(state.kill)
            {

            }
        }
    }
}
