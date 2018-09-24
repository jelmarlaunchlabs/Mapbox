using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using MapBox;

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
	}
}
