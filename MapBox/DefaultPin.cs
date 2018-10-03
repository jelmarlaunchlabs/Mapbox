using System;
using MapBox.Models;
using Xamarin.Forms;

namespace MapBox
{
	/// <summary>
	/// This is only for test pin, (custom pin calibrator and bounds calibrator)
	/// </summary>
	public class DefaultPin : BindableObject
	{
		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(Position), typeof(DefaultPin), default(Position), BindingMode.OneWay);
		public Position Position {
			get { return (Position)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

		public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(DefaultPin), string.Empty, BindingMode.OneWay);
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public DefaultPin()
		{
		}
	}
}
