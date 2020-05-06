using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace BlobFileShare
{
    class BlobFileShare
    {
        string storageAccount_connectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountfirst811b;AccountKey=isFT3L8nk3Fmz0FNlkjPH5WVyoWFbLy8gWxxYfqS9l0ET8G9N1sErKaYo7t/PpIxlwH3BASjoD0XtHFvN/Kmig==;EndpointSuffix=core.windows.net";

        public void uploadToBlob(string fileToUpload, string azure_ContainerName)
        {
            Console.WriteLine("Inside upload method");

            string file_extension, filename_withExtension;
            Stream file;
 
            file = new FileStream(fileToUpload, FileMode.Open);

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(storageAccount_connectionString);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(azure_ContainerName);
 
            if (container.CreateIfNotExists())
            {
                container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
  
            file_extension = Path.GetExtension(fileToUpload);
            filename_withExtension = Path.GetFileName(fileToUpload);

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;

            cloudBlockBlob.UploadFromStreamAsync(file);  

            Console.WriteLine("Upload Completed!");

        }
        public void downloadFromBlob(string filetoDownload, string azure_ContainerName)
        {
            Console.WriteLine("Inside downloadfromBlob()");

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(storageAccount_connectionString);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(azure_ContainerName);
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filetoDownload);
         
            string folder = @"D:\uploadpath\";
            string fileName = "downloadedfile.txt";
            string fullPath = folder + fileName;
            Stream file = File.OpenWrite(fullPath);    
          
            cloudBlockBlob.DownloadToStream(file);

            Console.WriteLine("Download completed!");
            Console.ReadKey();

        }
    }
}
