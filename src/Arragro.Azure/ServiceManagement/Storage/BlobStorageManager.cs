using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.ServiceManagement.Storage
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

        public static string GetContainerSasUri(
            CloudBlobContainer container,
            string policyName,
            int sharedAccessExpiryTimeInHours = 24,
            SharedAccessBlobPermissions sharedAccessBlobPermissions = SharedAccessBlobPermissions.List)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            var sharedPolicy = new SharedAccessBlobPolicy();
            sharedPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(sharedAccessExpiryTimeInHours);
            sharedPolicy.Permissions = sharedAccessBlobPermissions;

            //Get the container's existing permissions.
            var permissions = new BlobContainerPermissions();

            //Add the new policy to the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            container.SetPermissions(permissions);

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            var sasContainerToken = container.GetSharedAccessSignature(null, policyName);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }

        public CloudBlockBlob UploadStream(string containerName, Stream stream, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(stream);
            return blockBlob;
        }

        public static CloudBlockBlob UploadStream(Uri uri, Stream stream, string blobName)
        {
            var container = new CloudBlobContainer(uri);
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
