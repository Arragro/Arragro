using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;

namespace Arragro.Azure.ServiceManagement.Storage
{
    public class TableStorageManager
    {
        private readonly string _connectionString;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudTableClient _cloudTableClient;

        public CloudTableClient TableClient { get { return _cloudTableClient; } }

        public TableStorageManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))

            _connectionString = connectionString;
            _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();

            var blobProperties = _cloudTableClient.GetServicePropertiesAsync().Result;
        }

        private CloudTable GetTable(string tableName)
        {
            return _cloudTableClient.GetTableReference(tableName);
        }

        public CloudTable CreateTable(string tableName)
        {
            var table = GetTable(tableName);
            if (!table.ExistsAsync().Result)
            {
                table.CreateAsync().Wait();
            }
            return table;
        }

        public bool DeleteTable(string tableName)
        {
            var table = GetTable(tableName);
            return table.DeleteIfExistsAsync().Result;
        }

        public static string GetTableSharedAccessSignatureUri(
            CloudTable table,
            string policyName,
            int sharedAccessExpiryTimeInHours = 24,
            SharedAccessTablePermissions sharedAccessTablePermissions = SharedAccessTablePermissions.Query)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            var sharedPolicy = new SharedAccessTablePolicy();
            sharedPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(sharedAccessExpiryTimeInHours);
            sharedPolicy.Permissions = sharedAccessTablePermissions;

            //Get the container's existing permissions.
            var permissions = new TablePermissions();

            //Add the new policy to the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            table.SetPermissionsAsync(permissions).Wait();

            return GetTableSharedAccessSignatureUri(table, policyName);
        }

        public static string GetTableSharedAccessSignatureUri(
            CloudTable table,
            string policyName)
        {
            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            var sasContainerToken = table.GetSharedAccessSignature(null, policyName);

            //Return the URI string for the container, including the SAS token.
            return table.Uri + sasContainerToken;

        }
    }
}
