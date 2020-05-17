using GuideOne_Xamarin.Helpers;
using GuideOne_Xamarin.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace GuideOne_Xamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginConfirmPage : ContentPage
	{
		public  LoginConfirmPage()
		{
			InitializeComponent();
			if (Conf.User == null)
			{
				Navigation.PushAsync(new LoginPage());
			}
			else if (Conf.User.IsConfirmed)
			{
				Navigation.PopAsync();
			}
		}


		async void OnButtonClicked(object sender, System.EventArgs e)
		{
			Button button = (Button)sender;
			button.IsEnabled = false;

			var httpConnector = new HTTPConector(hash: true);
			httpConnector.Address = "user/confirm";
			httpConnector.Content = ("Code", Code.Text.Replace("-", ""));
			var resp = string.Empty;
			try
			{
				resp = await httpConnector.SendAsync<UserMiniSerializer>();
			}
			catch (Exception ex)
			{	
				systeminfo.Text = ex.Message;
			}
			if (httpConnector.Response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var jobj = JObject.Parse(resp);
				var isConfirm = jobj.Value<bool>("isConfirmed");
				if (isConfirm)
				{
					var userExt = jobj.GetValue("user").ToObject<User>();
					Conf.User.CompanyId = userExt.CompanyId;
					Conf.User.Name = userExt.Name;
					Conf.User.SecondName = userExt.SecondName;
					Conf.User.HourPrice = userExt.HourPrice;
					Conf.User.IsConfirmed = true;
					App.Database.SaveItem(Conf.User);
					var mp = new MainPage();
					await Navigation.PushAsync(mp);
				}
				else
				{
					Info.Text = "Не верный код";
					Info.TextColor = Color.Red;
				}
			}
			Submit.IsEnabled = true;
		}
	}
}