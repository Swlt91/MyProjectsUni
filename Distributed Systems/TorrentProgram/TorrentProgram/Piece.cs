using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    public class Piece
    {
        public int pieceNumber;
        public byte[] bytes;
        public string IpAddress;

        public Piece(int inPieceNumber, byte[] inBytes, string inIP)
        {
            pieceNumber = inPieceNumber;
            bytes = inBytes;
            IpAddress = inIP;
        }
    }
}
