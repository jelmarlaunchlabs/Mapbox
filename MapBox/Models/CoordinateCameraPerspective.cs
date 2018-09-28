using System;
using MapBox.Abstractions;

namespace MapBox.Models
{
	public class CoordinateCameraPerspective : ICameraPerspective
	{
		public bool isAnimated { get; set; }
		public Position position { get; set; }

		internal CoordinateCameraPerspective(Position position, bool isAnimated = true)
		{
			this.position = position;
			this.isAnimated = isAnimated;
			this.isAnimated = isAnimated;
		}
	}
}
