using System;
using System.Collections.ObjectModel;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox
{
	/// <summary>
	/// IMPORTANT NOTE: For now later changes to the property of the instance of this class will not reflect yet.
	/// </summary>
	public class Route : BindableObject
	{
		public static readonly BindableProperty pointsProperty = BindableProperty.Create(
			nameof(points),
			typeof(ObservableCollection<Position>),
			typeof(Route),
			default(ObservableCollection<Position>),
			BindingMode.OneWay);
		public ObservableCollection<Position> points {
			get { return (ObservableCollection<Position>)GetValue(pointsProperty); }
			set { SetValue(pointsProperty, value); }
		}

		public static readonly BindableProperty borderLineColorProperty = BindableProperty.Create(
			nameof(borderLineColor),
			typeof(string),
			typeof(Route),
			"#000000",
			BindingMode.TwoWay);
		public string borderLineColor {
			get { return (string)GetValue(borderLineColorProperty); }
			set { SetValue(borderLineColorProperty, value); }
		}

		public static readonly BindableProperty borderLineWidthProperty = BindableProperty.Create(
			nameof(borderLineWidth),
			typeof(double),
			typeof(Route),
			(double)6,
			BindingMode.OneWay);
		/// <summary>
		/// This is already scaled in both platforms
		/// </summary>
		/// <value>The width of the border line.</value>
		public double borderLineWidth {
			get { return (double)GetValue(borderLineWidthProperty); }
			set { SetValue(borderLineWidthProperty, value); }
		}

		public static readonly BindableProperty lineColorProperty = BindableProperty.Create(
			nameof(lineColor),
			typeof(string),
			typeof(Route),
			"#000000",
			BindingMode.TwoWay);
		public string lineColor {
			get { return (string)GetValue(lineColorProperty); }
			set { SetValue(lineColorProperty, value); }
		}

		public static readonly BindableProperty lineWidthProperty = BindableProperty.Create(
			nameof(lineWidth),
			typeof(double),
			typeof(Route),
			(double)3,
			BindingMode.OneWay);
		/// <summary>
		/// This is already scaled in both platforms
		/// </summary>
		/// <value>The width of the line.</value>
		public double lineWidth {
			get { return (double)GetValue(lineWidthProperty); }
			set { SetValue(lineWidthProperty, value); }
		}

		public Route()
		{
			points = new ObservableCollection<Position>();
		}
	}
}