using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Maps.Geocoding
{
	[DataContract]
	public class GeocodingResponse
	{
		[DataMember(Name = "status")]
		public ServiceResponseStatus Status { get; set; }

        [DataMember(Name = "results")]
		public GeocodingResult[] Results { get; set; }
	}
}
