using System;
using MapBox.Models;
using MapBox.Extensions;

namespace MapBox.Helpers
{
	public class SphericalUtil
	{
		private const double EARTH_RADIUS = GmsMathUtils.EarthRadius;

		/// <summary>
		/// Returns the heading from one LatLng to another LatLng. Headings are
		/// expressed in degrees clockwise from North within the range[-180, 180).
		/// </summary>
		/// <returns>The heading in degrees clockwise from north.</returns>
		public static double computeHeading(Position from, Position to)
		{
			// http://williams.best.vwh.net/avform.htm#Crs
			double fromLat = from.latitude.ToRadian();
			double fromLng = from.longitude.ToRadian();
			double toLat = to.latitude.ToRadian();
			double toLng = to.longitude.ToRadian();
			double dLng = toLng - fromLng;
			double heading = Math.Atan2(
					Math.Sin(dLng) * Math.Cos(toLat),
				Math.Cos(fromLat) * Math.Sin(toLat) - Math.Sin(fromLat) * Math.Cos(toLat) * Math.Cos(dLng));
			return GmsMathUtils.Wrap(heading.ToDegrees(), -180, 180);
		}

		/// <summary>
		/// Returns the LatLng which lies the given fraction of the way between the
		/// origin LatLng and the destination LatLng.
		/// </summary>
		/// <returns>The interpolated LatLng.</returns>
		/// <param name="from">The LatLng from which to start.</param>
		/// <param name="to">The LatLng toward which to travel.</param>
		/// <param name="fraction">A fraction of the distance to travel.</param>
		public static Position interpolate(Position from, Position to, double fraction)
		{
			// http://en.wikipedia.org/wiki/Slerp
			double fromLat = from.latitude.ToRadian();
			double fromLng = from.longitude.ToRadian();
			double toLat = to.latitude.ToRadian();
			double toLng = to.longitude.ToRadian();
			double cosFromLat = Math.Cos(fromLat);
			double cosToLat = Math.Cos(toLat);

			// Computes Spherical interpolation coefficients.
			double angle = computeAngleBetween(from, to);
			double sinAngle = Math.Sin(angle);
			if (sinAngle < 1E-6) {
				return from;
			}
			double a = Math.Sin((1 - fraction) * angle) / sinAngle;
			double b = Math.Sin(fraction * angle) / sinAngle;

			// Converts from polar to vector and interpolate.
			double x = a * cosFromLat * Math.Cos(fromLng) + b * cosToLat * Math.Cos(toLng);
			double y = a * cosFromLat * Math.Sin(fromLng) + b * cosToLat * Math.Sin(toLng);
			double z = a * Math.Sin(fromLat) + b * Math.Sin(toLat);

			// Converts interpolated vector back to polar.
			double lat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
			double lng = Math.Atan2(y, x);
			return new Position(lat.ToDegrees(), lng.ToDegrees());
		}

		/// <summary>
		/// Returns the angle between two LatLngs, in radians. This is the same as the distance
		/// on the unit sphere.
		/// </summary>
		static double computeAngleBetween(Position from, Position to)
		{
			return distanceRadians(from.latitude.ToRadian(), from.longitude.ToRadian(),
								   to.latitude.ToRadian(), to.longitude.ToRadian());
		}

		/// <summary>
		/// Returns distance on the unit sphere; the arguments are in radians.
		/// </summary>
		private static double distanceRadians(double lat1, double lng1, double lat2, double lng2)
		{
			return GmsMathUtils.ArcHav(GmsMathUtils.HavDistance(lat1, lat2, lng1 - lng2));
		}

		/// <summary>
		/// Returns the distance between two LatLngs, in meters.
		/// </summary>
		public static double computeDistanceBetween(Position from, Position to)
		{
			return computeAngleBetween(from, to) * EARTH_RADIUS;
		}
	}
}
