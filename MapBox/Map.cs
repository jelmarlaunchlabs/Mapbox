using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MapBox.Abstractions;
using MapBox.Models;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("MapBox.Android"), InternalsVisibleTo("Mapbox.iOS")]
namespace MapBox
{
    public class Map : View
    {
		internal Assembly callerAssembly { get; set; }
		internal ObservableCollection<Pin> oldPins { get; set; }
		internal ObservableCollection<Route> oldRoutes { get; set; }
		internal IMapFunctions mapFunctions { get; set; }

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

		public static readonly BindableProperty currentZoomLevelProperty = BindableProperty.Create(
			nameof(currentZoomLevel),
			typeof(double),
			typeof(Map),
			default(double),
			BindingMode.OneWayToSource,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				var newValue = (double)p2;
			}
		);
		/// <summary>
		/// Max zoom level is 22 (the closest to the ground), OneWayToSource binding only
		/// </summary>
		/// <value>The current zoom level.</value>
		public double currentZoomLevel {
			get { return (double)GetValue(currentZoomLevelProperty); }
			set { SetValue(currentZoomLevelProperty, value); }
		}

		public static readonly BindableProperty currentMapCenterProperty = BindableProperty.Create(
			nameof(currentMapCenter),
			typeof(Position),
			typeof(Map),
			default(Position),
			BindingMode.OneWayToSource,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				var newValue = (Position)p2;
			}
		);
		/// <summary>
		/// OneWayToSource binding only.
		/// </summary>
		/// <value>The current map center.</value>		public Position currentMapCenter {
			get { return (Position)GetValue(currentMapCenterProperty); }
			set { SetValue(currentMapCenterProperty, value); }
		}

		public ICameraPerspective initialCameraUpdate { get; set; }

		public Map()
		{
			callerAssembly = Assembly.GetCallingAssembly();
			//// One time pin set only
			//this.SetValue(pinsProperty, new ObservableCollection<Pin>());
		}

		public void moveMapToRegion(ICameraPerspective cameraPerspective)
		{
			if (cameraPerspective == null || this.mapFunctions == null)
				return;

			this.mapFunctions.updateMapPerspective(cameraPerspective);
		}
	}
}
