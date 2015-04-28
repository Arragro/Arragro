using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Maps.Elevation
{
	[DataContract]
	public class ElevationResponse
	{
		[DataMember(Name = "status")]
		public ServiceResponseStatus Status { get; set; }

		[DataMember(Name = "results")]
		public ElevationResult[] Results { get; set; }
	}
}
