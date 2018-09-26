using System;
namespace MapBox.Models
{
	public struct Position
	{
		public double latitude { get; }
		public double longitude { get; }

		public Position(double latitude, double longitude) : this()
		{
			this.latitude = latitude;
			this.longitude = longitude;
		}

		public static bool operator ==(Position p1, Position p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(Position p1, Position p2)
		{
			return p1.Equals(p2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Position)) {
				return false;
			}

			var position = (Position)obj;
			return latitude == position.latitude &&
				   longitude == position.longitude;
		}

		public override int GetHashCode()
		{
			var hashCode = 2125274203;
			hashCode = hashCode * -1521134295 + latitude.GetHashCode();
			hashCode = hashCode * -1521134295 + longitude.GetHashCode();
			return hashCode;
		}
	}
}
