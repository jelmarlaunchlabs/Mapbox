using System;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox
{
	public class Pin : BindableObject
	{
		public static readonly BindableProperty imageProperty = BindableProperty.Create(
			nameof(image),
			typeof(string),
			typeof(Pin),
			default(string),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (string)p2;
			}
		);
		public string image {
			get { return (string)GetValue(imageProperty); }
			set { SetValue(imageProperty, value); }
		}

		public static readonly BindableProperty IsCenterAndFlatProperty = BindableProperty.Create(
			nameof(IsCenterAndFlat),
			typeof(bool),
			typeof(Pin),
			default(bool),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (bool)p2;
			}
		);
		public bool IsCenterAndFlat {
			get { return (bool)GetValue(IsCenterAndFlatProperty); }
			set { SetValue(IsCenterAndFlatProperty, value); }
		}

		public static readonly BindableProperty headingProperty = BindableProperty.Create(
			nameof(heading),
			typeof(double),
			typeof(Pin),
			default(double),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (double)p2;
			}
		);

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
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (double)p2;
			}
		);
		/// <summary>
		/// Note this is the base image width, the final width will be decided by setting the factor
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
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (double)p2;
			}
		);
		/// <summary>
		/// Note this is the base image height, the final height will be decided by setting the factor
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
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (double)p2;
			}
		);
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
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				view.previousPinPosition = (Position)p1;
				var newValue = (Position)p2;

			}
		);
		public Position position {
			get { return (Position)GetValue(positionProperty); }
			set { SetValue(positionProperty, value); }
		}
		public Position previousPinPosition { get; set; }

		public Pin()
		{

		}
	}
}
