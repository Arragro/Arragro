using System.Runtime.Serialization;

namespace Arragro.Google.Apis.Maps.Geocoding
{
    [DataContract]
	public class AddressComponent
	{
		/// <summary>
		/// The full text description or name of the address component as
		/// returned by the Geocoder.
		/// </summary>
		[DataMember(Name = "long_name")]
		public string LongName { get; set; }

		/// <summary>
		/// An abbreviated textual name for the address component, if available.
		/// For example, an address component for the state of Alaska may have
		/// a short_name of "AK" using the 2-letter postal abbreviation.
		/// </summary>
        [DataMember(Name = "short_name")]
		public string ShortName { get; set; }

		/// <summary>
		/// Indicates the type of address component. This array contains a set
		/// of one or more tags.
		/// </summary>
        [DataMember(Name = "types")]
		public AddressType[] Types { get; set; }
	}
}
