using System;

namespace Arragro.Azure.Models
{
    public enum SasUriType
    {
        Blob,
        Queue,
        Table
    }

    public class SasUriDetails
    {
        public string AccountName { get; private set; }
        public SasUriType SasUriType { get; private set; }
        public string Uri { get; private set; }
        public string PolicyName { get; private set; }

        public SasUriDetails(
            string accountName, SasUriType sasUriType, 
            string uri, string policyName)
        {
            if (string.IsNullOrEmpty(accountName))
                throw new ArgumentNullException("accountName");
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");
            if (string.IsNullOrEmpty(policyName))
                throw new ArgumentNullException("policyName");

            AccountName = accountName;
            SasUriType = sasUriType;
            Uri = uri;
            PolicyName = policyName;
        }
    }
}
