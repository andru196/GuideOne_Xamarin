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
using SkiaSharp;
using Xamarin.Forms.Maps;
using SkiaSharp.Views.Forms;
using Android.Bluetooth;
using Xamarin.Forms.Internals;
using Plugin.AudioRecorder;
using Xamarin.Forms.Markup;
using GuideOne_Xamarin.Helpers;
using GuideOne_Xamarin.Components;
using System.IO;
using GuideOne_Xamarin.Model;
using Newtonsoft.Json.Linq;
using Java.Sql;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json;

namespace GuideOne_Xamarin
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		MainPageStatus State;

		object locker;
		public MainPage()
		{
			State = MainPageStatus.ShowMap;
			InitializeComponent();
			Task.Run(async () =>
			{
				SKCanvasView canvasViewRecordLbl = new SKCanvasView();
				canvasViewRecordLbl.PaintSurface += OnCanvasViewPaintSurface;
				gridMenu.Children.Add(canvasViewRecordLbl, 2, 0);

				var locator = CrossGeolocator.Current;
				var position = await locator.GetLastKnownLocationAsync();
				if (position == null)
					position = await locator.GetPositionAsync(new TimeSpan(10000));
				map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
														 Distance.FromMeters(500)));
				map.MapClicked += OnMapClicked;
				map.PropertyChanged += OnMapChangeProperty;
				mapChangers = new List<object>();
				locker = new object();

			});
			NavigationPage.SetHasNavigationBar(this, false);

		}

		List<object> mapChangers;
		async void OnMapChangeProperty(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "VisibleRegion")
			{
				var wait = Task.Delay(200);
				var tester = new object();
				lock (mapChangers)
					mapChangers.Add(tester);
				await wait; 
				lock (mapChangers)
				{
					if (mapChangers.LastOrDefault() != tester)
						return;
					else
					{
						mapChangers.Clear();
						wait = GetRecordsToTheMap(map.VisibleRegion);
					}
				}
				await wait;
			}
		}

		async Task GetRecordsToTheMap(MapSpan mapRegion)
		{
			{
				//Достаём из БД записи, которые входят в карту и ещё на её не помещались
				var recordsFromDb = App.Database.GetItems<Record>().Where(x =>
				map.VisibleRegion.IsInclude(x.Point) && !map.Pins.Any(p => (p as PinWithEntity)?.Entity.Id == x.Id));
				FillMap(recordsFromDb);
			}
			var httpConnector = new HTTPConector();
			httpConnector.Address = $"record/get";
			var content = new
			{
				Latitude = mapRegion.Center.Latitude,
				Longitude = mapRegion.Center.Longitude,
				Radius = mapRegion.Radius.Meters
			};
			httpConnector.Content = ("MapSpan", content);
			var resp = string.Empty;
			//try
			//{
			//	resp = await httpConnector.SendAsync<UserMiniSerializer>();
			//}
			//catch (Exception ex)
			//{
			//	return;
			//}
			
			//if (resp != null && (httpConnector.Response?.IsSuccessStatusCode ?? false))
			//{
			//	var op = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

			//	var recList = System.Text.Json.JsonSerializer.Deserialize<Record[]>(resp, op)
			//		.Where(x => !map.Pins.Any(p => (p as PinWithEntity)?.Entity?.Id == x.Id));

			//	FillMap(recList);
			//	App.Database.SaveItems(recList.ToList());
			//}
			//foreach (var pin in map.Pins.Where(p => !map.VisibleRegion.IsInclude(p.Position)))
			//	map.Pins.Remove(pin);
			
		}


		void FillMap(IEnumerable<Record> recs)
		{
			foreach (var rec in recs)
			{
				var pin = (new PinWithEntity
				{
					Label = rec.Name,
					Position = rec.Point,
					Type = PinType.Place,
					Entity = rec
				});
				pin.MarkerClicked += OnPinClicked;
				map.Pins.Add(pin);
			}
		}

		async void OnPinClicked(object sender, PinClickedEventArgs e)
		{
			map.PropertyChanged -= OnMapChangeProperty;
			var pin = sender as PinWithEntity;
			pin.Label = "";
			var d = Task.Delay(700);
			var recs = map.Pins.Where(p => pin.Position.Distance(p.Position) <= 90).Select(p => (p as PinWithEntity).Entity as Record).OrderBy(x => pin.Position.Distance(x.Point));
			var len = recs.Count();
			await d;
			var position = pin.Position;
			map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude - 0.0015, position.Longitude),
													 Distance.FromMeters(150)));
			State = MainPageStatus.ShowRecord;
			var playerView = new RecordPlayer(recs);
			relBox.HeightRequest = gridMenu.Height + (MapPage.Height - gridMenu.Height) / 2;
			relBox.Children.Add(playerView
				, Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => parent.Width),
				Constraint.RelativeToView(gridMenu, (parent, menu) =>
				{
					return (parent.HeightRequest - menu.Height);
				})
				);

			map.PropertyChanged += OnMapChangeProperty;
		}

		void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			SKImageInfo info = args.Info;
			SKSurface surface = args.Surface;
			SKCanvas canvas = surface.Canvas;

			canvas.Clear();

			SKPaint paint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				Color = Color.FromRgba(r: 0.7, g: 0, b: 0, a: 0.3).ToSKColor(),
				StrokeWidth = 25
			};
			canvas.DrawCircle(info.Width / 2, info.Height / 2, 50, paint);
			paint.Style = SKPaintStyle.Fill;
			paint.Color = Color.FromRgba(r: 1.0, g: 0, b: 0, a: 0.6).ToSKColor();
			canvas.DrawCircle(info.Width / 2, info.Height / 2, 40, paint);
		}
		async void OnMapClicked(object sender, MapClickedEventArgs e)
		{
			if (State != MainPageStatus.ShowMap)
			{
				var trash = relBox.Children.Where(x => x.ClassId != "def");
				trash.ToList().ForEach(x => relBox.Children.Remove(x)); 
				State = MainPageStatus.ShowMap;
				gridMenu.Children.ForEach(x => x.IsEnabled = true);
			}
			var position = map.VisibleRegion;
		}

		async void OnRecordClicked(object sender, EventArgs e)
		{
			var locator = CrossGeolocator.Current;
			var position = await locator.GetPositionAsync(new TimeSpan(10000));
			map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude - 0.0015, position.Longitude),
													 Distance.FromMeters(150)));

			gridMenu.Children.ForEach(x => x.IsEnabled = false);
			State = MainPageStatus.MakeRecord;
			var recorderView = new Recorder() { Position = new Position(position.Latitude, position.Longitude)};
			relBox.HeightRequest = gridMenu.Height + (MapPage.Height - gridMenu.Height) / 2;
			relBox.Children.Add(recorderView
				, Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => parent.Width),
				Constraint.RelativeToView(gridMenu, (parent, menu) =>
				{
					return (parent.HeightRequest - menu.Height); 
				})
				);
			await recorderView.StartRecord();
			recorderView.PageState = State;
			recorderView.RecordReady += OnRecordReady;
		}

		async void OnRecordReady(object sender, Stream stream)
		{
			var len = stream.Length;

			var bytes = new byte[len];
			stream.Read(bytes, 0, (int)len);
			var recView = (Recorder)sender;
			var recs = App.Database.GetItems<Record>();
			var id = 1;
			if (recs.Any())
				id = recs.Max(x => x.Id) + 1;
			var rec = new Record
			{
				Id = id,
				Name = recView.Title.Text,
				Description = recView.Description.Text,
				UserId = Conf.User.Id,
				Audio = bytes,
				Point = recView.Position
			};
			App.Database.SaveItem(rec);

			var httpConnector = new HTTPConector();
			httpConnector.Address = "record";
			httpConnector.Content = ("Record", rec);
			var resp = string.Empty;
			try
			{
				resp = await httpConnector.SendAsync<UserMiniSerializer>();
			}
			catch (Exception ex)
			{
				return;
			}
			if (httpConnector.Response?.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var jobj = JObject.Parse(resp);
				var jrec = jobj.GetValue("record");
				var recFromServer = jrec.ToObject<Record>();

				App.Database.DeleteItem<Record>(rec.Id);
				rec.Id = recFromServer.Id;
				App.Database.SaveItem(rec);
			}
		}


		public enum MainPageStatus
		{
			ShowMap = 1,
			ShowRecord = 2,
			MakeRecord = 4,
			PauseRecord = 8,
			ComplateRecord = 16
		}
		
	}
}
