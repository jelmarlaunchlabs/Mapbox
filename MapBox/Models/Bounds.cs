using System;
using System.Collections.Generic;

namespace MapBox.Models
{
	public class Bounds
	{
		public Position Center { get; }
		public Position SouthWest { get; }
		public Position NorthEast { get; }
		public Position SouthEast { get { return new Position(SouthWest.latitude, NorthEast.longitude); } }
		public Position NorthWest { get { return new Position(NorthEast.latitude, SouthWest.longitude); } }
		public double WidthDegrees { get { return Math.Abs(NorthEast.longitude - SouthWest.longitude); } }
		public double HeightDegrees { get { return Math.Abs(NorthEast.latitude - SouthWest.latitude); } }

		public static Bounds FromPositions(IEnumerable<Position> positions)
		{
			if (positions == null) {
				throw new ArgumentNullException(nameof(positions));
			}

			var minX = double.MaxValue;
			var minY = double.MaxValue;
			var maxX = double.MinValue;
			var maxY = double.MinValue;
			var isEmpty = true;

			foreach (var p in positions) {
				isEmpty = false;
				minX = Math.Min(minX, p.longitude);
				minY = Math.Min(minY, p.latitude);
				maxX = Math.Max(maxX, p.longitude);
				maxY = Math.Max(maxY, p.latitude);
			}

			if (isEmpty) {
				throw new ArgumentException(@"{nameof(positions)} is empty");
			}

			return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
		}

		public Bounds(Position southWest, Position northEast)
		{
			SouthWest = southWest;
			NorthEast = northEast;
		}

		public Bounds(Position southWest, Position northEast, Position center)
		{
			SouthWest = southWest;
			NorthEast = northEast;
			Center = center;
		}

		public Bounds Including(Position position)
		{
			var minX = Math.Min(SouthWest.longitude, position.longitude);
			var minY = Math.Min(SouthWest.latitude, position.latitude);
			var maxX = Math.Max(NorthEast.longitude, position.longitude);
			var maxY = Math.Max(NorthEast.latitude, position.latitude);

			return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
		}

		public Bounds Including(Bounds other)
		{
			var minX = Math.Min(SouthWest.longitude, other.SouthEast.longitude);
			var minY = Math.Min(SouthWest.latitude, other.SouthWest.latitude);
			var maxX = Math.Max(NorthEast.longitude, other.NorthEast.longitude);
			var maxY = Math.Max(NorthEast.latitude, other.NorthEast.latitude);

			return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
		}

		public bool Contains(Position position)
		{
			return SouthWest.longitude <= position.longitude && position.longitude <= NorthEast.longitude
					&& SouthWest.latitude <= position.latitude && position.latitude <= NorthEast.latitude;
		}
	}
}
