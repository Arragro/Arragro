
using Arragro.Azure.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using System.Collections.Generic;
namespace Arragro.Azure.AccountManagement
{
    public class WebSiteAccountManager : AccountManagementBase
    {
        public WebSiteAccountManager(string subscriptionId, string certificateName) 
            : base(subscriptionId, certificateName) { }

        public WebSiteCreateResponse CreateWebsite(
            string webSiteName, string webSpaceName = AustralianWebSpaceNames.AustraliaEastWebSpace, 
            string hostName = ".azurewebsites.net", string geoRegion = AustralianLocations.AustraliaEast, string webHostingPlan = WebHostingPlanModes.Free)
        {
            using(var webSiteClient = CloudContext.Clients.CreateWebSiteManagementClient(CertificateCloudCredentials))
            {
                WebSiteCreateParameters.WebSpaceDetails webSpaceDetails = new WebSiteCreateParameters.WebSpaceDetails
                {
                    GeoRegion = geoRegion,
                    Plan = WebSpacePlanNames.VirtualDedicatedPlan,
                    Name = webSpaceName
                };



                var webSiteCreateParameters = new WebSiteCreateParameters
                {
                    Name = webSiteName,
                    WebSpace = webSpaceDetails,
                    ServerFarm = webHostingPlan
                };

                return webSiteClient.WebSites.Create(webSpaceName, webSiteCreateParameters);
            }
        }
    }
}
