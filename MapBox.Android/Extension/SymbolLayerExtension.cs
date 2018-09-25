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
			return pins.toFeatureList().toFeatureCollection();
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

		public static List<Feature> toFeatureList(this IEnumerable<Route> routes)
		{
			var features = new List<Feature>();

			foreach (var route in routes) {
				var list = route.points.Select((Models.Position arg) => {
					return Point.FromLngLat(arg.longitude, arg.latitude);
				}).ToList();

				var lineString = LineString.FromLngLats(list);

				var feature = Feature.FromGeometry(lineString);
				feature.AddStringProperty(MapboxRenderer.border_line_color_key, route.borderLineColor);
				feature.AddStringProperty(MapboxRenderer.line_color_key, route.lineColor);
				feature.AddNumberProperty(MapboxRenderer.border_line_width_key, (Java.Lang.Number)route.borderLineWidth);
				feature.AddNumberProperty(MapboxRenderer.line_width_key, (Java.Lang.Number)route.lineWidth);

				features.Add(feature);
			}

			return features;
		}

		public static FeatureCollection toFeatureCollection(this IEnumerable<Route> routes)
		{
			return routes.toFeatureList().toFeatureCollection();
		}
	}
}