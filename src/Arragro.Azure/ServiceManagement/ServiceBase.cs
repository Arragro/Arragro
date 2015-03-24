using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.ServiceManagement
{
    public class ServiceManagementBase
    {
        protected readonly CertificateCloudCredentials CertificateCloudCredentials = null;

        /// <summary>
        /// Certificate Name must exist in the Certificate Store under the Current User\Personal\Certificates
        /// for this method to work.  Exception will be thrown if no certificate is found.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="certificateName"></param>
        /// <returns></returns>
        public ServiceManagementBase(string subscriptionId, string certificateName)
        {
            var store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
            X509Certificate2 certificate = null;
            if (certificates.Count > 0)
                certificate = certificates[0];
            if (certificate == null)
                throw new ApplicationException(
                    string.Format("Certificate '{0}' was not found in the local users Personel Certificate store.", certificateName));

            CertificateCloudCredentials = new CertificateCloudCredentials(subscriptionId, certificate);
        }
    }
}
