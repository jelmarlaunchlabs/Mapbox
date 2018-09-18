using System;
using Android.Content;
using Android.Support.V7.App;
using Android.Widget;
using Com.Mapbox.Mapboxsdk.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NView = Android.Views.View;

[assembly:ExportRenderer(typeof(MapBox.Class1),typeof(MapBox.Android.Class1Renderer))]
namespace MapBox.Android
{
    public class Class1Renderer : ViewRenderer<Class1, NView>, IOnMapReadyCallback
    {
        private MapViewFragment fragment;
        private MapboxMap map;

        public static void init(Context context, string accessToken)
        {
            Com.Mapbox.Mapboxsdk.Mapbox.GetInstance(context, accessToken);
        }

        public Class1Renderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Class1> e)
        {
            base.OnElementChanged(e);
            if(Control == null)
            {
                var activity = (AppCompatActivity)Context;
                var view = new FrameLayout(activity)
                {
                    Id = GenerateViewId()
                };

                SetNativeControl(view);

                fragment = new MapViewFragment();

                activity.SupportFragmentManager.BeginTransaction()
                .Replace(view.Id, fragment)
                .CommitAllowingStateLoss();
                fragment.GetMapAsync(this);
            }
        }

        public void OnMapReady(MapboxMap mapBox)
        {
            map = mapBox;
            map.SetStyle("mapbox://styles/mapbox/streets-v9");
        }
    }
}
