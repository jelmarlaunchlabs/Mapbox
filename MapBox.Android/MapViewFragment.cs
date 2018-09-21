using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Com.Mapbox.Geojson;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Mapboxsdk.Style.Layers;
using Com.Mapbox.Mapboxsdk.Style.Sources;
using MapBox.Android.Extension;
using MapBox.Extensions;

namespace MapBox.Android
{
    public class MapViewFragment : SupportMapFragment, MapView.IOnMapChangedListener
    {
        public MapView MapView { get; private set; }

        public MapView.IOnMapChangedListener OnMapChangedListener { get; set; }

        public bool StateSaved { get; private set; }

        public MapViewFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MapViewFragment() : base()
        {

        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            MapView = view as MapView;
            MapView?.AddOnMapChangedListener(this);
        }


        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MapView?.RemoveOnMapChangedListener(this);
        }

        public void OnMapChanged(int p0)
        {
            OnMapChangedListener?.OnMapChanged(p0);
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        internal void ToggleInfoWindow(MapboxMap mapboxMap, Marker marker)
        {
            if (marker.IsInfoWindowShown)
            {
                mapboxMap.DeselectMarker(marker);
            }
            else
            {
                mapboxMap.SelectMarker(marker);
            }
        }
    }

	public partial class MapboxRenderer
	{
		//private List<string> nativePinKeyReferences = new List<string>();

		public void addPin(Pin pin)
		{
			#region Working
			//// The image is the type/class key
			//var key = pin.image;
			//// Find a source that has the same image as this pin
			//var bitmap = nMap.GetImage(key);
			//// Get all pins with the same key
			//var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.image == key).toFeatureCollection();
			//int pinHeight;

			//// Use the existing values in map
			//if (bitmap != null) {
			//	var geoJsonSource = (GeoJsonSource)nMap.GetSource(key);
			//	//var layer = nMap.GetLayer(key);

			//	// Refresh entire source when a pin is added to a specific source
			//	geoJsonSource.SetGeoJson(pinsWithSimilarKey);
			//} else { // Else, then create new source, image and layer
			//		 // New collection of points
			//		 //var featureCollect = xMap.pins.toFeatureCollection();

			//	// New source
			//	var geoJsonSource = new GeoJsonSource(key, pinsWithSimilarKey);
			//	nMap.AddSource(geoJsonSource);

			//	// New image
			//	using (var stream = key.getRawStremFromEmbeddedResource(xMap.callerAssembly, pin.width, pin.height)) {
			//		var newBitmap = BitmapFactory.DecodeStream(stream);
			//		pinHeight = newBitmap.Height;
			//		nMap.AddImage(key, newBitmap);
			//	}

			//	// New layer - configure elements
			//	var propertyValues = new List<PropertyValue>{
			//		PropertyFactory.IconImage(key),
			//		PropertyFactory.IconAllowOverlap(Java.Lang.Boolean.True),
			//	};
			//	if (pin.IsCenterAndFlat) {
			//		propertyValues.Add(PropertyFactory.IconRotationAlignment("map")); // Finally the map-lock flat
			//		propertyValues.Add(PropertyFactory.IconRotate(new Java.Lang.Float(pin.heading))); // Programatic heading
			//	} else
			//		propertyValues.Add(PropertyFactory.IconAnchor("bottom")); // https://www.mapbox.com/mapbox-gl-js/style-spec/#layout-symbol-icon-anchor

			//	var layer = new SymbolLayer(key, key).WithProperties(propertyValues.ToArray());
			//	nMap.AddLayer(layer);

			//	// save the key for later removal of all pins natively
			//	nativePinKeyReferences.Add(key);
			//}
			#endregion
		}

		public void removePin(Pin pin)
		{
			// The image is the type/class key
			var key = pin.image;
			// Find a source that has the same image as this pin
			var bitmap = nMap.GetImage(key);
			// Get all pins with the same key
			var pinsWithSimilarKey = xMap.pins.Where((Pin arg) => arg.image == key).toFeatureCollection();

			// Use the existing values in map
			if (bitmap != null) {
				var geoJsonSource = (GeoJsonSource)nMap.GetSource(key);
				//var layer = nMap.GetLayer(key);

				// Refresh entire source when a pin is added to a specific source
				geoJsonSource.SetGeoJson(pinsWithSimilarKey);
			}
		}
	}
}
