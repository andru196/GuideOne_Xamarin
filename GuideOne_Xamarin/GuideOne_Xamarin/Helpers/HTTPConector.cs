using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Text.Json;
using GuideOne_Xamarin.Model;
using System.Security.Cryptography;
using System.Linq;
using Xamarin.Android.Net;
using Plugin.Connectivity;
using Xamarin.Essentials;
using System.Net;
using Android.Provider;

namespace GuideOne_Xamarin.Helpers
{
	class HTTPConector 
	{
		public HttpMethod Method
		{
			get {
				return Request.Method;
			}
			set
			{
				if (Request != null)
					Request.Method = value;
			}
		}
		public HttpResponseMessage Response { get; private set; }

		public event EventHandler ConnectionFailed;

		public (string, object) Content { get; set; }
		
		public string Address {
			get
			{
				return Request?.RequestUri?.AbsolutePath;
			}
			set
			{
				if (Request != null)
					Request.RequestUri = new Uri((string)Resources["ServerUriPrefix"] + (string)Resources["ServerUri"] + "/" + value);
			}
		}

		public static ResourceDictionary Resources;
		
		public HttpRequestMessage Request { get; private set; }
		
		public bool NeedHash { get; set; }
		
		public HTTPConector(HttpMethod method = null, bool hash = true)
		{
			Request = new HttpRequestMessage();
			Request.Method = method ?? HttpMethod.Post;
			NeedHash = hash;
		}

		/// <summary>
		/// Отправить данные на сервер
		/// </summary>
		/// <typeparam name="T">Класс, которым будет сериализоваться пользователь</typeparam>
		/// <returns></returns>
		public async Task<string> SendAsync<T>(bool getRaw = false) where T : UserMiniSerializer, new()
		{
			(string contentJson, string hash) = CreateContent<T>();

			Request.Headers.Add("hash", hash);

			Request.Content = new StringContent(contentJson, Encoding.UTF8, "application/json");

			//TODO:
			//Общее решение при не доступности сервера
			if (!await ServerAvailable(Request.RequestUri))
			{
				ConnectionFailed.Invoke(this, null);
				return " Не доступен";
			}
			using (var client = GetClient())
			{
				Response = await client.SendAsync(Request);
			}
			if (getRaw)
				return null;

			if (!Response.IsSuccessStatusCode)
			{
				ConnectionFailed.Invoke(this, null);
				return null;
			}

			if (Response.Content.Headers.ContentType.MediaType == "application/octet-stream")
			{
				var decBytes = DecodeResponseContent(await Response.Content.ReadAsByteArrayAsync());
				return Encoding.UTF8.GetString(decBytes);
			}
			else
				return await Response.Content.ReadAsStringAsync();
		}

		protected HttpClient GetClient()
		{
			if (Device.RuntimePlatform == Device.Android)
				return new HttpClient(new AndroidClientHandler());
			else
				return new HttpClient();
		}

		protected static async Task<bool> ServerAvailable(Uri URL)
		{
			return await CrossConnectivity.Current.IsRemoteReachable(URL.Host, URL.Port);
		}

		protected (string, string) CreateContent<T>() where T: UserMiniSerializer, new()
		{
			if (NeedHash)
			{
				var jso = new JsonSerializerOptions();
				jso.Converters.Add(new  T());
				string user = JsonSerializer.Serialize(Conf.User, jso);
				var contentJson = $"{{{user}," +
					(Content.Item1 == null && Content.Item2 == null 
					? "" 
					: $"\"{Content.Item1}\":{JsonSerializer.Serialize(Content.Item2,new JsonSerializerOptions { IgnoreNullValues = true })},") +
					$"\"CrDt\":\"{DateTime.UtcNow:s}\"}}";
				var body = Encoding.UTF8.GetBytes(contentJson);
				var token = Encoding.ASCII.GetBytes(Conf.User.Token);
				var all = new byte[body.Length + token.Length];
				Array.Copy(body, all, body.Length);
				Array.Copy(token, 0, all, body.Length, token.Length);
				var bhash = MD5.Create().ComputeHash(all);
				var hash = BitConverter.ToString(bhash).Replace("-", "");

				return (contentJson, hash);
			}
			else
				return ($"{{\"{Content.Item1}\":{JsonSerializer.Serialize(Content.Item2)},\"CrDt\":\"{DateTime.UtcNow:s}\"}}", "");
		}

		protected byte[] DecodeResponseContent(byte[] body)
		{
			var partSize = (Conf.CryptoProvider.KeySize) / 8;
			var parts = body.Length / partSize;
			var decBytes = new byte[parts * partSize];
			var bytepart = new byte[partSize];
			var pointer = 0;
			
			for (var i = 0; i < parts; i++)
			{
				Array.Copy(body, partSize * i, bytepart, 0, partSize);
				var decpart = Conf.CryptoProvider.Decrypt(bytepart, false);
				Array.Copy(decpart, 0, decBytes, pointer, decpart.Length);
				pointer += decpart.Length;
			}

			return decBytes.Take(pointer).ToArray();
		}
	}
}
