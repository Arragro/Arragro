using Microsoft.WindowsAzure.Management.Storage.Models;
using System;

namespace Arragro.Azure
{
    public class AzureStorageConnectionStrings
    {
        //private const string _storageConnectionStringFormat = "<add key=\"StorageConnectionString\" value=\"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}\" />";
        private const string _storageConnectionStringFormat = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";

        public string PrimaryConnectionString { get; private set; }
        public string SecondaryConnectionString { get; private set; }

        public AzureStorageConnectionStrings(string accountName, StorageAccountGetKeysResponse storageAccountGetKeysResponse)
        {
            if (storageAccountGetKeysResponse.StatusCode != System.Net.HttpStatusCode.OK)
                throw new ArgumentException("The response from azure is not ok.", "storageAccountGetKeysResponse");

            PrimaryConnectionString = string.Format(_storageConnectionStringFormat, accountName, storageAccountGetKeysResponse.PrimaryKey);
            SecondaryConnectionString = string.Format(_storageConnectionStringFormat, accountName, storageAccountGetKeysResponse.SecondaryKey);
        }
    }
}
