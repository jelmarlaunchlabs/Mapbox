using System;
using System.Collections.Generic;
using MapBox.Abstractions;
using Xamarin.Forms;

namespace MapBox.Models
{
	public class CoordinatesAndPaddingCameraPerspective : ICameraPerspective
	{
		public IEnumerable<Position> positions { get; set; }
		public Thickness padding { get; set; }

		internal CoordinatesAndPaddingCameraPerspective(IEnumerable<Position> positions, Thickness padding)
		{
			this.positions = positions;
			this.padding = padding;
		}
	}
}
