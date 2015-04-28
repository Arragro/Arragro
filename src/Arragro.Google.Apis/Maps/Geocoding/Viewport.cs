using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Maps.Geocoding
{
	[DataContract]
	public class Viewport
	{
        [DataMember(Name = "southwest")]
		public GeographicPosition Southwest { get; set; }

        [DataMember(Name = "northeast")]
		public GeographicPosition Northeast { get; set; }
	}
}
