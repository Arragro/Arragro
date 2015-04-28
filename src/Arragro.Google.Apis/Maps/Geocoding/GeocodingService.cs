using System;

namespace Arragro.Google.Apis.Maps.Maps.Geocoding
{
	/// <summary>
	/// Provides a direct way to access a geocoder via an HTTP request.
	/// Additionally, the service allows you to perform the converse operation
	/// (turning coordinates into addresses); this process is known as
	/// "reverse geocoding."
	/// </summary>
	public static class GeocodingService
	{
		public static readonly Uri ApiUrl = 
			new Uri("http://maps.google.com/maps/api/geocode/");

		/// <summary>
		/// Sends the specified request to the Google Maps Geocoding web
		/// service and parses the response as an GeocodingResponse
		/// object.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static GeocodingResponse GetResponse(GeocodingRequest request)
		{
			var url = new Uri(ApiUrl, request.ToUri());
			return Http.Get(url).As<GeocodingResponse>();
		}
	}
}
