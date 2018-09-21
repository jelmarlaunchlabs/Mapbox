using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("MapBox.Android"), InternalsVisibleTo("Mapbox.iOS")]
namespace MapBox
{
    public class Map : View
    {
		internal Assembly callerAssembly { get; set; }

		public static readonly BindableProperty pinsProperty = BindableProperty.Create(
			nameof(pins),
			typeof(ObservableCollection<Pin>),
			typeof(Map),
			default(ObservableCollection<Pin>),
			BindingMode.TwoWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				var newValue = (ObservableCollection<Pin>)p2;
			}
		);
		public ObservableCollection<Pin> pins {
			get { return (ObservableCollection<Pin>)GetValue(pinsProperty); }
			set { SetValue(pinsProperty, value); }
		}

		public Map()
		{
			callerAssembly = Assembly.GetCallingAssembly();
			//// One time pin set only
			//this.SetValue(pinsProperty, new ObservableCollection<Pin>());
		}
	}
}
