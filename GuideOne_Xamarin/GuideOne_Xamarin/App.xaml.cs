using GuideOne_Xamarin.Helpers;
using GuideOne_Xamarin.Model;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GuideOne_Xamarin
{
	public partial class App : Application
	{
		public const string DATABASE_NAME = "g1Mobile.db";
		static Repository database;
		public static Repository Database
		{
			get
			{
				if (database == null)
				{
					database = new Repository(
						Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
				}
				return database;
			}
		}
		public App()
		{
			HTTPConector.Resources = Resources;
			Resources.Add("ServerUri", "192.168.1.68:8080/api");
			Resources.Add("ServerUriPrefix", "http://");
			InitializeComponent();
			var user = Database.FindUser();
			ContentPage mp;
			if (user == null)
				mp = new LoginPage();
			else {
				Conf.User = user;
				Conf.CryptoProvider = new System.Security.Cryptography.RSACryptoServiceProvider();
				Conf.CryptoProvider.FromXmlString(user.PrivateKey);
				if (!user.IsConfirmed)
					mp =new LoginConfirmPage();
				else
					mp = new MainPage();
			}
			MainPage = new NavigationPage(mp);
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
