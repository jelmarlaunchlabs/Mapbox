﻿using System.Collections.Generic;
using Com.Mapbox.Mapboxsdk.Offline;
using GoogleGson;
using Java.Lang;
using MapBox.Models;
using MapBox.Offline;

namespace MapBox.Android.Offline
{
	public static class OfflinePackExtensions
	{
		public static OfflinePack ToFormsPack(this OfflineRegion mbRegion)
		{
			if (mbRegion == null) return null;
			var output = new OfflinePack() {
				Handle = mbRegion.Handle
			};
			output.Id = mbRegion.ID;
			var definition = mbRegion.Definition;
			if (definition is OfflineTilePyramidRegionDefinition def) {
				output.Region = def.ToFormsRegion();
			}
			if (mbRegion.GetMetadata() is byte[] metadata) {
				String json = new String(metadata, OfflineStorageService.JSON_CHARSET);
				try {
					JsonObject jsonObject = (JsonObject)new Gson().FromJson(json.ToString(), Java.Lang.Class.FromType(typeof(JsonObject)));
					if (jsonObject != null) {
						var keys = jsonObject.KeySet();
						output.Info = new Dictionary<string, string>(keys.Count);
						foreach (string key in keys) {
							output.Info.Add(key, jsonObject.Get(key).AsString);
						}
					}
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine("Failed to decode offline region metadata: " + ex.Message);
				}
			}
			return output;
		}

		public static OfflinePackRegion ToFormsRegion(this OfflineTilePyramidRegionDefinition definition)
		{
			return new OfflinePackRegion {
				Bounds = new Bounds(new Position(definition.Bounds.SouthWest.Latitude, definition.Bounds.SouthWest.Longitude),
									new Position(definition.Bounds.NorthEast.Latitude, definition.Bounds.NorthEast.Longitude)),
				StyleURL = definition.StyleURL,
				MinimumZoomLevel = definition.MinZoom,
				MaximumZoomLevel = definition.MaxZoom
			};
		}
	}
}
