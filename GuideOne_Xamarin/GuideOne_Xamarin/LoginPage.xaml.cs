using GuideOne_Xamarin.Helpers;
using GuideOne_Xamarin.Model;
using Newtonsoft.Json;
using Org.Apache.Http.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace GuideOne_Xamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			if (Conf.User == null)
				Navigation.PopAsync();
			Title = "Авторизация";
			InitializeComponent();
			Task.Run(() =>
			{
				var rand = new Random();
				var keySize = rand.Next(8, 16) * 128;
				Conf.CryptoProvider = new RSACryptoServiceProvider(keySize);
				Conf.User = new User()
				{
					PrivateKey = Conf.CryptoProvider.ToXmlString(true),
				};
			});
		}

		protected async override void OnAppearing()
		{
			if (Conf.User != null && Conf.User.IsConfirmed)
			{
				var mp = new MainPage();
				Navigation.InsertPageBefore(mp, this);
				await Navigation.PopAsync(true);
			}
		}

		async void OnButtonClicked(object sender, System.EventArgs e)
		{
			Button button = (Button)sender;
			button.IsEnabled = false;
			systeminfo.Text = (string)HTTPConector.Resources["ServerUriPrefix"] + (string)HTTPConector.Resources["ServerUri"];
			Conf.User.Phone = Phone.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
			var json = new
			{
				Phone = Conf.User.Phone,
				PublicKey = Conf.CryptoProvider.ToXmlString(false)
			};
			var httpConnector = new HTTPConector(hash: false);
			httpConnector.Address = "user";
			httpConnector.Content = ("User", json);
			var resp = string.Empty;
			try
			{
				resp = await httpConnector.SendAsync<UserMiniSerializer>();
			}
			catch (Exception ex)
			{
				systeminfo.Text = ex.Message;
			}

			if (httpConnector?.Response?.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var newUser = JsonConvert.DeserializeObject<User>(resp);
				var user = Conf.User;
				user.Token = newUser.Token;
				user.Id = newUser.Id;
				user.AuthId = newUser.AuthId;
				newUser = App.Database.FindUser();
				if (newUser != null)
					App.Database.DeleteItem<User>(newUser.Id);
				user.IsUser = true;
				App.Database.SaveItem(user);
				Conf.User = user;
				await Navigation.PushAsync(new LoginConfirmPage());
			}
			else
			{
				systeminfo.Text += resp;
				Submit.IsEnabled = true;
			}
		}
	}
}