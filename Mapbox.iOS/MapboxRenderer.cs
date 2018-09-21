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

[assembly: ExportRenderer(typeof(MapBox.Map), typeof(Mapbox.iOS.MapboxRenderer))]
namespace Mapbox.iOS
{
    public class MapboxRenderer : ViewRenderer<Map, MGLMapView>, IMGLMapViewDelegate
    {
		private MGLMapView nMap;
		private Map xMap;

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
			}
			if (e.NewElement != null) {
				// Subscribe
				var map = e.NewElement;
				xMap = map;
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
			testMarker(mGLStyle);
			//testPolyline(mGLStyle);
		}

		private void testMarker(MGLStyle style)
		{
			var x = new Dictionary<NSString, NSObject> {
				{new NSString("type"), NSString.FromObject("carBlack")}
			};
			var d = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(x.Values.ToArray(), x.Keys.ToArray());

			var x2 = new Dictionary<NSString, NSObject> {
				{new NSString("type"), NSString.FromObject("car")}
			};

			NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
				new object[]{
					"carBlack",
				},
				new object[]{
					"type"
				});

			var d2 = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(x2.Values.ToArray(), x2.Keys.ToArray());

			var features = new List<NSObject> {
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(0,0), Attributes = d},
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(1,1), Attributes = d2},
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(2,2), Attributes = d},
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(3,3), Attributes = d2},
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(4,4), Attributes = d},
				new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(5,5), Attributes = d2}
			};

			// Set the geoJsonSource
			var geoJsonSource = new MGLShapeSource("us-lighthouses", features.ToArray(), null);
			style.AddSource(geoJsonSource);

			// Init actual image
			UIImage uIImage = null;
			using (var stream = "Resources.car.png".getRawStremFromEmbeddedResource(xMap.callerAssembly, 50, 50))
			using (var imageData = NSData.FromStream(stream))
				uIImage = UIImage.LoadFromData(imageData);
			style.SetImage(uIImage, "car");


			UIImage uIImage2 = null;
			using (var stream = "Resources.carBlack.jpg".getRawStremFromEmbeddedResource(xMap.callerAssembly, 50, 50))
			using (var imageData = NSData.FromStream(stream))
				uIImage2 = UIImage.LoadFromData(imageData);
			style.SetImage(uIImage2, "carBlack");

			// Set the layer
			var layer = new MGLSymbolStyleLayer("marker-layer", geoJsonSource);
			layer.IconImageName = NSExpression.FromKeyPath("type");
			//layer.IconImageName = NSExpression.FromConstant(NSObject.FromObject("{type}"));
			layer.IconRotationAlignment = NSExpression.FromConstant(NSObject.FromObject("map"));
			layer.IconRotation = NSExpression.FromConstant(NSObject.FromObject(90f));
			layer.IconAllowsOverlap = NSExpression.FromConstant(NSObject.FromObject(true));
			style.AddLayer(layer);

			// Update new location
			Device.StartTimer(TimeSpan.FromSeconds(5), () => {
				// Update new location
				var features2 = new List<NSObject> {
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(10,10), Attributes = d},
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(11,11), Attributes = d2},
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(12,12), Attributes = d},
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(13,13), Attributes = d2},
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(14,14), Attributes = d},
						new MGLPointFeature{Coordinate = new CLLocationCoordinate2D(15,15), Attributes = d2}
					};

				// Updated the location
				geoJsonSource.Shape = MGLShapeCollectionFeature.ShapeCollectionWithShapes(features2.ToArray());
				return false;
			});
		}

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

			var geoJsonSource = new MGLShapeSource("line-source", polyline, null);
			style.AddSource(geoJsonSource);

			var lineStyleLayer = new MGLLineStyleLayer("lineStyleLayer", geoJsonSource);
			lineStyleLayer.LineColor = NSExpression.FromConstant(Color.FromHex("#d3c717").ToUIColor());
			lineStyleLayer.LineWidth = NSExpression.FromConstant(NSObject.FromObject(6f));
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
