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

[assembly:ExportRenderer(typeof(MapBox.Map),typeof(MapBox.Android.MapboxRenderer))]
namespace MapBox.Android
{
	public partial class MapboxRenderer : ViewRenderer<Map, NView>, IOnMapReadyCallback
	{
		public const string pin_image_key = nameof(pin_image_key);
		public const string pin_rotation_method_key = nameof(pin_rotation_method_key);
		public const string pin_rotation_key = nameof(pin_rotation_key);
		public const string pin_anchor_key = nameof(pin_anchor_key);

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
			Device.StartTimer(TimeSpan.FromMilliseconds(50), () => {
				// Add all pin first
				initializePinsLayer();
				addAllReusablePinImages();
				addAllPins();
				//testMarker();

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

		private void initializePinsLayer()
		{
			// New layer - configure elements
			var propertyValues = new List<PropertyValue>{
				PropertyFactory.IconImage("{"+pin_image_key+"}"), //Tokenize the field
				PropertyFactory.IconRotationAlignment("pin_rotation_method_key"), // Finally the map-lock flat = "map"
				PropertyFactory.IconRotate(new Com.Mapbox.Mapboxsdk.Style.Expressions.Expression("{"+pin_rotation_key+"}")), // Programatic heading
				PropertyFactory.IconAnchor("bottom"), // https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-anchor = "bottom" or "center"
				PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True) // Always overlap
			};

			//Com.Mapbox.Mapboxsdk.Style.Expressions.Expression.SwitchCase()

			var layer = new SymbolLayer("pinsLayer", "pinsSource")
				.WithProperties(propertyValues.ToArray());
			nMap.AddLayer(layer);
		}

		private void addAllReusablePinImages()
		{
			foreach (var pin in xMap.pins) {
				var bitmap = nMap.GetImage(pin.image);

				if (bitmap == null) {
					// New image
					using (var stream = pin.image.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
						var newBitmap = BitmapFactory.DecodeStream(stream);
						nMap.AddImage(pin.image, newBitmap);
					}
				}
			}
		}

		private void addAllPins()
		{
			if (xMap.pins == null)
				return;

			// Initial batch update
			var geoJsonSource = new GeoJsonSource("pinsSource", xMap.pins.toFeatureCollection());
			nMap.AddSource(geoJsonSource);
		}

		private void removeAllPins()
		{
			//if (xMap.pins == null)
			//	return;

			//// Clear all native pin stuffs
			//foreach (var pinKey in nativePinKeyReferences) {
			//	nMap.RemoveSource(pinKey);
			//	nMap.RemoveImage(pinKey);
			//	nMap.RemoveLayer(pinKey);
			//}

			//// Reset the saved pin key references of all types
			//nativePinKeyReferences.Clear();
		}

		void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(XMapbox.Map.pins)) {
				// The entire pins collection itself has been changed
				// Remove all pins first for this map instance
				removeAllPins();
				// Then add all of the pins from the new collection
				addAllPins();
			}
		}

