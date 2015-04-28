using Arragro.Azure.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.ServiceManagement.Storage
{
    public class QueueStorageManager
    {
        private readonly string _connectionString;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudQueueClient _cloudQueueClient;

        public QueueStorageManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))

            _connectionString = connectionString;
            _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudQueueClient = _cloudStorageAccount.CreateCloudQueueClient();

            var blobProperties = _cloudQueueClient.GetServiceProperties();
        }

        public CloudQueue GetQueue(string queueName)
        {
            return _cloudQueueClient.GetQueueReference(queueName);
        }

        public CloudQueue CreateQueue(string queueName)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            return queue;
        }

        public bool DeleteQueue(string queueName)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            return queue.DeleteIfExists();
        }

        public static SasUriDetails CreateAndGetQueueSharedAccessSignatureUri(
            string accountName,
            CloudQueue queue,
            string policyName,
            int sharedAccessExpiryTimeInHours = 24,
            SharedAccessQueuePermissions sharedAccessQueuePermissions = SharedAccessQueuePermissions.Add)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            var sharedPolicy = new SharedAccessQueuePolicy();
            sharedPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(sharedAccessExpiryTimeInHours);
            sharedPolicy.Permissions = sharedAccessQueuePermissions;

            //Get the container's existing permissions.
            var permissions = new QueuePermissions();

            //Add the new policy to the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            queue.SetPermissions(permissions);       

            //Return the URI string for the container, including the SAS token.
            return GetQueueSharedAccessSignatureUri(accountName, queue, policyName);
        }

        public static SasUriDetails GetQueueSharedAccessSignatureUri(
            string accountName,
            CloudQueue queue,
            string policyName)
        {
            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            var sasContainerToken = queue.GetSharedAccessSignature(null, policyName);

            //Return the URI string for the container, including the SAS token.
            return new SasUriDetails(accountName, SasUriType.Queue, queue.Uri + sasContainerToken, policyName);

        }

        public void AddMessageToQueue(string queueName, string message)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            var queueMessage = new CloudQueueMessage(message);
            queue.AddMessage(queueMessage);
        }

        public static void AddMessageToQueue(Uri uri, string message)
        {
            var queue = new CloudQueue(uri);
            var queueMessage = new CloudQueueMessage(message);
            queue.AddMessage(queueMessage);
        }

        public CloudQueueMessage PeekAtNextMessage(string queueName)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            return queue.PeekMessage();
        }

        public void UpdateMessage(
            string queueName, string message, 
            TimeSpan? visibilityTimeOut = null, 
            MessageUpdateFields messageUpdateFields = MessageUpdateFields.Content | MessageUpdateFields.Visibility)
        {
            if (visibilityTimeOut == null)
                visibilityTimeOut = TimeSpan.FromSeconds(0.0);

            var queue = _cloudQueueClient.GetQueueReference(queueName);

            var queueMessage = queue.GetMessage();

            queueMessage.SetMessageContent(message);
            queue.UpdateMessage(
                queueMessage,
                visibilityTimeOut.Value,
                messageUpdateFields);
        }

        public CloudQueueMessage DeQueue(string queueName)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            return queue.GetMessage();
        }

        public void DeleteQueueMessage(string queueName, CloudQueueMessage queueMessage)
        {
            var queue = _cloudQueueClient.GetQueueReference(queueName);
            queue.DeleteMessage(queueMessage);
        }

    }
}
