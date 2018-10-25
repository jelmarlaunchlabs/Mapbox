using System;
using MapBox.Abstractions;
using MapBox.Android.DependecyService;
using Xamarin.Forms;
using Droid = Android;

[assembly: Dependency(typeof(DisplayMetricsImplementation))]
namespace MapBox.Android.DependecyService
{
	public class DisplayMetricsImplementation : IDisplayMetrics
	{
		Droid.Content.Context context = Droid.App.Application.Context;

		public double getHeight()
		{
			var nativeScale = getNativeScale();
			
            var screenHeight = context.Resources.DisplayMetrics.HeightPixels / nativeScale;

            int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                screenHeight = screenHeight - (context.Resources.GetDimensionPixelSize(resourceId) / nativeScale);
            }

            return screenHeight;
		}

		public double getNativeScale()
		{
			return context.Resources.DisplayMetrics.Density;
		}

		public double getWidth()
		{
			var nativeScale = getNativeScale();
			return context.Resources.DisplayMetrics.WidthPixels / nativeScale;
		}
	}
}
