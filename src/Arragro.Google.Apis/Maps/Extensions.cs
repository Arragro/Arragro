using System;

namespace Arragro.Google.Apis.Maps.Maps
{
	internal static class Extensions
	{
		public static string Append(this string current, string key, string value)
		{
			return String.IsNullOrEmpty(value) ? current : current + key + value + "&";
		}
	}
}
