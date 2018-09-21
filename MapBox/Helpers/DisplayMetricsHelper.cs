using System;
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

		public double nativeScale { get; set; }
		public double screenWidth { get; set; }
		public double screenHeight { get; set; }
		public double screenScale {
			get {
				return screenWidth / 320.0f;
			}
		}

		private DisplayMetricsHelper()
		{
			// Wait.... don't use me
		}
	}
}
