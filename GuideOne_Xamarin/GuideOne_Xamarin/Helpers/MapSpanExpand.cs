using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace GuideOne_Xamarin.Helpers
{
	public static class MapSpanExpand
	{
		static Position pos;
		static double rad;

		static Position angle1;
		static Position angle2;

		public static bool IsInclude(this MapSpan ms, Position position)
		{
			if (pos == ms.Center && rad == ms.Radius.Meters)
			{
				return angle1.Latitude >= position.Latitude && angle2.Latitude <= position.Latitude
						&& angle1.Longitude <= position.Longitude && angle2.Longitude >= position.Longitude;
			}
			else
			{
				pos = ms.Center;
				rad = ms.Radius.Meters;
				angle2 = new Position(ms.Center.Latitude - 360 * Math.Sin(Math.PI * 0.75) * rad / 40000000
					, ms.Center.Longitude - 360 * Math.Cos(Math.PI * 0.75) * rad / (40075696 * Math.Cos(ms.Center.Latitude / 180.0 * Math.PI)));
				angle1 = new Position(ms.Center.Latitude - 360 * Math.Sin(Math.PI * 1.75) * rad / 40000000
					, ms.Center.Longitude - 360 * Math.Cos(Math.PI * 1.75) * rad / (40075696 * Math.Cos(ms.Center.Latitude / 180.0 * Math.PI)));
				return IsInclude(ms, position);
			}
		}
	}
}
