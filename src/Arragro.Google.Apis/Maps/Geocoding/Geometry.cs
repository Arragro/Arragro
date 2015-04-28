using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Geocoding
{
	[DataContract]
	public class Geometry
	{
		/// <summary>
		/// Contains the geocoded latitude,longitude value. For normal address
		/// lookups, this field is typically the most important.
		/// </summary>
        [DataMember(Name = "location")]
		public GeographicPosition Location { get; set; }

		/// <summary>
		/// Stores additional data about the specified location.
		/// </summary>
        [DataMember(Name = "location_type")]
		public LocationType LocationType { get; set; }

		/// <summary>
		/// Contains the recommended viewport for displaying the returned
		/// result, specified as two latitude,longitude values defining the
		/// southwest and northeast corner of the viewport bounding box.
		/// Generally the viewport is used to frame a result when displaying
		/// it to a user.
		/// </summary>
        [DataMember(Name = "viewport")]
		public Viewport Viewport { get; set; }
	}
}
