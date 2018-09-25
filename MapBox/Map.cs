using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("MapBox.Android"), InternalsVisibleTo("Mapbox.iOS")]
namespace MapBox
{
    public class Map : View
    {
		internal Assembly callerAssembly { get; set; }
		internal ObservableCollection<Pin> oldPins { get; set; }
		internal ObservableCollection<Route> oldRoutes { get; set; }

		public static readonly BindableProperty pinsProperty = BindableProperty.Create(
			nameof(pins),
			typeof(ObservableCollection<Pin>),
			typeof(Map),
			default(ObservableCollection<Pin>),
			BindingMode.OneWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				view.oldPins = (ObservableCollection<Pin>)p1;
				var newValue = (ObservableCollection<Pin>)p2;
			}
		);
		public ObservableCollection<Pin> pins {
			get { return (ObservableCollection<Pin>)GetValue(pinsProperty); }
			set { SetValue(pinsProperty, value); }
		}

		public static readonly BindableProperty routesProperty = BindableProperty.Create(
			nameof(routes),
			typeof(ObservableCollection<Route>),
			typeof(Map),
			default(ObservableCollection<Route>),
			BindingMode.OneWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				view.oldRoutes = (ObservableCollection<Route>)p1;
				var newValue = (ObservableCollection<Route>)p2;
			}
		);
		public ObservableCollection<Route> routes {
			get { return (ObservableCollection<Route>)GetValue(routesProperty); }
			set { SetValue(routesProperty, value); }
		}

		public static readonly BindableProperty pinClickedCommandProperty = BindableProperty.Create(
			nameof(pinClickedCommand),
			typeof(ICommand),
			typeof(Map),
			default(ICommand),
			BindingMode.OneWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				var newValue = (ICommand)p2;
			}
		);
		public ICommand pinClickedCommand {
			get { return (ICommand)GetValue(pinClickedCommandProperty); }
			set { SetValue(pinClickedCommandProperty, value); }
		}
		public Map()
		{
			callerAssembly = Assembly.GetCallingAssembly();
			//// One time pin set only
			//this.SetValue(pinsProperty, new ObservableCollection<Pin>());
		}
	}
}
