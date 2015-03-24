using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Azure.ServiceManagement
{
    public class Storage : ServiceManagementBase
    {
        public Storage(string subscriptionId, string certificateName) : base(subscriptionId, certificateName) { }

        private void CreateStorageAccount(string accountName, StorageManagementClient client)
        {
            var storageAccountParameters = new StorageAccountCreateParameters
            {
                Name = accountName,
                AccountType = "Standard_GRS",
                Location = "Australia East"
            };

            var storageCreateResponse = client.StorageAccounts.Create(storageAccountParameters);

            if (storageCreateResponse.Status != OperationStatus.Succeeded)
                throw new CloudException(
                    string.Format("The create storage account did not succeed.\n\n\tHttp Status Code: {0}\n\tStatus Code: {1}\n\tError: {2}",
                        storageCreateResponse.HttpStatusCode, storageCreateResponse.StatusCode,
                        storageCreateResponse.Error));
        }

        public AzureStorageConnectionStrings CreateStorageAccount(string accountName)
        {
            var storageClient = CloudContext.Clients.CreateStorageManagementClient(CertificateCloudCredentials);

            var checkNameResponse = storageClient.StorageAccounts.CheckNameAvailability(accountName);

            if (checkNameResponse.IsAvailable)
            {
                CreateStorageAccount(accountName, storageClient);
            }

            return new AzureStorageConnectionStrings(accountName, storageClient.StorageAccounts.GetKeys(accountName));
        }

        public AzureStorageConnectionStrings GetStorageAccountConnectionStrings(string accountName)
        {
            var storageClient = CloudContext.Clients.CreateStorageManagementClient(CertificateCloudCredentials);
            var keysResponse = storageClient.StorageAccounts.GetKeys(accountName);
            return new AzureStorageConnectionStrings(accountName, keysResponse);
        }

        public void RemoveStorageAccount(string accountName)
        {
            var storageClient = CloudContext.Clients.CreateStorageManagementClient(CertificateCloudCredentials);
            var storageAccountGetResponse = storageClient.StorageAccounts.Get(accountName);

            if (storageAccountGetResponse.StorageAccount != null)
            {
                var operationResponse = storageClient.StorageAccounts.Delete(accountName);
                if (operationResponse.StatusCode != System.Net.HttpStatusCode.NotFound &&
                    operationResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new CloudException(
                        string.Format("The operation failed.\n\n\tStatus Code: {0}", operationResponse.StatusCode));
            }
        }
    }
}
