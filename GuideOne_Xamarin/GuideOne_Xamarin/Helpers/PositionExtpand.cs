using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace GuideOne_Xamarin.Helpers
{
	public static class PositionExtpand
	{
		const double EARTH_RADIUS = 6371.0 ;
		public static double Distance(this Position p1, Position p2)
		{

			// перевести координаты в радианы
			var lat1 = p1.Latitude; //* Math.PI / 180;
			var lat2 = p2.Latitude; //* Math.PI / 180;
			var long1 = p1.Longitude;// * Math.PI / 180;
			var long2 = p2.Longitude;// * Math.PI / 180;
			// косинусы и синусы широт и разницы долгот
			var cl1 = Math.Cos(lat1);
			var cl2 = Math.Cos(lat2);
			var sl1 = Math.Sin(lat1);
			var sl2 = Math.Sin(lat2);
			var delta = long2 - long1;
			var cdelta = Math.Cos(delta);
			var sdelta = Math.Sin(delta);
			
			// вычисления длины большого круга
			var y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
			var x = sl1 * sl2 + cl1 * cl2 * cdelta;
 
			//
			var ad = Math.Atan2(y, x);
			return ad * EARTH_RADIUS;
		}
	}
}
