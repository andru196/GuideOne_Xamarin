using Android.Media;
using Android.OS;
using CarouselView.FormsPlugin.Abstractions;
using GuideOne_Xamarin.Helpers;
using GuideOne_Xamarin.Model;
using Java.IO;
using MediaManager.Library;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Xamarin.Forms;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
//using Environment = System.Environment;

namespace GuideOne_Xamarin.Components
{
	public class RecordPlayer : ContentView
	{
		public ISimpleAudioPlayer Player;
		public CarouselViewControl Cvc;
		public RecordPlayer(IEnumerable<Record> recList)
		{
			var f = new Frame();
			f.CornerRadius = 10;
			var grid = new Grid
			{
				RowDefinitions =
				{
					new RowDefinition {Height = new GridLength(3, GridUnitType.Star)},
					new RowDefinition {Height = new GridLength(1, GridUnitType.Star)}
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			Content = f;
			f.Content = (grid);
			
			Cvc = new CarouselViewControl();
			Cvc.ItemsSource = recList;
			Cvc.ItemTemplate = new DataTemplate(() =>
			 {
				 Label titleLabel = new Label { FontSize = 18 };
				 titleLabel.SetBinding(Label.TextProperty, "Name");

				 Label latitudeLabel = new Label { FontSize = 12 };
				 latitudeLabel.SetBinding(Label.TextProperty, "Latitude");

				 Label longitudeLabel = new Label { FontSize = 12 };
				 longitudeLabel.SetBinding(Label.TextProperty, "Longitude");

				 Label descLabel = new Label { FontSize = 12 };
				 descLabel.SetBinding(Label.TextProperty, "Description");

				 var rl = new RelativeLayout { Padding = new Thickness(0, 5) };

				 rl.Children.Add(latitudeLabel, Constraint.RelativeToParent((parent) => parent.Width - latitudeLabel.Text.Length * 4));
				 rl.Children.Add(longitudeLabel, Constraint.RelativeToParent((parent) => latitudeLabel.X), 
					 Constraint.RelativeToView(latitudeLabel, (par, view) => view.Height * 1.5));
				 rl.Children.Add(titleLabel, Constraint.Constant(0), Constraint.Constant(0),
					 Constraint.RelativeToView(latitudeLabel, (par, view) => view.Width - 5));
				 rl.Children.Add(descLabel, Constraint.Constant(0), Constraint.RelativeToParent((parent) => parent.Height / 2));
				 return new ContentView { Content = rl };
			 });
			grid.Children.Add(Cvc, 0, 0);
			Grid.SetColumnSpan(Cvc, 3);
			Cvc.PositionSelected += RecordSelected;
			var dislikeButton = new Button { Text = "Фу", BackgroundColor = Color.Red };
			var pauseButton = new Button { Text = "Пауза" };
			var likeButton = new Button { Text = "Нрав", BackgroundColor = Color.Green };
			grid.Children.Add(dislikeButton, 0, 1);
			grid.Children.Add(pauseButton, 1, 1);
			grid.Children.Add(likeButton, 2, 1);
			Player = CrossSimpleAudioPlayer.Current;
		}

		public async void RecordSelected(object sender, PositionSelectedEventArgs e)
		{
			var cvc = sender as CarouselViewControl;
			var item = cvc.ItemsSource.GetItem(e.NewValue) as Record;
			if (item.IsDownloaded)
			{
				
			}
			else
			{
				var httpConnectorDesc = new HTTPConector();
				httpConnectorDesc.Address = $"record/description";
				var contentDesc = new
				{
					Id = item.Id,
					IsPublic = true
				};
				httpConnectorDesc.Content = ("Record", contentDesc);
				var respDesc = string.Empty;
				try
				{
					respDesc = await httpConnectorDesc.SendAsync<UserMiniSerializer>();
				}
				catch (Exception ex)
				{
					return;
				}
				var respRec = JsonSerializer.Deserialize<Record>(respDesc, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
				item.Description = respRec.Description;
				item.IsDownloaded = true;
			}
			var httpConnector = new HTTPConector();
			httpConnector.Address = $"record/listen";
			var content = new
			{
				Id = item.Id,
				IsPublic = true
			};
			httpConnector.Content = ("Record", content);
			var resp = string.Empty;
			try
			{
				resp = await httpConnector.SendAsync<UserMiniSerializer>(true);
			}
			catch (Exception ex)
			{
				return;
			}
			//var path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, item.Id.ToString() + ".wav");
			//if (System.IO.File.Exists(path))
			//	System.IO.File.Delete(path);
			//var file = System.IO.File.Create(path);
			//var ar = await httpConnector.Response.Content.ReadAsByteArrayAsync();
			//await file.WriteAsync(ar, 0, ar.Length);
			//file.Close();
			////file.Seek(0, SeekOrigin.Begin);
			///
			if (Device.RuntimePlatform == Device.Android)
			{
				var path = Path.Combine(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).AbsolutePath, "123.wav");
				var f = new File(path);
				if (f.Exists())
					f.Delete();
				f.CreateNewFile();
				var fd = new Java.IO.FileWriter(f);
				//{
					var bytes = await httpConnector.Response.Content.ReadAsByteArrayAsync();
					var chars = System.Text.Encoding.ASCII.GetChars(bytes);
					fd.Write(chars, 0, chars.Length);
				//}
				IMediaItem media = new MediaItem();
				await MediaManager.CrossMediaManager.Current.Play(new FileInfo(f.AbsolutePath));



				//MediaPlayer mp = new MediaPlayer();

				//var mp = new MediaPlayer();
				//mp.SetDataSource(f.AbsolutePath);

				//mp.
				//mp.Prepared += (sender, args) =>
				//{
				//	mp.Start();
				//};

				//mp.PrepareAsync();
				//Player.Play();
			}
			else
			{
				Player.Load(await httpConnector.Response.Content.ReadAsStreamAsync());
				Player.Play();
			}
		}
	}
}