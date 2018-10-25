using System;
using Mapbox.iOS.DependencyService;
using MapBox.Abstractions;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayMetricsImplementation))]
namespace Mapbox.iOS.DependencyService
{
	public class DisplayMetricsImplementation : IDisplayMetrics
	{
		public double getHeight()
		{
            var screenHeight = (float)UIScreen.MainScreen.Bounds.Height;

            UIWindow w = new UIWindow();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (w.SafeAreaInsets.Top > 0 && w.SafeAreaInsets.Bottom > 0)
                {
                    screenHeight = 667.0f;
                }
            }

            return screenHeight;
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
