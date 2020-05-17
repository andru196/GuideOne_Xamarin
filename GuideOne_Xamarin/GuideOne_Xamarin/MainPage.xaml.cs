using Android.Views;
using GuideOne_Xamarin.Interfaces;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GuideOne_Xamarin
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			Task.Run(async () =>
			{
				var locator = CrossGeolocator.Current;
				var position = await locator.GetLastKnownLocationAsync();
				if (position == null)
					position = await locator.GetPositionAsync(new TimeSpan(10000));
				map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
														 Distance.FromMeters(500)));
			});
			NavigationPage.SetHasNavigationBar(this, false);


			//this.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
			//this.SetValue(NavigationPage.BackgroundColorProperty, Color.Black);
			//var statusbar = DependencyService.Get<IStatusBarPlatformSpecific>();
			//statusbar.SetStatusBarColor(Color.Green);

		}
	}
}
