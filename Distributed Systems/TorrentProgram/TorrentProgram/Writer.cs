using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class Writer
    {
        ConnectionState state;
        public Writer(ConnectionState state)
        {
            this.state = state;
        }

        public void start()
        {
            while (!state.kill)
            {
                while (state.hasWrite())
                {
                    string message = state.dequeueWrite();
               
                    try
                    {
                        int bytesCount = 0;
                        byte[] sendBuffer = new byte[4096];
                        bytesCount = message.Length;
                        byte[] messageByte = Encoding.ASCII.GetBytes(message);

                        // Send the message size in the first 4 bytes, with the rest of the message following
                        Buffer.BlockCopy(BitConverter.GetBytes(bytesCount), 0, sendBuffer, 0, 4);
                        Buffer.BlockCopy(messageByte, 0, sendBuffer, 4, messageByte.Length);                       
                        state.sock.Send(sendBuffer, 0, bytesCount + 4,0);
                    }
                    catch (Exception e)
                    {
                        state.kill = true;
                        Console.WriteLine(e.ToString());
                        break;
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
