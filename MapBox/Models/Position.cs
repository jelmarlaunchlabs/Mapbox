using System;
namespace MapBox.Models
{
	public struct Position
	{
		public double latitude { get; private set; }
		public double longitude { get; private set; }

		public Position(double latitude, double longitude) : this()
		{
			this.latitude = latitude;
			this.longitude = longitude;
		}
	}
}
