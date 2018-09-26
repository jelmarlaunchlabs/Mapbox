using System;
using CoreLocation;
using MapBox.Models;

namespace Mapbox.iOS.Extensions
{
	public static class MapExtensions
	{
		public static Position toFormsPosition(this CLLocationCoordinate2D self)
		{
			return new Position(self.Latitude, self.Longitude);
		}

		public static CLLocationCoordinate2D toNativeCLLocationCoordinate2D(this Position self)
		{
			return new CLLocationCoordinate2D(self.latitude, self.longitude);
		}
	}
}
