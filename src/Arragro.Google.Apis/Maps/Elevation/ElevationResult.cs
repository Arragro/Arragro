using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Maps.Elevation
{
    [DataContract]
	public class ElevationResult
	{
        [DataMember(Name = "location")]
		public GeographicPosition Location { get; set; }

        [DataMember(Name = "elevation")]
		public decimal Elevation { get; set; }
	}
}
