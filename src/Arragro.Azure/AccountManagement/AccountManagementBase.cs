using Microsoft.WindowsAzure;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Azure.AccountManagement
{
    public class AccountManagementBase
    {
        protected readonly CertificateCloudCredentials CertificateCloudCredentials = null;

        /// <summary>
        /// Certificate Name must exist in the Certificate Store under the Current User\Personal\Certificates
        /// for this method to work.  Exception will be thrown if no certificate is found.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="certificateName"></param>
        /// <returns></returns>
        public AccountManagementBase(string subscriptionId, string certificateName)
        {
            var store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
            X509Certificate2 certificate = null;
            foreach(var cert in certificates)
                if (cert.SubjectName.Name.EndsWith(certificateName))
                    certificate = cert;
            if (certificate == null)
                throw new ApplicationException(
                    string.Format("Certificate '{0}' was not found in the local users Personel Certificate store.", certificateName));

            CertificateCloudCredentials = new CertificateCloudCredentials(subscriptionId, certificate);
        }
    }
}
