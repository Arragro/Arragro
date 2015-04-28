using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Maps.Geocoding
{
	[DataContract]
	public class GeocodingResult
	{
		/// <summary>
		/// Indicates the type of the returned result. This array contains a
		/// set of one or more tags identifying the type of feature returned
		/// in the result. For example, a geocode of "Chicago" returns
		/// "locality" which indicates that "Chicago" is a city, and also
		/// returns "political" which indicates it is a political entity.
		/// </summary>
        [DataMember(Name = "types")]
		public AddressType[] Types { get; set; }

		/// <summary>
		/// A string containing the human-readable address of this location.
		/// Often this address is equivalent to the "postal address," which
		/// sometimes differs from country to country. (Note that some
		/// countries, such as the United Kingdom, do not allow distribution
		/// of true postal addresses due to licensing restrictions.) This
		/// address is generally composed of one or more address components.
		/// </summary>
        [DataMember(Name = "formatted_address")]
		public string FormattedAddress { get; set; }

		/// <summary>
		/// An array containing the separate address components.  For example,
		/// the address "111 8th Avenue, New York, NY" contains separate
		/// address components for "111" (the street number, "8th Avenue" (the
		/// route), "New York" (the city) and "NY" (the US state).
		/// </summary>
        [DataMember(Name = "address_components")]
		public AddressComponent[] Components { get; set; }

        [DataMember(Name = "geometry")]
		public Geometry Geometry { get; set; }
	}
}
