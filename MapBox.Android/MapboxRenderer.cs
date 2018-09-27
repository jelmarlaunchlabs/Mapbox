using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Widget;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MapBox.Extensions;
using Com.Mapbox.Mapboxsdk.Style.Layers;
using Com.Mapbox.Geojson;
using Com.Mapbox.Mapboxsdk.Style.Sources;
using System.Collections.Generic;
using NView = Android.Views.View;
using AGraphics = Android.Graphics;
using XMapbox = MapBox;
using MapBox.Android.Extensions;
using System.Collections.Specialized;
using Com.Mapbox.Mapboxsdk.Style.Expressions;
using System.Linq;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Camera;
using MapBox.Android.Extension;
using MapBox.Abstractions;
using MapBox.Models;
using MapBox.Helpers;

[assembly: ExportRenderer(typeof(MapBox.Map), typeof(MapBox.Android.MapboxRenderer))]
namespace MapBox.Android
{
	public partial class MapboxRenderer : ViewRenderer<Map, NView>, IOnMapReadyCallback, MapboxMap.IOnMapClickListener, IMapFunctions
	{
		#region Pin constants
		public const string mapLockedPinsKey = nameof(mapLockedPinsKey);
		public const string mapLockedPinsSourceKey = nameof(mapLockedPinsSourceKey);

		public const string normalPinsKey = nameof(normalPinsKey);
		public const string normalPinsSourceKey = nameof(normalPinsSourceKey);

		public const string pin_image_key = nameof(pin_image_key);
		public const string pin_rotation_key = nameof(pin_rotation_key);
		public const string pin_size_key = nameof(pin_size_key);
		public const string pin_offset_key = nameof(pin_offset_key);
		#endregion

		#region Route constants
		// The univesal source of all polylines
		public const string line_source_key = nameof(line_source_key);

		public const string border_line_color_key = nameof(border_line_color_key);
		public const string border_line_width_key = nameof(border_line_width_key);

		public const string line_color_key = nameof(line_color_key);
		public const string line_width_key = nameof(line_width_key);

		public const string border_line_layer_key = nameof(border_line_layer_key);
		public const string line_layer_key = nameof(line_layer_key);
		#endregion

		private MapViewFragment fragment;
		private MapboxMap nMap;
		private XMapbox.Map xMap;
		private bool isPinAnimating;

		public static void init(Context context, string accessToken)
		{
			// Initialize the token
			Com.Mapbox.Mapboxsdk.Mapbox.GetInstance(context, accessToken);
		}

