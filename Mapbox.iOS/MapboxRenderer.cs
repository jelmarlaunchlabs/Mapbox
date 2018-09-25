using System;
using System.Collections.Generic;
using System.Reflection;
using CoreLocation;
using Foundation;
using MapBox;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MapBox.Extensions;
using System.Linq;
using System.Collections.Specialized;
using Mapbox.iOS.Extensions;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(MapBox.Map), typeof(Mapbox.iOS.MapboxRenderer))]
namespace Mapbox.iOS
{
    public class MapboxRenderer : ViewRenderer<Map, MGLMapView>, IMGLMapViewDelegate, IUIGestureRecognizerDelegate
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

		private Map xMap;
		private MGLMapView nMap;
		private MGLStyle nStyle;

		public static void init()
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

				// Unsubscribe to changes in the collection first
				if (map.routes != null)
					map.routes.CollectionChanged -= Routes_CollectionChanged;

				removeAllPins();

				removeAllRoutes();

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
			nMap = new MGLMapView(Bounds, MGLStyle.OutdoorsStyleURL) {
				WeakDelegate = this,
			};
			SetNativeControl(nMap);
		}

		[Export("mapView:didFinishLoadingStyle:")]
		public void didFinishLoadingStyle(MGLMapView mGLMapView, MGLStyle mGLStyle)
		{
			nMap = mGLMapView;
			nStyle = mGLStyle;
			setupGestureRecognizer();

			// Add all pin first
			initializePinsLayer();
			addAllReusablePinImages();
			addAllPins();

			initializeRoutesLayer();
			addAllRoutes();

			// Then subscribe to pins added
			if (xMap.pins != null)
				xMap.pins.CollectionChanged += Pins_CollectionChanged;

			if (xMap.routes != null)
				xMap.routes.CollectionChanged += Routes_CollectionChanged;

			// Subscribe to changes in map bindable properties after all pins are loaded
			xMap.PropertyChanged += Map_PropertyChanged;
		}

		private void setupGestureRecognizer()
		{
			var tapGest = new UITapGestureRecognizer();
			tapGest.NumberOfTapsRequired = 1;
			tapGest.CancelsTouchesInView = false;
			tapGest.Delegate = this;
			nMap.AddGestureRecognizer(tapGest);
			tapGest.AddTarget((NSObject obj) => {
				var gesture = obj as UITapGestureRecognizer;
				if (gesture.State == UIGestureRecognizerState.Ended) {
					var point = gesture.LocationInView(nMap);
					var touchedCooridinate = nMap.ConvertPoint(point, nMap);
					var position = new MapBox.Models.Position(touchedCooridinate.Latitude, touchedCooridinate.Longitude);
					var features = nMap.VisibleFeaturesAtPoint(point, new NSSet<NSString>(new NSString[] { new NSString(mapLockedPinsKey), new NSString(normalPinsKey) }));

					if (features.Any() && xMap.pinClickedCommand != null)
						xMap.pinClickedCommand.Execute(null);
				}
			});
		}

		#region Route initializers
		private void initializeRoutesLayer()
		{
			// Just initialize with empty so that it's guaranteed that there are sources initiated
			// when the routes collection is still null
			var routeSource = new MGLShapeSource(line_source_key, new NSObject[] { }, null);
			nStyle.AddSource(routeSource);

			var borderLinesLayer = new MGLLineStyleLayer(border_line_layer_key, routeSource);
			borderLinesLayer.LineColor = NSExpression.FromKeyPath(border_line_color_key);
			borderLinesLayer.LineWidth = NSExpression.FromKeyPath(border_line_width_key);
			borderLinesLayer.LineJoin = NSExpression.FromConstant(NSObject.FromObject("round"));
			borderLinesLayer.LineCap = NSExpression.FromConstant(NSObject.FromObject("round"));
			nStyle.AddLayer(borderLinesLayer);

			var linesLayer = new MGLLineStyleLayer(line_layer_key, routeSource);
			linesLayer.LineColor = NSExpression.FromKeyPath(line_color_key);
			linesLayer.LineWidth = NSExpression.FromKeyPath(line_width_key);
			linesLayer.LineJoin = NSExpression.FromConstant(NSObject.FromObject("round"));
			linesLayer.LineCap = NSExpression.FromConstant(NSObject.FromObject("round"));
			nStyle.AddLayer(linesLayer);
		}

		private void addAllRoutes()
		{
			if (xMap.routes == null)
				return;

			// Subscribe all pins to their respective changes
			foreach (var route in xMap.routes)
				route.PropertyChanged += Pin_PropertyChanged;

			var routeSource = (MGLShapeSource)nStyle.SourceWithIdentifier(line_source_key);
			routeSource.Shape = xMap.routes.toShapeCollectionFeature();
		}
		#endregion

		#region Post route initialization route operations
		private void removeAllRoutes()
		{
			if (xMap.routes == null)
				return;

			var routeSource = (MGLShapeSource)nStyle.SourceWithIdentifier(line_source_key);

			// Just set to empy route, this will also update with the latest route and the old routes removed
			routeSource.Shape = xMap.routes.toShapeCollectionFeature();

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

			var routeSource = (MGLShapeSource)nStyle.SourceWithIdentifier(line_source_key);

			// Update the route
			routeSource.Shape = xMap.routes.toShapeCollectionFeature();
		}
		#endregion

		#region Route change detectors
		private void Routes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

		private void Route_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// TODO: implement
		}
		#endregion

		#region Pin intializers
		private void initializePinsLayer()
		{
			// Just initialize with empty so that it's guaranteed that there are sources initiated
			// when the pins collection is still null
			var mapLockedPinsSource = new MGLShapeSource(mapLockedPinsSourceKey, new NSObject[]{}, null);
			nStyle.AddSource(mapLockedPinsSource);

			// Pins with bearings layer - configure elements
			var mapLockedPinsLayer = new MGLSymbolStyleLayer(mapLockedPinsKey, mapLockedPinsSource) {
				IconImageName = NSExpression.FromKeyPath(pin_image_key),
				IconRotation = NSExpression.FromKeyPath(pin_rotation_key),
				IconScale = NSExpression.FromKeyPath(pin_size_key),
				IconRotationAlignment = NSExpression.FromConstant(NSObject.FromObject("map")), // Finally the map-lock flat = "map"
				IconAllowsOverlap = NSExpression.FromConstant(NSObject.FromObject(true)) // Always overlap
			};
			nStyle.AddLayer(mapLockedPinsLayer);

			// Just initialize with empty so that it's guaranteed that there are sources initiated
			// when the pins collection is still null
			var normalPinsSource = new MGLShapeSource(normalPinsSourceKey, new NSObject[] { }, null);
			nStyle.AddSource(normalPinsSource);

			// Normal pins layer - configure elements
			var normalPinsLayer = new MGLSymbolStyleLayer(normalPinsKey, normalPinsSource) {
				IconImageName = NSExpression.FromKeyPath(pin_image_key),
				IconScale = NSExpression.FromKeyPath(pin_size_key),
				IconOffset = NSExpression.FromKeyPath(pin_offset_key), //https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-offset
				IconAnchor = NSExpression.FromConstant(NSObject.FromObject("bottom")), // https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-anchor = "bottom" or "center"
				IconAllowsOverlap = NSExpression.FromConstant(NSObject.FromObject(true))
			};
			nStyle.AddLayer(normalPinsLayer);
		}

		private void addAllReusablePinImages()
		{
			foreach (var pin in xMap.pins) {
				var uiImage = nStyle.ImageForName(pin.image);

				// If any existing item does not yet exist
				if (uiImage == null) {
					// Then add new image
					using (var stream = pin.image.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
						using (var imageData = NSData.FromStream(stream)) {
							var newUIImage = UIImage.LoadFromData(imageData);
							nStyle.SetImage(newUIImage, pin.image);
						}
					}
				}
			}
		}

		private void addAllPins()
		{
			if (xMap.pins == null)
				return;

			// Subscribe all pins to their respective changes
			foreach (var pin in xMap.pins)
				pin.PropertyChanged += Pin_PropertyChanged;

			// Add the mapLocked pins first
			// Select all pins where it has heading setter activated
			var mapLockedFeatureCollection = xMap.pins.Where((Pin pin) => pin.IsCenterAndFlat).toShapeSourceArray();

			var mapLockedPinSource = (MGLShapeSource)nStyle.SourceWithIdentifier(mapLockedPinsSourceKey);
			mapLockedPinSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(mapLockedFeatureCollection);

			// Add the normal pins after
			// Select all pins where it has heading disabled
			var normalFeatureCollection = xMap.pins.Where((Pin pin) => !pin.IsCenterAndFlat).toShapeSourceArray();

			var normalPinSource = (MGLShapeSource)nStyle.SourceWithIdentifier(normalPinsSourceKey);
			normalPinSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(normalFeatureCollection);
		}
		#endregion

		#region Post pin initialization pin operations
		private void removeAllPins()
		{
			if (xMap.pins == null)
				return;

			// Get the segregated pin type by flat value to avoid multiple instance of the same pin to be added
			var mapLockedFeatureCollection = xMap.pins.Where((Pin pin) => pin.IsCenterAndFlat);
			var normalFeatureCollection = xMap.pins.Where((Pin pin) => !pin.IsCenterAndFlat);

			var flatPinsSource = (MGLShapeSource)nStyle.SourceWithIdentifier(mapLockedPinsSourceKey);
			var normalPinsSource = (MGLShapeSource)nStyle.SourceWithIdentifier(normalPinsSourceKey);

			// Just set to empty pins, this will also refreshes with the latest pin and the old pins removed
			flatPinsSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(mapLockedFeatureCollection.toShapeSourceArray());
			normalPinsSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(normalFeatureCollection.toShapeSourceArray());

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
			var uiImage = nStyle.ImageForName(key);
			// Get all pins with the same flat value
			var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat);

			// If there are existing image of the same type the reuse
			if (uiImage != null)
				updatePins(pin);
			else {
				// Otherwise add new

				//New image
				using (var stream = pin.image.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
					using (var imageData = NSData.FromStream(stream)) {
						var newUIImage = UIImage.LoadFromData(imageData);
						nStyle.SetImage(newUIImage, key);
					}
				}

				updatePins(pin);
			}
		}

		private void updatePins(Pin pin)
		{
			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var uiImage = nStyle.ImageForName(key);
			// Get all pins with the same flat value
			var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat);

			// If there are existing image of the same type the reuse
			if (uiImage != null) {
				MGLShapeSource geoJsonSource;

				// Edit the proper source
				if (pin.IsCenterAndFlat)
					geoJsonSource = (MGLShapeSource)nStyle.SourceWithIdentifier(mapLockedPinsSourceKey);
				else
					geoJsonSource = (MGLShapeSource)nStyle.SourceWithIdentifier(normalPinsSourceKey);

				// Refresh entire source when a pin is added to a specific source
				geoJsonSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(pinsWithSimilarKey.toShapeSourceArray());
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
			var bitmap = nStyle.ImageForName(key);
			// Get all pins with the same key and same flat value
			var pinsWithSimilarKeyAndNotThisPin = xMap.pins.Where(
				(Pin arg) => arg.IsCenterAndFlat == pin.IsCenterAndFlat && arg != pin).toFeatureList();
			var geoJsonSource = (MGLShapeSource)nStyle.SourceWithIdentifier(mapLockedPinsSourceKey);

			pin.previousPinPosition.animatePin(
				pin.position,
				new Command<double>((d) => {
					var feature = new MGLPointFeature {
						Coordinate = new CLLocationCoordinate2D(pin.position.latitude * d,
																pin.position.longitude * d)
					};
					feature.Attributes = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
						new object[] { 
							pin.image,
							pin.heading,
							pin.imageScaleFactor
						},
						new object[] {
							MapboxRenderer.pin_image_key,
							MapboxRenderer.pin_rotation_key,
							MapboxRenderer.pin_size_key
						});

					// Update the feature thats need animating
					pinsWithSimilarKeyAndNotThisPin.Add(feature);

					// Update the entire layer
					geoJsonSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(pinsWithSimilarKeyAndNotThisPin.toShapeSourceArray());

					// Remove the feature so that there will be no multiple instances of it accross the animation path
					pinsWithSimilarKeyAndNotThisPin.Remove(feature);
				}));
		}
		#endregion

		#region Change detectors
		void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// The entire pins collection itself has been changed
			if (e.PropertyName == Map.pinsProperty.PropertyName)
				removeAllPins();
			if (e.PropertyName == Map.routesProperty.PropertyName)
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
		#endregion

		private void testPolyline(MGLStyle style)
		{
			var features = new List<CLLocationCoordinate2D> {
				new CLLocationCoordinate2D(0,0),
				new CLLocationCoordinate2D(1,1),
				new CLLocationCoordinate2D(2,2),
				new CLLocationCoordinate2D(3,3),
				new CLLocationCoordinate2D(4,4),
				new CLLocationCoordinate2D(5,5)
			};

			var x = features.ToArray();
			var polyline = MGLPolylineFeature.PolylineWithCoordinates(ref x[0], (System.nuint)x.Length);

			var geoJsonSource = new MGLShapeSource("line-source", polyline, NSDictionary<NSString, NSObject>.FromObjectsAndKeys(new object[]{100}, new object[]{"shit"}));
			style.AddSource(geoJsonSource);
			//Xamarin.Forms.Color.FromHex("").ToUIColor();
			var lineStyleLayer = new MGLLineStyleLayer("lineStyleLayer", geoJsonSource);
			lineStyleLayer.LineColor = NSExpression.FromConstant(Color.FromHex("#d3c717").ToUIColor());
			lineStyleLayer.LineWidth = NSExpression.FromConstant(NSObject.FromObject(6f));
			//lineStyleLayer.LineWidth = NSExpression.FromKeyPath("shit");
			lineStyleLayer.LineJoin = NSExpression.FromConstant(NSObject.FromObject("round"));
			lineStyleLayer.LineCap = NSExpression.FromConstant(NSObject.FromObject("round"));
			style.AddLayer(lineStyleLayer);

			var lineStyleLayer2 = new MGLLineStyleLayer("lineStyleLayer2", geoJsonSource);
			lineStyleLayer2.LineColor = NSExpression.FromConstant(Color.FromHex("#fc0000").ToUIColor());
			lineStyleLayer2.LineWidth = NSExpression.FromConstant(NSObject.FromObject(3f));
			lineStyleLayer2.LineJoin = NSExpression.FromConstant(NSObject.FromObject("round"));
			lineStyleLayer2.LineCap = NSExpression.FromConstant(NSObject.FromObject("round"));
			style.AddLayer(lineStyleLayer2);
		}
	}
}
