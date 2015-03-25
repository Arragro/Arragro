﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Azure.AccountManagement
{
    public class StorageAccountManager : AccountManagementBase
    {
        public StorageAccountManager(string subscriptionId, string certificateName) : base(subscriptionId, certificateName) { }
        
        private void CreateStorageAccount(
            string accountName, StorageManagementClient client,
            string accountType, string location)
        {
            if (string.IsNullOrEmpty(accountType)) throw new ArgumentNullException("accountType");
            if (string.IsNullOrEmpty(location)) throw new ArgumentNullException("location");
            
            var storageAccountParameters = new StorageAccountCreateParameters
            {
                Name = accountName,
                AccountType = accountType,
                Location = location
            };

            var storageCreateResponse = client.StorageAccounts.Create(storageAccountParameters);

            if (storageCreateResponse.Status != OperationStatus.Succeeded)
                throw new CloudException(
                    string.Format("The create storage account did not succeed.\n\n\tHttp Status Code: {0}\n\tStatus Code: {1}\n\tError: {2}",
                        storageCreateResponse.HttpStatusCode, storageCreateResponse.StatusCode,
                        storageCreateResponse.Error));
        }

        public AzureStorageConnectionStrings CreateStorageAccount(
            string accountName, string accountType, string location)
        {
            var storageClient = CloudContext.Clients.CreateStorageManagementClient(CertificateCloudCredentials);

            var checkNameResponse = storageClient.StorageAccounts.CheckNameAvailability(accountName);

            if (checkNameResponse.IsAvailable)
            {
                CreateStorageAccount(accountName, storageClient, accountType, location);
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

        public void SetBlobHourlyMetrics(
            AzureStorageConnectionStrings azureConnectionString, MetricsLevel metricsLevel = MetricsLevel.Service, int retentionDays = 7)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString.PrimaryConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var blobProperties = cloudBlobClient.GetServiceProperties();

            blobProperties.HourMetrics.MetricsLevel = metricsLevel;
            blobProperties.HourMetrics.RetentionDays = retentionDays;

            cloudBlobClient.SetServiceProperties(blobProperties);
        }

        public void SetBlobMinuteMetrics(
            AzureStorageConnectionStrings azureConnectionString, MetricsLevel metricsLevel = MetricsLevel.Service, int retentionDays = 7)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString.PrimaryConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var blobProperties = cloudBlobClient.GetServiceProperties();

            blobProperties.MinuteMetrics.MetricsLevel = metricsLevel;
            blobProperties.MinuteMetrics.RetentionDays = retentionDays;

            cloudBlobClient.SetServiceProperties(blobProperties);
        }

        public void SetBlobLogging(
            AzureStorageConnectionStrings azureConnectionString, LoggingOperations loggingOperation = (LoggingOperations.Write | LoggingOperations.Delete), int retentionDays = 7)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString.PrimaryConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var blobProperties = cloudBlobClient.GetServiceProperties();

            blobProperties.Logging.LoggingOperations = loggingOperation;
            blobProperties.Logging.RetentionDays = retentionDays;

            cloudBlobClient.SetServiceProperties(blobProperties);
        }
    }
}