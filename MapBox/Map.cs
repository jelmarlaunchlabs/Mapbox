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
		public event EventHandler regionChangedIdle;

		public event EventHandler CameraMoveStarted;
		public event EventHandler CameraMoving;
		public event EventHandler<Bounds> CameraIdled;
		public event EventHandler<Position> MapClicked;

		#region Internal properties
		internal Assembly callerAssembly { get; set; }
		internal ObservableCollection<Pin> oldPins { get; set; }
		internal ObservableCollection<Route> oldRoutes { get; set; }
		internal IMapFunctions mapFunctions { get; set; }
		#endregion

		#region Bindable Properties
		public static readonly BindableProperty pinsProperty = BindableProperty.Create(
			nameof(pins),
			typeof(ObservableCollection<Pin>),
			typeof(Map),
			default(ObservableCollection<Pin>),
			BindingMode.OneWay,
			propertyChanged: (bindable, p1, p2) => {
				var view = bindable as Map;
				view.oldPins = (ObservableCollection<Pin>)p1;
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
			BindingMode.OneWay);
		public ICommand pinClickedCommand {
			get { return (ICommand)GetValue(pinClickedCommandProperty); }
			set { SetValue(pinClickedCommandProperty, value); }
		}

		public static readonly BindableProperty currentZoomLevelProperty = BindableProperty.Create(
			nameof(currentZoomLevel),
			typeof(double),
			typeof(Map),
			default(double),
			BindingMode.OneWayToSource);
		/// <summary>
		/// Max zoom level is 22, 25 on iOS (the closest to the ground), OneWayToSource binding only
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
			BindingMode.OneWayToSource);
		/// <summary>
		/// OneWayToSource binding only.
		/// </summary>
		/// <value>The current map center.</value>        public Position currentMapCenter {
			get { return (Position)GetValue(currentMapCenterProperty); }
			set { SetValue(currentMapCenterProperty, value); }
		}

		public static readonly BindableProperty DefaultPinsProperty = BindableProperty.Create(nameof(DefaultPins), typeof(ObservableCollection<DefaultPin>), typeof(Map), default(ObservableCollection<DefaultPin>), BindingMode.OneWay);
		public ObservableCollection<DefaultPin> DefaultPins {
			get { return (ObservableCollection<DefaultPin>)GetValue(DefaultPinsProperty); }
			set { SetValue(DefaultPinsProperty, value); }
		}
		#endregion

		public ICameraPerspective initialCameraUpdate { get; set; }

		public Map()
		{
			callerAssembly = Assembly.GetCallingAssembly();
			this.pins = new ObservableCollection<Pin>();
			this.routes = new ObservableCollection<Route>();
		}

		public void moveMapToRegion(ICameraPerspective cameraPerspective)
		{
			if (cameraPerspective == null || this.mapFunctions == null)
				return;

			this.mapFunctions.updateMapPerspective(cameraPerspective);
		}

		#region Internal methods
		internal void regionChanged()
		{
			regionChangedIdle?.Invoke(this, new EventArgs());
		}

		internal void cameraMoveStarted()
		{
			CameraMoveStarted?.Invoke(this, new EventArgs());
		}

		internal void cameraMoving()
		{
			CameraMoving?.Invoke(this, new EventArgs());
		}

		internal void cameraIdled(Bounds bounds)
		{
			CameraIdled?.Invoke(this, bounds);
		}

		internal void mapClicked(Position position)
		{
			MapClicked?.Invoke(this, position);
		}
		#endregion
	}
}
