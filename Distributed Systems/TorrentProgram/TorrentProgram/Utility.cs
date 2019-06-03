using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    public static class Utility
    {
        public static string SetSizeUI(long inAmount)
        {
            // Create array for each file size identifier
            string[] sizes = { "B", "KB", "MB", "GB" };
            double length = inAmount;
            int count = 0;

            // Divide the length by 1024, until it is below 1024.
            // End the loop once the count is over the size length - 1
            // Eventually returning the length/size in readable form
            while (length >= 1024 && count < sizes.Length - 1)
            {
                count++;
                length = length / 1024;
            }

            string result = String.Format("{0:0.##} {1}", length, sizes[count]);
            // Format the string to show the size and identifier
            return result;
        }
    }
}
