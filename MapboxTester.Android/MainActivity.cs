﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

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
			MapBox.Android.MapboxRenderer.init(this, "pk.eyJ1IjoiY3Jvd2VsbW8iLCJhIjoiY2ptNjU5MnFoMTdlYTN3bjIxdDRlb3Q4MyJ9.kg7ws7RpJ4nIKL8C31nBDw");
			LoadApplication(new App());

            ////Need to initialize after LoadApplication(new App()) so that the user of the map (this) context will have its map NOT EQUAL to null
            //MapBox.Android.MapboxRenderer.init(this, "pk.eyJ1IjoiY3Jvd2VsbW8iLCJhIjoiY2ptNjU5MnFoMTdlYTN3bjIxdDRlb3Q4MyJ9.kg7ws7RpJ4nIKL8C31nBDw");
        }
    }
}