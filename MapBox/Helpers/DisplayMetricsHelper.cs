using System;
using MapBox.Abstractions;
using Xamarin.Forms;

namespace MapBox.Helpers
{
	public class DisplayMetricsHelper
	{
		private static DisplayMetricsHelper _instance;
		public static DisplayMetricsHelper instance{
			get{
				if (_instance == null)
					_instance = new DisplayMetricsHelper();
				return _instance;
			}
		}

		public double nativeScale { get; }
		public double screenWidth { get; }
		public double screenHeight { get; }
		public double screenScale { get; }

		private DisplayMetricsHelper()
		{
			var dependency = DependencyService.Get<IDisplayMetrics>();
			nativeScale = dependency.getNativeScale();
			screenWidth = dependency.getWidth();
			screenHeight = dependency.getHeight();
			screenScale = screenWidth / 320.0f;
		}
	}
}
