using System;

namespace Arragro.Google.Apis.Maps.Elevation
{
	/// <summary>
	/// The Elevation service provides elevation data for all locations on the
	/// surface of the earth, including depth locations on the ocean floor
	/// (which return negative values). In those cases where Google does not
	/// possess exact elevation measurements at the precise location you
	/// request, the service will interpolate and return an averaged value
	/// using the four nearest locations.
	/// </summary>
	/// <see cref="http://code.google.com/apis/maps/documentation/elevation/"/>
	public static class ElevationService
	{
		public static readonly Uri ApiUrl =
			new Uri("http://maps.google.com/maps/api/elevation/");

		/// <summary>
		/// Sends the specified request to the Google Maps Elevation web
		/// service and parses the response as an ElevationResponse
		/// object.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static ElevationResponse GetResponse(ElevationRequest request)
		{
			var url = new Uri(ApiUrl, request.ToUri());
			return Http.Get(url).As<ElevationResponse>();
		}
	}
}
