using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using MapBox;
using MapBox.Helpers;

namespace Mapbox.iOS.Extensions
{
	public static class SymbolLayerExtension
	{
		public static NSObject[] toShapeSourceArray(this IEnumerable<Pin> pins)
		{
			return pins.toFeatureList().ToArray();
		}

		public static List<NSObject> toFeatureList(this IEnumerable<Pin> pins)
		{
			var features = new List<NSObject>();

			foreach (var pin in pins) {
				var feature = new MGLPointFeature {
					Coordinate = new CLLocationCoordinate2D(pin.position.latitude, pin.position.longitude)
				};

				object[] offset = {pin.iconOffset.X, pin.iconOffset.Y}; //[x,y] coordinates Positive values indicate right and down
				feature.Attributes = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
					new object[]{
						pin.image,
						pin.heading,
						pin.imageScaleFactor,
						NSArray.FromObjects(offset)
					},
					new object[]{
						MapboxRenderer.pin_image_key,
						MapboxRenderer.pin_rotation_key,
						MapboxRenderer.pin_size_key,
						MapboxRenderer.pin_offset_key
					}
				);

				features.Add(feature);
			}

			return features;
		}

		public static NSObject[] toShapeSourceArray(this IEnumerable<NSObject> features)
		{
			return features.ToArray();
		}

		public static List<NSObject> toFeatureList(this IEnumerable<Route> routes)
		{
			var features = new List<NSObject>();
			var nativeScale = DisplayMetricsHelper.instance.nativeScale;

			foreach (var route in routes) {
				var coordinates = route.points.Select((MapBox.Models.Position arg) => {
					return new CLLocationCoordinate2D(arg.latitude, arg.longitude);
				});

				var featureInArrayFormat = coordinates.ToArray();
				var feature = new MGLPolylineFeature();
				feature.SetCoordinates(ref featureInArrayFormat[0], (System.nuint)featureInArrayFormat.Length);
				feature.Attributes = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
					new object[]{
						route.borderLineColor,
						route.lineColor,
						(route.borderLineWidth * 2 + route.lineWidth) * nativeScale,
						route.lineWidth * nativeScale
					},
					new object[]{
						MapboxRenderer.border_line_color_key,
						MapboxRenderer.line_color_key,
						MapboxRenderer.border_line_width_key,
						MapboxRenderer.line_width_key
					});

				features.Add(feature);
			}

			return features;
		}

		public static MGLShapeCollectionFeature toShapeCollectionFeature(this IEnumerable<Route> routes)
		{
			return MGLShapeCollectionFeature.ShapeCollectionWithShapes(routes.toFeatureList().ToArray());
		}
	}
}
