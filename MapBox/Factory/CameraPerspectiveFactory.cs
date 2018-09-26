using System;
using System.Collections.Generic;
using MapBox.Abstractions;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox.Factory
{
	public static class CameraPerspectiveFactory
	{
		public static ICameraPerspective fromCenterAndZoomLevel(Position position, double zoomLevel)
		{
			return new CenterAndZoomCameraPerspective(position, zoomLevel);
		}

		public static ICameraPerspective fromCoordinatesAndPadding(IEnumerable<Position> positions, Thickness padding)
		{
			return new CoordinatesAndPaddingCameraPerspective(positions, padding);
		}
	}
}
