using System;
using System.Runtime.CompilerServices;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox
{
	/// <summary>
	/// IMPORTANT NOTE: For now, succeeding pins in the pins list will inherit the size of the first
	/// pin image if it happens to be the same image source, to override this there is imageScaleFactor (unique for each pin) to temporarily fix this.
	/// </summary>
	public class Pin : BindableObject
	{
		public static readonly BindableProperty imageProperty = BindableProperty.Create(
			nameof(image),
			typeof(string),
			typeof(Pin),
			default(string),
			BindingMode.OneWay);
		public string image {
			get { return (string)GetValue(imageProperty); }
			set { SetValue(imageProperty, value); }
		}

		public static readonly BindableProperty IsCenterAndFlatProperty = BindableProperty.Create(
			nameof(IsCenterAndFlat),
			typeof(bool),
			typeof(Pin),
			default(bool),
			BindingMode.OneWay);
		public bool IsCenterAndFlat {
			get { return (bool)GetValue(IsCenterAndFlatProperty); }
			set { SetValue(IsCenterAndFlatProperty, value); }
		}

		public static readonly BindableProperty headingProperty = BindableProperty.Create(
			nameof(heading),
			typeof(double),
			typeof(Pin),
			default(double),
			BindingMode.OneWay);

		/// <summary>
		/// Ignored if IsCenterAndFlat = false
		/// </summary>
		/// <value>The heading.</value>
		public double heading {
			get { return (double)GetValue(headingProperty); }
			set { SetValue(headingProperty, value); }
		}

		public static readonly BindableProperty widthProperty = BindableProperty.Create(
			nameof(width),
			typeof(double),
			typeof(Pin),
			(double)50,
			BindingMode.OneWay);
		/// <summary>
		/// Note this is the base image width, the final width will be decided by setting the factor
		/// The width is already scaled!!!
		/// </summary>
		/// <value>The width.</value>
		public double width {
			get { return (double)GetValue(widthProperty); }
			set { SetValue(widthProperty, value); }
		}

		public static readonly BindableProperty heightProperty = BindableProperty.Create(
			nameof(height),
			typeof(double),
			typeof(Pin),
			(double)50,
			BindingMode.OneWay);
		/// <summary>
		/// Note this is the base image height, the final height will be decided by setting the factor
		/// The height is already scaled!!!
		/// </summary>
		/// <value>The height.</value>
		public double height {
			get { return (double)GetValue(heightProperty); }
			set { SetValue(heightProperty, value); }
		}

		public static readonly BindableProperty imageScaleFactorProperty = BindableProperty.Create(
			nameof(imageScaleFactor),
			typeof(double),
			typeof(Pin),
			(double)1,
			BindingMode.OneWay);
		/// <summary>
		/// This is the scaling factor, the number in this will decide the final size of the image PER pin.
		/// </summary>
		/// <value>The image scale factor.</value>
		public double imageScaleFactor {
			get { return (double)GetValue(imageScaleFactorProperty); }
			set { SetValue(imageScaleFactorProperty, value); }
		}

		public static readonly BindableProperty positionProperty = BindableProperty.Create(
			nameof(position),
			typeof(Position),
			typeof(Pin),
			default(Position),
			BindingMode.OneWay,
			propertyChanging: (bindable, oldValue, newValue) => {
				var view = bindable as Pin;
				// Need this to be called in PropertyChanging so that it will fire first before anything else
				view.previousPinPosition = (Position)oldValue;

				// Do not request for update if this is the very first update after this pin was put on the map
				if (!view.hasUpdatedOnce)
					view.hasUpdatedOnce = true;
				else
					view.requestForUpdate = true;
			}
		);
		public Position position {
			get { return (Position)GetValue(positionProperty); }
			set { SetValue(positionProperty, value); }
		}

		public static readonly BindableProperty iconOffsetProperty = BindableProperty.Create(
			nameof(iconOffset),
			typeof(Point),
			typeof(Pin),
			new Point(0,0),
			BindingMode.OneWay);
		/// <summary>
		/// Positive values indicate right and down, ignored if IsCenterAndFlat = true
		/// https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-offset
		/// </summary>
		/// <value>The icon offset.</value>
		public Point iconOffset {
			get { return (Point)GetValue(iconOffsetProperty); }
			set { SetValue(iconOffsetProperty, value); }
		}

		internal Position previousPinPosition { get; set; }
		internal bool hasUpdatedOnce { get; set; }
		internal bool requestForUpdate { get; set; }

		public Pin()
		{

		}
	}
}