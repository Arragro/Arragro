using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.ServiceManagement
{
    public class BlobStorageManager
    {        
        private readonly string _connectionString;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudBlobClient _cloudBlobClient;

        public BlobStorageManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))

            _connectionString = connectionString;
            _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();

            var blobProperties = _cloudBlobClient.GetServiceProperties();
        }

        private CloudBlobContainer GetBlobContainer(string containerName)
        {
            return _cloudBlobClient.GetContainerReference(containerName);
        }

        public CloudBlobContainer CreateBlobContainer(string containerName)
        {
            var container = GetBlobContainer(containerName);
            container.CreateIfNotExists();
            return container;
        }

        public CloudBlockBlob UploadStream(string containerName, Stream stream, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(stream);
            return blockBlob;
        }

        public void DeleteBlob(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.Delete();
        }
    }
}
