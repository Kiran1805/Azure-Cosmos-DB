using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobFileShare
{
    class Program
    {
        public static void Main(string[] args)
        {
            string folder = @"D:\uploadpath\";
            // Filename  
            string fileName = "downloadfile.txt";
            // Fullpath. You can direct hardcode it if you like.  
            string fullPath = folder + fileName;
            string containerName = "firstblobstoragr";
            BlobFileShare blobFileShare = new BlobFileShare();
            blobFileShare.uploadToBlob(fullPath, containerName);
     
            blobFileShare.downloadFromBlob(fileName, containerName);
        }
    }
}
