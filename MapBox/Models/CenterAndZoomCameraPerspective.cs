using System;
using MapBox.Abstractions;

namespace MapBox.Models
{
	public class CenterAndZoomCameraPerspective : ICameraPerspective
	{
		public bool isAnimated { get; set; }
		public Position position { get; set; }
		public double zoomLevel { get; set; }

		internal CenterAndZoomCameraPerspective(Position position, double zoomLevel, bool isAnimated = true)
		{
			this.position = position;
			this.zoomLevel = zoomLevel;
			this.isAnimated = isAnimated;
		}
	}
}
