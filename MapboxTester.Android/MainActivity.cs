﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics;

namespace MapboxTester.Droid
{
    [Activity(Label = "MapboxTester", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			//Passing the Bitmap for Drawable Pins.
			Dictionary<string, Bitmap> nameToBitmap = new Dictionary<string, Bitmap>();
			nameToBitmap.Add("MapPinTaxi", BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.MapPinTaxi));
			nameToBitmap.Add("MapPinOrigin", BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.MapPinOrigin));
			nameToBitmap.Add("MapPinDestination", BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.MapPinDestination));
			MapBox.Android.MapboxRenderer.init(this, savedInstanceState, "pk.eyJ1IjoiY3Jvd2VsbW8iLCJhIjoiY2ptNjU5MnFoMTdlYTN3bjIxdDRlb3Q4MyJ9.kg7ws7RpJ4nIKL8C31nBDw", nameToBitmap);

			LoadApplication(new App());
        }
    }
}