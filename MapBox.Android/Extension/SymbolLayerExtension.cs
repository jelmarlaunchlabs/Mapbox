using System;
using System.Collections.ObjectModel;
using Com.Mapbox.Geojson;
using System.Linq;
using System.Collections.Generic;

namespace MapBox.Android.Extensions
{
	public static class SymbolLayerExtension
	{
		public static FeatureCollection toFeatureCollection(this IEnumerable<Pin> pins)
		{
			return FeatureCollection.FromFeatures(pins.toFeatureList().ToArray());
		}

		public static List<Feature> toFeatureList(this IEnumerable<Pin> pins)
		{
			var features = new List<Feature>();

			foreach (var pin in pins) {
				var feature = Feature.FromGeometry(
					Point.FromLngLat(pin.position.longitude,
									 pin.position.latitude));
				feature.AddStringProperty(MapboxRenderer.pin_image_key, pin.image);
				feature.AddNumberProperty(MapboxRenderer.pin_rotation_key, (Java.Lang.Number)pin.heading);
				feature.AddNumberProperty(MapboxRenderer.pin_size_key, (Java.Lang.Number)pin.imageScaleFactor);
				var offset = new GoogleGson.JsonArray(2); //[x,y] coordinates Positive values indicate right and down
				offset.Add((Java.Lang.Number)pin.iconOffset.X);
				offset.Add((Java.Lang.Number)pin.iconOffset.Y);
				feature.AddProperty(MapboxRenderer.pin_offset_key, offset);
				features.Add(feature);
			}

			return features;
		}

		public static FeatureCollection toFeatureCollection(this IEnumerable<Feature> features)
		{
			return FeatureCollection.FromFeatures(features.ToArray());
		}
	}
}