		void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// No move and replace support yet
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (Pin pin in e.NewItems)
						addPin(pin);
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Pin pin in e.OldItems)
						removePin(pin);
					break;
				case NotifyCollectionChangedAction.Reset:
					removeAllPins();
					break;
			}
		}

		private void testMarker()
		{
			#region MyRegion
			//Device.StartTimer(TimeSpan.FromSeconds(5), () => {
			//	// Set the features
			//	var features = new List<Feature>{
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(0,0)),
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(1,1)),
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(2,2)),
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(3,3)),
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(4,4)),
			//		Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(5,5)),
			//	};
			//	var featureCollection = FeatureCollection.FromFeatures(features.ToArray());

			//	// Set the geoJsonSource
			//	var geoJsonSource = new GeoJsonSource("marker-source", featureCollection);
			//	nMap.AddSource(geoJsonSource);

			//	// Init actual image
			//	using (var stream = "car.png".getRawStremFromEmbeddedResource(xMap.callerAssembly, 50, 50)) {
			//		var bitmap = BitmapFactory.DecodeStream(stream);
			//		nMap.AddImage("my-marker-image", bitmap);
			//	}

			//	// Set the layer
			//	var markers = new SymbolLayer("marker-layer", "marker-source")
			//		.WithProperties(PropertyFactory.IconImage("my-marker-image"),
			//						PropertyFactory.IconRotationAlignment("map"), // Finally the map-lock flat
			//						PropertyFactory.IconRotate(new Java.Lang.Float(90f)), // Programatic heading
			//						PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True));
			//	nMap.AddLayer(markers);

			//	// Update new location
			//	Device.StartTimer(TimeSpan.FromSeconds(5), () => {
			//		// Update location of the pin

			//		// Set the features
			//		var features2 = new List<Feature>{
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(10,10)),
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(11,11)),
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(12,12)),
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(13,13)),
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(14,14)),
			//			Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(15,15)),
			//		};
			//		var featureCollection2 = FeatureCollection.FromFeatures(features2.ToArray());
			//		geoJsonSource.SetGeoJson(featureCollection2);
			//		return false;
			//	});
			//	return false;
			//});
			#endregion

			Device.StartTimer(TimeSpan.FromSeconds(5), () => {
				// Init actual image
				using (var stream = "Resources.car.png".getRawStremFromEmbeddedResource(xMap.callerAssembly, 50, 50)) {
					var bitmap = BitmapFactory.DecodeStream(stream);
					nMap.AddImage("image", bitmap);
				}

				using (var stream = "Resources.carBlack.jpg".getRawStremFromEmbeddedResource(xMap.callerAssembly, 50, 50)) {
					var bitmap = BitmapFactory.DecodeStream(stream);
					nMap.AddImage("image2", bitmap);
				}

				// Set the layer
				var markers = new SymbolLayer("marker-layer", "marker-source")
					.WithProperties(PropertyFactory.IconImage("{type}"),
									PropertyFactory.IconRotationAlignment("map"), // Finally the map-lock flat
									PropertyFactory.IconRotate(new Java.Lang.Float(90f)), // Programatic heading
									PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True));
				nMap.AddLayer(markers);

				// Set the features
				var features = new List<Feature>{
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(0,0)),
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(1,1)),
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(2,2)),
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(3,3)),
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(4,4)),
					Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(5,5)),
				};

				features[0].AddStringProperty("type", "image");
				features[1].AddStringProperty("type", "image2");
				features[2].AddStringProperty("type", "image");
				features[3].AddStringProperty("type", "image2");
				features[4].AddStringProperty("type", "image");
				features[5].AddStringProperty("type", "image2");

				var featureCollection = FeatureCollection.FromFeatures(features.ToArray());

				// Set the geoJsonSource
				var geoJsonSource = new GeoJsonSource("marker-source", featureCollection);
				nMap.AddSource(geoJsonSource);

				// Update new location
				Device.StartTimer(TimeSpan.FromSeconds(5), () => {
					// Update location of the pin

					// Set the features
					var features2 = new List<Feature>{
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(10,10)),
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(11,11)),
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(12,12)),
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(13,13)),
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(14,14)),
						Feature.FromGeometry(Com.Mapbox.Geojson.Point.FromLngLat(15,15)),
					};

					features2[0].AddStringProperty("type", "image");
					features2[1].AddStringProperty("type", "image2");
					features2[2].AddStringProperty("type", "image");
					features2[3].AddStringProperty("type", "image2");
					features2[4].AddStringProperty("type", "image");
					features2[5].AddStringProperty("type", "image2");

					var featureCollection2 = FeatureCollection.FromFeatures(features2.ToArray());
					geoJsonSource.SetGeoJson(featureCollection2);
					return false;
				});
				return false;
			});
		}

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
