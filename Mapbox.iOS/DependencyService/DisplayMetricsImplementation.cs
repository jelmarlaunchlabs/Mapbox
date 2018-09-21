﻿using System;
using MapBox.Abstractions;
using UIKit;

namespace Mapbox.iOS.DependencyService
{
	public class DisplayMetricsImplementation : IDisplayMetrics
	{
		public double getHeight()
		{
			return UIDevice.CurrentDevice.CheckSystemVersion(11, 0) ? 667 : (float)UIScreen.MainScreen.Bounds.Height;
		}

		public double getNativeScale()
		{
			return UIScreen.MainScreen.Scale;
		}

		public double getWidth()
		{
			return (float)UIScreen.MainScreen.Bounds.Width;
		}
	}
}
