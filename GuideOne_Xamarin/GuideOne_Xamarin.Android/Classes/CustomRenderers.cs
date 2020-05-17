using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace GuideOne_Xamarin.Droid.Classes
{
	class CustomRenderers
	{
        public class Statusbar : GuideOne_Xamarin.Interfaces.IStatusBarPlatformSpecific
        {
            public Statusbar()
            {
            }

            public void SetStatusBarColor(Color color)
            {
                // The SetStatusBarcolor is new since API 21
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var androidColor = color.AddLuminosity(-0.1).ToAndroid();
                    //Just use the plugin
                    CrossCurrentActivity.Current.Activity.Window.SetStatusBarColor(androidColor);
                }
                else
                {
                    // Here you will just have to set your 
                    // color in styles.xml file as shown below.
                }
            }
        }
    }
}