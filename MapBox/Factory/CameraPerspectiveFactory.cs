using System;
using System.Collections.Generic;
using MapBox.Abstractions;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox.Factory
{
	public static class CameraPerspectiveFactory
	{
		/// <summary>
		/// Creates a camera perspective based on a center position and a zoom level.
		/// </summary>
		/// <returns>The CenterAndZoomCameraPerspective instance.</returns>
		/// <param name="position">The center positon.</param>
		/// <param name="zoomLevel">The zoom level max is 22 in Android and 25 in iOS.</param>
		public static ICameraPerspective fromCenterAndZoomLevel(Position position, double zoomLevel)
		{
			return new CenterAndZoomCameraPerspective(position, zoomLevel);
		}

		/// <summary>
		/// Creates a camera perspective based on a list of position that must me shown in the map and Left, Top, Right and Bottom padding.
		/// </summary>
		/// <returns>The CoordinatesAndPaddingCameraPerspective instance.</returns>
		/// <param name="positions">The list of position to show on map.</param>
		/// <param name="padding">The scaled padding, NOTE: the padding is already scaled.</param>
		public static ICameraPerspective fromCoordinatesAndPadding(IEnumerable<Position> positions, Thickness padding)
		{
			return new CoordinatesAndPaddingCameraPerspective(positions, padding);
		}
	}
}
