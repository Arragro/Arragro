using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps
{
    [DataContract]
	public class GeographicPosition
	{
		[DataMember(Name = "lat")]
		public decimal Latitude { get; set; }

        [DataMember(Name = "lng")]
		public decimal Longitude { get; set; }
	}
}
