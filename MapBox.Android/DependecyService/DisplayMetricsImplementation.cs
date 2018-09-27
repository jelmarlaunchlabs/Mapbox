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
			return context.Resources.DisplayMetrics.HeightPixels / nativeScale;
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
