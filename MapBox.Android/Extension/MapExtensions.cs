using System;
using Com.Mapbox.Mapboxsdk.Geometry;
using MapBox.Models;

namespace MapBox.Android.Extension
{
	public static class MapExtensions
	{
		public static Position toFormsPostion(this LatLng self)
		{
			return new Position(self.Latitude, self.Longitude);
		}

		public static LatLng toNativeLatLng(this Position self)
		{
			return new LatLng(self.latitude, self.longitude);
		}
	}
}
