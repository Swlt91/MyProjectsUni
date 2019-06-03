using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentProgram
{
    public class ConnectionState
    {
        ManualResetEvent mre_readQueue;
        ManualResetEvent mre_writeQueue;
        Queue<String> readQueue;
        Queue<String> writeQueue;
        public Socket sock;
        public bool kill = false;
        public ConsoleColor cc;
        public bool downloading;

        public ConnectionState()
        {
            mre_readQueue = new ManualResetEvent(true);
            mre_writeQueue = new ManualResetEvent(true);
            readQueue = new Queue<string>();
            writeQueue = new Queue<string>();
            sock = null;
            
            Random r = new Random();
            cc = (ConsoleColor)r.Next(0, 16);
        }

        public bool hasRead()
        {
            if (readQueue.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool hasWrite()
        {
            if (writeQueue.Count > 0)
            {
                return true;
            }
            return false;
        }
        public int enqueueRead(string temp)
        {
            mre_readQueue.WaitOne();
            mre_readQueue.Reset();

            readQueue.Enqueue(temp);

            mre_readQueue.Set();
            return 0;     // No actual return value, alter to indicate queue success
        }

        public int enqueueWrite(string temp)
        {
            mre_writeQueue.WaitOne();

            mre_writeQueue.Reset();

            writeQueue.Enqueue(temp);

            mre_writeQueue.Set();
            return 0;     // No actual return value, alter to indicate queue success
        }

        public string dequeueRead()
        {
            string temp;
            mre_readQueue.WaitOne();
            mre_readQueue.Reset();
          
            temp = readQueue.Dequeue();
            Console.WriteLine(temp);
            mre_readQueue.Set();
            return temp;
        }

        public string dequeueWrite()
        {
            string temp;
            mre_writeQueue.WaitOne();
            mre_writeQueue.Reset();

            temp = writeQueue.Dequeue();

            mre_writeQueue.Set();
            return temp;
        }
    }
}
