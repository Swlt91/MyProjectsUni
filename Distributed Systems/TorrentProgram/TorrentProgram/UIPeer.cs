using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class UIPeer
    {
        public string IpAddress;
        public int downloaded;
        public int uploaded;
        public int id;
        public string uiDownloaded;
        public string uiUploaded;

        public UIPeer(string inIP, int inId)
        {
            // Class used to update UI peers connected datagrid
            IpAddress = inIP;
            downloaded = 0;
            uploaded = 0;
            id = inId;
        }

        public void UIUpdate()
        {
            // Convert upload and download amount to readable format
            uiDownloaded = Utility.SetSizeUI(downloaded);
            uiUploaded = Utility.SetSizeUI(uploaded);
        }
    }
}
