using MapBox.Models;

namespace MapBox.Offline
{
	public class OfflinePackRegion
	{
		public string StyleURL { get; set; }

		public Bounds Bounds { get; set; }

		public double MaximumZoomLevel { get; set; }

		public double MinimumZoomLevel { get; set; }
	}
}