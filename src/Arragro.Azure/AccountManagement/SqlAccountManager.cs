using Arragro.Azure.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Sql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.AccountManagement
{
    public class SqlAccountManager : AccountManagementBase
    {
        public SqlAccountManager(string subscriptionId, string certificateName)
            : base(subscriptionId, certificateName) { }

        public ServerCreateResponse CreateSqlAccount(
            string administratorUserName, string administratorPassword, string location = AustralianLocations.AustraliaEast)
        {
            var serverCreateParameters = new ServerCreateParameters(administratorUserName, administratorPassword, location);

            using (var client = CloudContext.Clients.CreateSqlManagementClient(CertificateCloudCredentials))
            {
                var serverResponse = client.Servers.Create(serverCreateParameters);
                if (serverResponse.StatusCode != HttpStatusCode.Created)
                    throw new ApplicationException("Something went wrong!");

                return serverResponse;
            }
        }

        public DatabaseCreateResponse CreateDatabase(
            string serverName, string databaseName, string edition = DatabaseEditions.Basic)
        {
            var databaseCreateParameters = new DatabaseCreateParameters(databaseName);
            databaseCreateParameters.Edition = edition;


            using (var client = CloudContext.Clients.CreateSqlManagementClient(CertificateCloudCredentials))
            {
                var databaseResponse = client.Databases.Create(serverName, databaseCreateParameters);
                if (databaseResponse.StatusCode != HttpStatusCode.Created)
                    throw new ApplicationException("Something went wrong!");
                
                return databaseResponse;
            }
        }

        public FirewallRuleCreateResponse AddFirewallRule(
            string serverName, string ruleName,
            string startIpAddress, string endIpAddress)
        {
            var firewallRuleSettings = new FirewallRuleCreateParameters
            {
                Name = ruleName,
                StartIPAddress = startIpAddress,
                EndIPAddress = endIpAddress
            };

            using (var client = CloudContext.Clients.CreateSqlManagementClient(CertificateCloudCredentials))
            {
                var ruleResponse = client.FirewallRules.Create(serverName, firewallRuleSettings);
                if (ruleResponse.StatusCode == HttpStatusCode.Created)
                    throw new ApplicationException("Something went wrong!");

                return ruleResponse;
            }
        }

        public void DeleteServer(
            string serverName)
        {
            using (var client = CloudContext.Clients.CreateSqlManagementClient(CertificateCloudCredentials))
            {
                var databaseResponse = client.Servers.Delete(serverName);
                if (databaseResponse.StatusCode != HttpStatusCode.OK)
                    throw new ApplicationException("Something went wrong!");
            }
        }

        public void DeleteDatabase(
            string serverName, string databaseName)
        {
            using (var client = CloudContext.Clients.CreateSqlManagementClient(CertificateCloudCredentials))
            {
                var databaseResponse = client.Databases.Delete(serverName, databaseName);
                if (databaseResponse.StatusCode != HttpStatusCode.OK)
                    throw new ApplicationException("Something went wrong!");
            }
        }
    }
}
