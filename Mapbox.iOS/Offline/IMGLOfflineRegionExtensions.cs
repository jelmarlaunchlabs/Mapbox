using Mapbox;
using MapBox.Models;
using MapBox.Offline;
using Mapbox.iOS.Extensions;

namespace MapBox.iOS.Offline
{
	public static class IMGLOfflineRegionExtensions
	{
		public static OfflinePackRegion ToFormsRegion(this IMGLOfflineRegion region)
		{
			if (region == null) return null;
			var output = new OfflinePackRegion();
			if (region is MGLTilePyramidOfflineRegion tpoRegion) {
				output.Bounds = new Bounds(tpoRegion.Bounds.sw.toFormsPosition(), tpoRegion.Bounds.ne.toFormsPosition());
				output.MaximumZoomLevel = tpoRegion.MaximumZoomLevel;
				output.MinimumZoomLevel = tpoRegion.MinimumZoomLevel;
				output.StyleURL = tpoRegion.StyleURL?.AbsoluteString;
			}
			return output;
		}
	}

	public static class MGLTilePyramidOfflineRegionExtensions
	{
		public static OfflinePackRegion ToFormsRegion(this MGLTilePyramidOfflineRegion region)
		{
			if (region == null) return null;
			var output = new OfflinePackRegion();
			output.Bounds = new Bounds(region.Bounds.sw.toFormsPosition(), region.Bounds.ne.toFormsPosition());
			output.MaximumZoomLevel = region.MaximumZoomLevel;
			output.MinimumZoomLevel = region.MinimumZoomLevel;
			output.StyleURL = region.StyleURL?.AbsoluteString;
			return output;
		}
	}
}
