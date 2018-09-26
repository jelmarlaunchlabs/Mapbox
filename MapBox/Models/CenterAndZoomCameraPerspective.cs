using System;
using MapBox.Abstractions;

namespace MapBox.Models
{
	public class CenterAndZoomCameraPerspective : ICameraPerspective
	{
		public Position position { get; set; }
		public double zoomLevel { get; set; }

		internal CenterAndZoomCameraPerspective(Position position, double zoomLevel)
		{
			this.position = position;
			this.zoomLevel = zoomLevel;
		}
	}
}
