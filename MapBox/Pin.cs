using System;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox
{
	public class Pin : BindableObject
	{
		// ID
		// Image (embeded resource or url)
		// IsCenteredAndFlat
		// Heading
		// Width
		// Height

		public static readonly BindableProperty idProperty = BindableProperty.Create(
			nameof(id),
			typeof(int),
			typeof(Pin),
			default(int),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (int)p2;
			}
		);
		public int id {
			get { return (int)GetValue(idProperty); }
			set { SetValue(idProperty, value); }
		}

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
		/// Note, for now, the width will be decided on the width of the first added pin of a certain image
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
		/// Note, for now, the height will be decided on the height of the first added pin of a certain image
		/// </summary>
		/// <value>The width.</value>
		public double height {
			get { return (double)GetValue(heightProperty); }
			set { SetValue(heightProperty, value); }
		}

		public static readonly BindableProperty positionProperty = BindableProperty.Create(
			nameof(position),
			typeof(Position),
			typeof(Pin),
			default(Position),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Pin;
				var newValue = (Position)p2;
			}
		);
		public Position position {
			get { return (Position)GetValue(positionProperty); }
			set { SetValue(positionProperty, value); }
		}

		public Pin()
		{

		}
	}
}