		public MapboxRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);
			if (Control == null) {
				// Set the control
				initializeControl();
			}
			if (e.OldElement != null) {
				// Unsubscribe
				var map = e.NewElement;

				// Unsubscribe to changes in the collection first
				if (map.pins != null)
				{
					map.pins.CollectionChanged -= Pins_CollectionChanged;
					map.DefaultPins.CollectionChanged -= DefaultPins_CollectionChanged;
				}

				// Unsubscribe to changes in the collection first
				if (map.routes != null)
					map.routes.CollectionChanged -= Routes_CollectionChanged;

				// Native map unsubscribe
				nMap.CameraIdle -= NMap_CameraIdle;

				removeAllPins();

				removeAllRoutes();

				// Unubscribe to changes in map bindable properties
				map.PropertyChanged -= Map_PropertyChanged;
			}
			if (e.NewElement != null) {
				// Subscribe
				var map = e.NewElement;
				xMap = map;
				xMap.mapFunctions = this;

				// Additional operations will be handled in OnMapReady to apply delay
			}
		}

		private void initializeControl()
		{
			var activity = (AppCompatActivity)Context;
			var view = new FrameLayout(activity) {
				Id = GenerateViewId()
			};

			SetNativeControl(view);

			fragment = new MapViewFragment();

			activity.SupportFragmentManager.BeginTransaction()
					.Replace(view.Id, fragment)
					.CommitAllowingStateLoss();
			fragment.GetMapAsync(this);
		}

		public void OnMapReady(MapboxMap mapBox)
		{
			nMap = mapBox;
			nMap.SetStyle("mapbox://styles/mapbox/streets-v9");
			//nMap.SetStyle("mapbox://styles/mapbox/light-v9");
			nMap.UiSettings.CompassEnabled = false;
			nMap.AddOnMapClickListener(this);
			nMap.CameraIdle += NMap_CameraIdle;
			nMap.CameraMoveStarted += NMap_CameraMoveStarted;
			nMap.CameraMove += NMap_CameraMove;

			if (xMap?.initialCameraUpdate != null)
				updateMapPerspective(xMap.initialCameraUpdate);

			// This will make sure the map is FULLY LOADED and not just ready
			// Because it will not load pins/polylines if the operation is executed immediately
			Device.StartTimer(TimeSpan.FromMilliseconds(800), () => {
				// Initialize route first so that it will be the first in the layer list z-index = 0
				initializeRoutesLayer();
				addAllRoutes();

				// Add all pin first
				initializePinsLayer();
				addAllReusablePinImages();
				addAllPins();

				// Then subscribe to pins added
				if (xMap.pins != null)
				{
					xMap.pins.CollectionChanged += Pins_CollectionChanged;
					xMap.DefaultPins.CollectionChanged += DefaultPins_CollectionChanged;
				}

				if(xMap.routes!=null)
					xMap.routes.CollectionChanged += Routes_CollectionChanged;

				// Subscribe to changes in map bindable properties after all pins are loaded
				xMap.PropertyChanged += Map_PropertyChanged;
				return false;
			});
		}

		#region Route initializers
		private void initializeRoutesLayer()
		{
			// Use one source only
			var borderLinesLayer = new LineLayer(border_line_layer_key, line_source_key)
				.WithProperties(
					PropertyFactory.LineColor(Expression.Get(border_line_color_key)),
					PropertyFactory.LineWidth(Expression.Get(border_line_width_key)),
					PropertyFactory.LineJoin("round"),
					PropertyFactory.LineCap("round"));

			var linesLayer = new LineLayer(line_layer_key, line_source_key)
				.WithProperties(
					PropertyFactory.LineColor(Expression.Get(line_color_key)),
					PropertyFactory.LineWidth(Expression.Get(line_width_key)),
					PropertyFactory.LineJoin("round"),
					PropertyFactory.LineCap("round"));

			nMap.AddLayer(borderLinesLayer);
			nMap.AddLayer(linesLayer);
		}

		private void addAllRoutes()
		{
			if (xMap.routes == null)
				return;

			// Subscribe all routes to their respective changes
			foreach (var route in xMap.routes)
				route.PropertyChanged += Route_PropertyChanged;

			var geoJsonSource = new GeoJsonSource(line_source_key, xMap.routes.toFeatureCollection());
			nMap.AddSource(geoJsonSource);
		}
		#endregion

		#region Post route initialization route operations
		private void removeAllRoutes()
		{
			if (xMap.routes == null)
				return;

			var routeSource = (GeoJsonSource)nMap.GetSource(line_source_key);

			// Just set to empy route, this will also update with the latest route and the old routes removed
			routeSource.SetGeoJson(xMap.routes.toFeatureCollection());

			// Unsubscribe all routes
			if (xMap.oldRoutes != null)
				foreach (var route in xMap.oldRoutes)
					route.PropertyChanged -= Route_PropertyChanged;

			// Subscribe new routes
			foreach (var route in xMap.routes)
				route.PropertyChanged += Route_PropertyChanged;
		}

		private void updateRouteCollection()
		{
			if (xMap.routes == null)
				return;

			var routeSource = (GeoJsonSource)nMap.GetSource(line_source_key);

			// Update the route
			routeSource.SetGeoJson(xMap.routes.toFeatureCollection());
		}
		#endregion

		#region Route change detectors
		void Routes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// No move and replace support yet
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (Route route in e.NewItems)
						route.PropertyChanged += Route_PropertyChanged;
					updateRouteCollection();
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Route route in xMap.routes)
						route.PropertyChanged -= Route_PropertyChanged;
					updateRouteCollection();
					break;
				case NotifyCollectionChangedAction.Reset:
					removeAllRoutes();
					break;
			}
		}

		void Route_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// TODO: Implement
		}
		#endregion

		#region Pin initializers
		/// <summary>
		/// NOTE: According to documentation, as of the moment, the icon-rotation-alignment does not support data-driven-styling yet
		/// https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-rotation-alignment
		/// </summary>
		private void initializePinsLayer()
		{
			// Pins with bearings layer - configure elements
			var mapLockedPinsPropertyValues = new List<PropertyValue>{
				PropertyFactory.IconImage("{"+pin_image_key+"}"), //Tokenize the field
				PropertyFactory.IconRotate(Expression.Get(pin_rotation_key)),
				PropertyFactory.IconSize(Expression.Get(pin_size_key)),
				PropertyFactory.IconRotationAlignment("map"), // Finally the map-lock flat = "map"
				PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True) // Always overlap
			};
			var mapLockedPinsLayer = new SymbolLayer(mapLockedPinsKey, mapLockedPinsSourceKey)
				.WithProperties(mapLockedPinsPropertyValues.ToArray());
			nMap.AddLayer(mapLockedPinsLayer);

			// Normal pins layer - configure elements
			var normalPinsPropertyValues = new List<PropertyValue>{
				PropertyFactory.IconImage("{"+pin_image_key+"}"), //Tokenize the field
				PropertyFactory.IconSize(Expression.Get(pin_size_key)),
				PropertyFactory.IconOffset(Expression.Get(pin_offset_key)), //https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-offset
				PropertyFactory.IconAnchor("bottom"), // https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-anchor = "bottom" or "center"
				PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True) // Always overlap
			};
			var normalPinsLayer = new SymbolLayer(normalPinsKey, normalPinsSourceKey)
				.WithProperties(normalPinsPropertyValues.ToArray());
			nMap.AddLayer(normalPinsLayer);
		}

		private void addAllReusablePinImages()
		{
			if (xMap.pins == null)
				return;

			foreach (var pin in xMap.pins) {
				var bitmap = nMap.GetImage(pin.image);

				// If any existing item does not yet exist
				if (bitmap == null) {
					// Then add new image
					using (var stream = pin.image.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
						var newBitmap = BitmapFactory.DecodeStream(stream);
						nMap.AddImage(pin.image, newBitmap);
					}
				}
			}
		}

		// Initial batch uodate
		private void addAllPins()
		{
			if (xMap.pins == null)
				return;

			// Subscribe all pins to their respective changes
			foreach (var pin in xMap.pins)
				pin.PropertyChanged += Pin_PropertyChanged;

			// Add the mapLocked pins first
			// Select all pins where it has heading setter activated
			var mapLockedFeatureCollection = xMap.pins.Where((Pin pin) => pin.IsCenterAndFlat).toFeatureCollection();

			var mapLockedGeoJsonSource = new GeoJsonSource(mapLockedPinsSourceKey, mapLockedFeatureCollection);
			nMap.AddSource(mapLockedGeoJsonSource);

			// Add the normal pins after
			// Select all pins where it has heading disabled
			var normalFeatureCollection = xMap.pins.Where((Pin pin) => !pin.IsCenterAndFlat).toFeatureCollection();

			var normalGeoJsonSource = new GeoJsonSource(normalPinsSourceKey, normalFeatureCollection);
			nMap.AddSource(normalGeoJsonSource);
		}
		#endregion

		#region Post pin initialization pin operations
		private void removeAllPins()
		{
			if (xMap.pins == null)
				return;
			var mapLockedFeatureCollection = xMap.pins.Where((Pin pin) => pin.IsCenterAndFlat);
			var normalFeatureCollection = xMap.pins.Where((Pin pin) => !pin.IsCenterAndFlat);

			var flatPinsSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);
			var normalPinsSource = (GeoJsonSource)nMap.GetSource(normalPinsSourceKey);

			// Just set to empty pins, this will also refreshes with the latest pin and the old pins removed
			flatPinsSource.SetGeoJson(mapLockedFeatureCollection.toFeatureCollection());
			normalPinsSource.SetGeoJson(normalFeatureCollection.toFeatureCollection());

			// Usubscribe each pin to change monitoring
			if (xMap.oldPins != null)
				foreach (var pin in xMap.oldPins)
					pin.PropertyChanged -= Pin_PropertyChanged;

			// Subcribe each new pin to change monitoring
			foreach (var pin in xMap.pins)
				pin.PropertyChanged += Pin_PropertyChanged;
		}

		private void addPin(Pin pin)
		{
			// Search for the existing image first
			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var bitmap = nMap.GetImage(key);

			// If there are existing image of the same type the reuse
			// Note, this is just removePin(Pin), but left this way because there might be a restructure for this in the fututre
			if (bitmap != null)
				updatePins(pin);
			else {
				// Otherwise add new

				// New image
				using (var stream = key.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
					var newBitmap = BitmapFactory.DecodeStream(stream);
					nMap.AddImage(key, newBitmap);
				}

				updatePins(pin);
			}
		}

		/// <summary>
		/// This method updates the pins to the current pin count, and includes all the pin property updates
		/// </summary>
		/// <param name="pin">Pin.</param>
		private void updatePins(Pin pin)
		{
			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var bitmap = nMap.GetImage(key);
			// Get all pins with the same key and same flat value
			var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat);

			// Use the existing values in map
			if (bitmap != null) {
				GeoJsonSource geoJsonSource;

				// Edit the proper source
				if (pin.IsCenterAndFlat)
					geoJsonSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);
				else
					geoJsonSource = (GeoJsonSource)nMap.GetSource(normalPinsSourceKey);

				// Refresh entire source when a pin is added to a specific source
				geoJsonSource.SetGeoJson(pinsWithSimilarKey.toFeatureCollection());
			}
		}

		private void animateLocationChange(Pin pin)
		{
			#region Simultaneous pin update
			System.Threading.Tasks.Task.Run(async () => {
				// Only animate flat pins
				if (!pin.IsCenterAndFlat || isPinAnimating)
					return;

				// Wait for pin position assignment in the same call
				await System.Threading.Tasks.Task.Delay(10);
				isPinAnimating = true;

				// Get all pins with the same key and same flat value (animatable pins)
				var pinsWithSimilarKey = xMap.pins.Where(
					(Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat).ToArray();

				Device.BeginInvokeOnMainThread(() => {
					var geoJsonSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);

					// Update the entire frame
					MapBox.Extensions.MapExtensions.animatePin(
						(double d) => {
							System.Threading.Tasks.Task.Run(() => {
								var movablePinCount = pinsWithSimilarKey.Count();
								var features = new List<Feature>();

								for (int i = 0; i < movablePinCount; i++) {
									var p = pinsWithSimilarKey[i];
									Position theCurrentAnimationJump = p.position;
									double theCurrentHeading = p.heading;

									// Only update the pin if it is the pin that cause this animation call
									// OR the if it actually requested for a position update before this current animation has not been finished
									if (pin == p || p.requestForUpdate) {
										theCurrentAnimationJump = SphericalUtil.interpolate(p.previousPinPosition, p.position, d);
										theCurrentHeading = SphericalUtil.computeHeading(p.previousPinPosition, p.position);
										p.heading = theCurrentHeading;
									}

									var feature = Feature.FromGeometry(
										Com.Mapbox.Geojson.Point.FromLngLat(theCurrentAnimationJump.longitude,
																			theCurrentAnimationJump.latitude));
									feature.AddStringProperty(MapboxRenderer.pin_image_key, p.image);
									feature.AddNumberProperty(MapboxRenderer.pin_rotation_key, (Java.Lang.Number)theCurrentHeading);
									feature.AddNumberProperty(MapboxRenderer.pin_size_key, (Java.Lang.Number)p.imageScaleFactor);

									// Add to the new animation frame
									features.Add(feature);
								}

								// Extension method bypass, fromFeatures accepts IList as parameter
								var featureCollection = FeatureCollection.FromFeatures(features);

								// Update the entire layer
								Device.BeginInvokeOnMainThread(() => geoJsonSource.SetGeoJson(featureCollection));
							});
						},
						(d, b) => {
							isPinAnimating = false;

							// Stabilize the pins, at this moment all the pins are updated
							foreach (var p in pinsWithSimilarKey)
								p.requestForUpdate = false;
						}, 500);
				});
			});
			#endregion
		}
		#endregion

		#region Change detectors
		void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == XMapbox.Map.pinsProperty.PropertyName)
				// The entire pins collection itself has been changed
				removeAllPins();
			else if (e.PropertyName == XMapbox.Map.routesProperty.PropertyName)
				removeAllRoutes();
		}

		void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// No move and replace support yet
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (Pin pin in e.NewItems) {
						pin.PropertyChanged += Pin_PropertyChanged;
						addPin(pin);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Pin pin in e.OldItems) {
						pin.PropertyChanged -= Pin_PropertyChanged;
						updatePins(pin);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					removeAllPins();
					break;
			}
		}

		void Pin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var pin = (Pin)sender;
			if (e.PropertyName == Pin.positionProperty.PropertyName)
				animateLocationChange(pin);
		}

		void DefaultPins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach(DefaultPin pin in e.NewItems)
					{
						nMap.AddMarker(new MarkerOptions().SetPosition(pin.Position.toNativeLatLng()).SetTitle(pin.Title));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					//nMap.RemoveMarker();
					break;
				case NotifyCollectionChangedAction.Reset:
					nMap.Markers.All((arg) => { nMap.RemoveMarker(arg); return true; });
					break;
			}
		}
		#endregion

		#region Map functions
		public void OnMapClick(LatLng point)
		{
			var mapLockedPinsLayer = (SymbolLayer)nMap.GetLayer(mapLockedPinsKey);
			var normalPinsLayer = (SymbolLayer)nMap.GetLayer(normalPinsKey);

			var pixel = nMap.Projection.ToScreenLocation(point);
			var features = nMap.QueryRenderedFeatures(pixel, mapLockedPinsKey, normalPinsKey);

			System.Diagnostics.Debug.WriteLine($"Features: {features}");

			// TODO: Return the actual pin clicked
			if (features.Any() && xMap.pinClickedCommand != null)
			{
				xMap.pinClickedCommand.Execute(null);
			}
			else
			{
				xMap.mapClicked(point.toFormsPostion());
			}	
		}
		#endregion

		void NMap_CameraMoveStarted(object sender, MapboxMap.CameraMoveStartedEventArgs e)
		{
			xMap.cameraMoveStarted();
		}

		void NMap_CameraMove(object sender, EventArgs e)
		{
			xMap.cameraMoving();
		}

		void NMap_CameraIdle(object sender, EventArgs e)
		{
			xMap.currentZoomLevel = nMap.CameraPosition.Zoom;
			xMap.currentMapCenter = nMap.CameraPosition.Target.toFormsPostion();
			xMap.regionChanged();

			xMap.cameraIdled(new Bounds(nMap.Projection.VisibleRegion.NearLeft.toFormsPostion(), nMap.Projection.VisibleRegion.FarRight.toFormsPostion(), nMap.CameraPosition.Target.toFormsPostion()));
		}

		public void updateMapPerspective(ICameraPerspective cameraPerspective)
		{
			if (nMap == null)
				return;

			if (cameraPerspective is CenterAndZoomCameraPerspective centerAndZoom) {
				nMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(centerAndZoom.position.toNativeLatLng(), centerAndZoom.zoomLevel));
			} else if (cameraPerspective is CoordinatesAndPaddingCameraPerspective span) {
				var density = Resources.DisplayMetrics.Density;
				var list = span.positions.Select((Position p) => new LatLng(p.latitude, p.longitude));
				var builder = new LatLngBounds.Builder();
				builder.Includes(list.ToList());

				var camera = nMap.GetCameraForLatLngBounds(
					builder.Build(),
					new int[]{
						(int)(span.padding.Left * density),
						(int)(span.padding.Top * density),
						(int)(span.padding.Right * density),
						(int)(span.padding.Bottom * density)
					});
				nMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(camera));
			}
		}
	}
}