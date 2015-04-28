using System;

namespace Arragro.Google.Apis.Maps.Maps.Elevation
{
	/// <summary>
	/// Provides a request for the Google Maps Elevation web service.
	/// </summary>
	public class ElevationRequest
	{
		/// <summary>
		/// Defines the location(s) on the earth from which to return elevation
		/// data. This parameter takes either a single location as a
		/// comma-separated {latitude,longitude} pair (e.g. "40.714728,-73.998672")
		/// or multiple latitude/longitude pairs passed as an array or as an
		/// encoded polyline.
		/// </summary>
		/// <remarks>Required if path not present.</remarks>
		/// <see cref="http://code.google.com/apis/maps/documentation/elevation/#Locations"/>
		public string Locations { get; set; }

		/// <summary>
		/// Defines a path on the earth for which to return elevation data.
		/// This parameter defines a set of two or more ordered {latitude,
		/// longitude} pairs defining a path along the surface of the earth.
		/// This parameter must be used in conjunction with the samples
		/// parameter.
		/// </summary>
		/// <remarks>Required if locations not present.</remarks>
		/// <see cref="http://code.google.com/apis/maps/documentation/elevation/#Paths"/>
		public string Path { get; set; }

		/// <summary>
		/// specifies the number of sample points along a path for which to return
		/// elevation data. The samples parameter divides the given path into an
		/// ordered set of equidistant points along the path.
		/// </summary>
		/// <remarks>Required if a path is specified.</remarks>
		public string Samples { get; set; }

		/// <summary>
		/// Specifies whether the application requesting elevation data is
		/// using a sensor to determine the user's location. This parameter
		/// is required for all elevation requests.
		/// </summary>
		/// <remarks>Required.</remarks>
		/// <see cref="http://code.google.com/apis/maps/documentation/elevation/#Sensor"/>
		public string Sensor { get; set; }

		internal Uri ToUri()
		{
			var url = "json?"
				.Append("locations=", Locations)
				.Append("path=", Path)
				.Append("samples=", Samples)
				.Append("sensor=", Sensor)
				.TrimEnd('&');

			return new Uri(url, UriKind.Relative);
		}
	}
}
