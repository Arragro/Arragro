namespace Arragro.Google.Apis.Maps.Maps
{
	public enum ServiceResponseStatus
	{
		Unknown,

		/// <summary>
		/// Indicates that no errors occurred; the address was successfully 
		/// parsed and at least one geocode was returned.
		/// </summary>
		Ok,

		/// <summary>
		/// Indicating the service request was malformed.
		/// </summary>
		InvalidRequest,

		/// <summary>
		/// Indicates that the geocode was successful but returned no results.
		/// This may occur if the geocode was passed a non-existent address or
		/// a latlng in a remote location.
		/// </summary>
		ZeroResults,

		/// <summary>
		/// Indicates that you are over your quota.
		/// </summary>
		OverQueryLimit,

		/// <summary>
		/// Indicates that your request was denied, generally because of lack
		/// of a sensor parameter.
		/// </summary>
		RequestDenied
	}
}
