using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    public class PeerResponse
    {
        public string ipAddress;
        public int port;
        public int amountLeft;

        // Used for the reponse from the tracker
        public PeerResponse(string inIpAddress, int inPort, int inAmount)
        {
            ipAddress = inIpAddress;
            port = inPort;
            amountLeft = inAmount;
        }
    }
}
