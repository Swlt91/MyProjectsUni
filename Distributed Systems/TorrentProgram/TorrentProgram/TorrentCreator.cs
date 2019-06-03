using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TorrentProgram
{
    class TorrentCreator
    {
        public string fileName;
        public string fileNameWithout;
        public byte[] hash;
        public string hashString;
        public string createdFilePath;
        public long fileSize;
        public int pieces;
        public int pieceSize;
        public string tracker;
        LoadingForm form;
        public int peersConnected = 0;
        string pathFile;
        public string parentPath;
        string id;
        bool showFile;

        public TorrentCreator(string inId, LoadingForm loadingForm, bool inShowFile)
        {          
            hash = null;
            hashString = "";
            fileSize = 0;
            pieces = 0;
            pieceSize = 0;
            id = inId;
            form = loadingForm;
            parentPath = System.IO.Directory.GetCurrentDirectory();
            parentPath = parentPath + "\\" + id + "\\";
            showFile = inShowFile;
        }
     
        public void CalculatePieceSize()
        {
            // This method determines how many pieces the file should be cut into, determined by the overall size of the file
            form.UpdateForm("Calculating piece size", 25);


            if (fileSize <= 100000)
            {
                pieceSize = (int)(fileSize / 20);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 100000 && fileSize <= 10000000)
            {
                pieceSize = (int)(fileSize / 50);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 10000000 && fileSize <= 50000000)
            {
                pieceSize = (int)(fileSize / 100);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 50000000 && fileSize <= 200000000)
            {
                pieceSize = (int)(fileSize / 250);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 200000000 && fileSize <= 400000000)
            {
                pieceSize = (int)(fileSize / 500);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 400000000 && fileSize <= 1000000000)
            {
                pieceSize = (int)(fileSize / 600);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 1000000000 && fileSize <= 5000000000)
            {
                pieceSize = (int)(fileSize / 700);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 5000000000 && fileSize <= 10000000000)
            {
                pieceSize = (int)(fileSize / 1000);
                pieces = (int)(fileSize / pieceSize);
            }

            else if (fileSize > 10000000000)
            {
                pieceSize = (int)(fileSize / 1500);
                pieces = (int)(fileSize / pieceSize);
            }
            // If the filesize does not evenly divide by the piece amount there is an overflow, add one more piece (representing a smaller piece than the rest)
            if (fileSize % pieceSize > 0)
            {
                pieces++;
            }
            
        }

     
        public void CopyFile(string path, string destination)
        {
            try
            {
                using (FileStream fwrite = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Put the file stream pointer to the area of data that is requested
                    int bufferSize = 1000 * 1024; // 10MB
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = 0;

                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        
                        while ((bytesRead = fs.Read(buffer, 0, bufferSize)) > 0)
                        {
                            fileSize += bytesRead;

                            if (bytesRead < bufferSize)
                            {
                                bufferSize = bytesRead;
                                // please note array contains only 'bytesRead' bytes from 'bufferSize'
                            }


                            fwrite.Write(buffer, 0, bufferSize);
                            // here 'buffer' you get current portion on file 
                            // process this
                            buffer = new byte[bufferSize];
                        }
                    }
                }
            }

            catch (Exception e)
            {

            }
        }
        public bool CreateTorrentFile(string path)
        {
            try
            {
                //get hash of the content
                pathFile = path;
                using (FileStream stream = File.OpenRead(path))
                {
                    using (SHA1Managed sha = new SHA1Managed())
                    {
                        hash = sha.ComputeHash(stream);
                        string sendCheckSum = BitConverter.ToString(hash)
                            .Replace("-", string.Empty);
                    }
                }

                // Get the file size
                hashString = BitConverter.ToString(hash);
                fileSize = new FileInfo(path).Length;
                fileName = Path.GetFileName(path);
                fileNameWithout = Path.GetFileNameWithoutExtension(path);

                // Implemented tracker is at this address
                tracker = "https://seanthomas1991.000webhostapp.com/";


                form.UpdateForm("Creating file directory", 20);
                string destinationFile = System.IO.Path.Combine(parentPath + fileNameWithout, fileName);

                // If the directory for the content does not exist create it
                if (!File.Exists(destinationFile))
                {
                    // This moves the selected content to be torrented, into a path for the program
                    System.IO.Directory.CreateDirectory(parentPath + fileNameWithout).ToString();
                    System.IO.File.Copy(path, destinationFile, true);
                   // CopyFile(path, destinationFile);
                }

                // Calculate the piece size
                CalculatePieceSize();

                // Write to the file
                WriteFile(path);

               

                form.UpdateForm("Completed", 5);
       
                return true;
            }

            catch (Exception e)
            {
                Console.Write(e.TargetSite.ToString());
                return false;
            }

        }

        public string HashPiece(int place, string path)
        {
            // Hash piece by index
            string sendCheckSum;
            byte[] pieceToHash = new byte[pieceSize];

            using (FileStream stream = File.OpenRead(path))
            {
                // find the position in the file
                if (place > 0)
                {
                    stream.Position = place * pieceSize;
                }
                
                // Hash the data and return it in string form
                stream.Read(pieceToHash, 0, pieceSize);
                using (SHA1Managed sha = new SHA1Managed())
                {
                    pieceToHash = sha.ComputeHash(pieceToHash);

                    return sendCheckSum = BitConverter.ToString(pieceToHash).Replace("-", string.Empty);
                }
            }
        }

        public void WriteFile(string inPath)
        {
            long amountHashed = 0;
            int count = 0;

            form.UpdateForm("Creating file", 25);

            // Create torrent file directory, if it already exists, nothing will change
            System.IO.Directory.CreateDirectory(parentPath + "TorrentFiles").ToString();
            string path = parentPath + "TorrentFiles\\" + fileNameWithout + "TorrentFile.txt";
            createdFilePath = path;

            // Write to the file

            try
            {


                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(fileName);
                    sw.WriteLine(hashString);
                    sw.WriteLine(fileSize.ToString());
                    sw.WriteLine(pieces.ToString());
                    sw.WriteLine(pieceSize.ToString());
                    sw.WriteLine(tracker);

                    int percent = pieces / 50;
                    int percentCount = 0;

                    // Go through the file and hash it piece by piece
                    while (amountHashed < fileSize)
                    {
                        string hashedByte = HashPiece(count, inPath);
                        sw.Write(hashedByte);
                        amountHashed += pieceSize;
                        count++;
                        percentCount++;

                        if (percentCount == percent)
                        {
                            form.UpdateForm("Hashing file pieces", 1);
                            percentCount = 0;
                        }

                    }
                }

                // show the file in the explorer
                if (showFile)
                {
                    Process.Start(parentPath + "TorrentFiles\\");
                }
                form.UpdateForm("Uploading file to tracker", 25);

                // Upload the file to the server
                WebClient client = new WebClient();
                byte[] response = client.UploadFile(tracker + "/FileUpload.php", "POST", path);

                string s = client.Encoding.GetString(response);
            }

            catch(Exception e)
            {

            }
        }  
    }
}