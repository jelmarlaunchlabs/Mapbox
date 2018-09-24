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
using MapBox.Android.Extension;
using System.Collections.Specialized;
using Com.Mapbox.Mapboxsdk.Style.Expressions;
using System.Linq;

[assembly: ExportRenderer(typeof(MapBox.Map), typeof(MapBox.Android.MapboxRenderer))]
namespace MapBox.Android
{
	public partial class MapboxRenderer : ViewRenderer<Map, NView>, IOnMapReadyCallback
	{
		public const string mapLockedPinsKey = nameof(mapLockedPinsKey);
		public const string mapLockedPinsSourceKey = nameof(mapLockedPinsSourceKey);

		public const string normalPinsKey = nameof(normalPinsKey);
		public const string normalPinsSourceKey = nameof(normalPinsSourceKey);

		public const string pin_image_key = nameof(pin_image_key);
		public const string pin_rotation_key = nameof(pin_rotation_key);
		public const string pin_size_key = nameof(pin_size_key);

		private MapViewFragment fragment;
		private MapboxMap nMap;
		private XMapbox.Map xMap;

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
					map.pins.CollectionChanged -= Pins_CollectionChanged;

				//Then remove all pins
				removeAllPins();

				// Unubscribe to changes in map bindable properties
				map.PropertyChanged -= Map_PropertyChanged;
			}
			if (e.NewElement != null) {
				// Subscribe
				var map = e.NewElement;
				xMap = map;

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


			// This will make sure the map is FULLY LOADED and not just ready
			// Because it will not load pins/polylines if the operation is executed immediately
			Device.StartTimer(TimeSpan.FromMilliseconds(100), () => {
				// Add all pin first
				initializePinsLayer();
				addAllReusablePinImages();
				addAllPins();

				// Then subscribe to pins added
				if (xMap.pins != null)
					xMap.pins.CollectionChanged += Pins_CollectionChanged;

				// Subscribe to changes in map bindable properties after all pins are loaded
				xMap.PropertyChanged += Map_PropertyChanged;

				// Temp accuracy marker
				nMap.AddMarker(new MarkerOptions().SetPosition(new Com.Mapbox.Mapboxsdk.Geometry.LatLng(0, 0)));
				return false;
			});
		}

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
				PropertyFactory.IconAnchor("bottom"), // https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-anchor = "bottom" or "center"
				PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True) // Always overlap
			};
			var normalPinsLayer = new SymbolLayer(normalPinsKey, normalPinsSourceKey)
				.WithProperties(normalPinsPropertyValues.ToArray());
			nMap.AddLayer(normalPinsLayer);
		}

		private void addAllReusablePinImages()
		{
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

			var flatPinsSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);
			var normalPinsSource = (GeoJsonSource)nMap.GetSource(normalPinsSourceKey);

			// Just set to empty pins
			flatPinsSource.SetGeoJson(xMap.pins.toFeatureCollection());
			normalPinsSource.SetGeoJson(xMap.pins.toFeatureCollection());

			// Usubscribe each pin to change monitoring
			foreach (var pin in xMap.pins) {
				pin.PropertyChanged -= Pin_PropertyChanged;
			}
		}

		private void addPin(Pin pin)
		{
			// Search for the existing image first
			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var bitmap = nMap.GetImage(key);
			// Get all pins with the same flat value
			var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat);

			// If there are existing image of the same type the reuse
			if (bitmap != null) {
				GeoJsonSource geoJsonSource;
				// Edit the proper source
				if (pin.IsCenterAndFlat)
					geoJsonSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);
				else
					geoJsonSource = (GeoJsonSource)nMap.GetSource(normalPinsSourceKey);

				// Refresh entire source when a pin is added to a specific source
				geoJsonSource.SetGeoJson(pinsWithSimilarKey.toFeatureCollection());
			} else {
				// Otherwise add new

				// New image
				using (var stream = key.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
					var newBitmap = BitmapFactory.DecodeStream(stream);
					nMap.AddImage(key, newBitmap);
				}

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

		private void removePin(Pin pin)
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
			// Only animate flat pins
			if (!pin.IsCenterAndFlat)
				return;

			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var bitmap = nMap.GetImage(key);
			// Get all pins with the same key and same flat value
			var pinsWithSimilarKeyAndNotThisPin = xMap.pins.Where(
				(Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat && arg != pin).toFeature();
			var geoJsonSource = (GeoJsonSource)nMap.GetSource(mapLockedPinsSourceKey);

			pin.previousPinPosition.animatePin(
				pin.position,
				new Xamarin.Forms.Command<double>((d) => {
					var feature = Feature.FromGeometry(
						Com.Mapbox.Geojson.Point.FromLngLat(pin.position.longitude * d,
															pin.position.latitude * d));
					feature.AddStringProperty(MapboxRenderer.pin_image_key, pin.image);
					feature.AddNumberProperty(MapboxRenderer.pin_rotation_key, (Java.Lang.Number)pin.heading);
					feature.AddNumberProperty(MapboxRenderer.pin_size_key, (Java.Lang.Number)pin.imageScaleFactor);

					// Update the feature thats need animating
					pinsWithSimilarKeyAndNotThisPin.Add(feature);

					// Update the entire layer
					geoJsonSource.SetGeoJson(pinsWithSimilarKeyAndNotThisPin.toFeatureCollection());

					// Remove the feature so that there will be no multiple instances of it accross the animation path
					pinsWithSimilarKeyAndNotThisPin.Remove(feature);
				}));
		}
		#endregion

		#region Change detectors
		void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(XMapbox.Map.pins)) {
				// The entire pins collection itself has been changed
				removeAllPins();
			}
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
						removePin(pin);
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
		#endregion

		private void testPolyLine()
		{
			Device.StartTimer(TimeSpan.FromMilliseconds(50), () => {

				var routeCoordinates = new List<Com.Mapbox.Geojson.Point>{
					Com.Mapbox.Geojson.Point.FromLngLat(0,0),
					Com.Mapbox.Geojson.Point.FromLngLat(1,1),
					Com.Mapbox.Geojson.Point.FromLngLat(2,2),
					Com.Mapbox.Geojson.Point.FromLngLat(3,3),
					Com.Mapbox.Geojson.Point.FromLngLat(4,4),
					Com.Mapbox.Geojson.Point.FromLngLat(5,5)
				};

				var lineString = LineString.FromLngLats(routeCoordinates);

				var featureCollection = FeatureCollection.FromFeatures(
					new Feature[] {
						Feature.FromGeometry(lineString)
					});

				var geoJsonSource = new GeoJsonSource("line-source", featureCollection);
				nMap.AddSource(geoJsonSource);

				var innerLineLayer = new LineLayer("innerLineLayer", "line-source")
					.WithProperties(PropertyFactory.LineColor(AGraphics.Color.ParseColor("#e55e5e")),
									PropertyFactory.LineWidth(new Java.Lang.Float(3f)),
									PropertyFactory.LineJoin("round"),
									PropertyFactory.LineCap("round"));
				var outerLineLayer = new LineLayer("outerLineLayer", "line-source")
					.WithProperties(PropertyFactory.LineColor(AGraphics.Color.ParseColor("#5677d8")),
									PropertyFactory.LineWidth(new Java.Lang.Float(6f)),
									PropertyFactory.LineJoin("round"),
									PropertyFactory.LineCap("round"));


				nMap.AddLayer(outerLineLayer);
				nMap.AddLayer(innerLineLayer);
				return false;
			});
		}
	}
}