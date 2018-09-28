using System;
using System.Collections.Generic;
using MapBox.Abstractions;
using Xamarin.Forms;

namespace MapBox.Models
{
	public class CoordinatesAndPaddingCameraPerspective : ICameraPerspective
	{
		public bool isAnimated { get; set; }
		public IEnumerable<Position> positions { get; set; }
		public Thickness padding { get; set; }

		internal CoordinatesAndPaddingCameraPerspective(IEnumerable<Position> positions, Thickness padding, bool isAnimated = true)
		{
			this.positions = positions;
			this.padding = padding;
			this.isAnimated = isAnimated;
		}
	}
}
