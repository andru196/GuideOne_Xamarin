using Plugin.AudioRecorder;
using Xamarin.Forms.Maps;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using static GuideOne_Xamarin.MainPage;

namespace GuideOne_Xamarin.Components
{
	public class Recorder : ContentView
	{
		public MainPage.MainPageStatus? PageState { get; set;}

		public Label TimerLabel { get; private set; }
		public Entry Title { get; private set; }
		public Editor Description { get; private set; }

		public Button PhotoButton { get; private set; }
		public Button PauseButton { get; private set; }
		public Button ContintueButton { get; private set; }
		public Button PlayPauseButton { get; private set; }
		public Button ReadyButton { get; private set; }
		public Position Position { get; set; }

		double _recordTimer = 0;
		AudioStreamDetails _audioStream;
		AudioRecorderService _audioRecorderService;
		AudioPlayer _player;

		public delegate void RecordHandler(object sender, Stream e);
		public event RecordHandler RecordReady;
		public Recorder()
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
			var recBox = new StackLayout { BackgroundColor = Color.FromHex("#AAFFFFFF") };

			grid.Children.Add(recBox, 0, 0);
			Grid.SetColumnSpan(recBox, 3);
			
			PhotoButton = new Button { Text = "Фото" };
			PauseButton = new Button { Text = "Пауза" };
			ReadyButton = new Button { Text = "Готово" };
			
			PlayPauseButton = new Button { Text = "Play", IsVisible = false, IsEnabled = false };
			ContintueButton = new Button { Text = "Продолжить", IsVisible = false, IsEnabled = false };
			grid.Children.Add(PhotoButton, 0, 1);
			grid.Children.Add(PauseButton, 1, 1);
			grid.Children.Add(ContintueButton, 1, 1);
			grid.Children.Add(PlayPauseButton, 1, 1);
			grid.Children.Add(ReadyButton, 2, 1);
			grid.BackgroundColor = Color.FromHex("#88FFFFFF");
			Content = f;
			f.Content = (grid);
			
			TimerLabel = new Label() { Text = "00:00" };
			Title = new Entry() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), MaxLength = 200, Placeholder = "Название записи" };
			Description = new Editor() { VerticalOptions = LayoutOptions.FillAndExpand, Placeholder = "Описание" };
			recBox.Children.Add(TimerLabel);
			recBox.Children.Add(Title);
			recBox.Children.Add(Description);
			PauseButton.Clicked += SwitchPauseContinue;
			ContintueButton.Clicked += SwitchPauseContinue;
			PlayPauseButton.Clicked += PlayPauseButtonClicked;


			_audioRecorderService = new AudioRecorderService
			{
				AudioSilenceTimeout = TimeSpan.FromSeconds(5),
				StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
				StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
				TotalAudioTimeout = TimeSpan.FromMinutes(10) //audio will stop recording after 15 seconds
			};
			_audioRecorderService.AudioInputReceived += Recorder_AudioInputReceived;

			PauseButton.Clicked += OnPauseButtonClicked;
			ContintueButton.Clicked += OnContinueButtonClicked;
			ReadyButton.Clicked += OnReadyButtonClicked;
		}
		private void Recorder_AudioInputReceived(object sender, string audioFile)
		{
			_player = new AudioPlayer();
			_player.Play(audioFile);
			_player.Pause();
			_player.FinishedPlaying += (e, o) => PlayPauseButton.Text = "Play";
			if (PageState == MainPageStatus.MakeRecord)
				PageState = MainPageStatus.ComplateRecord;
			PauseButton.IsEnabled = false;
			PauseButton.IsVisible = false;
			ContintueButton.IsEnabled = false;
			ContintueButton.IsVisible = false;
			PlayPauseButton.IsVisible = true;
			PlayPauseButton.IsEnabled = true;
		}

		async void OnPauseButtonClicked(object sender, EventArgs e)
		{
			PageState = MainPageStatus.PauseRecord;
			await _audioRecorderService.StopRecording();
		}

		public async Task StartRecord()
		{
			await _audioRecorderService.StartRecording();
			_audioStream = _audioRecorderService.AudioStreamDetails;
			StartTimer();
		}

		async void OnContinueButtonClicked(object sender, EventArgs e)
		{
			StartTimer();
			PageState = MainPageStatus.MakeRecord;
			await _audioRecorderService.StartRecording();
		}
		async void OnReadyButtonClicked(object sender, EventArgs e)
		{
			if (PageState != MainPageStatus.ComplateRecord)
			{
				PageState = MainPageStatus.ComplateRecord;
				if (_audioRecorderService.IsRecording)
					await _audioRecorderService.StopRecording();
			}
			if ((Title.Text?.Length ?? 0) < 6)
				Title.BackgroundColor = Color.Red;
			else
				RecordReady.Invoke(this, _audioRecorderService.GetAudioFileStream());

		}


		void PlayPauseButtonClicked(object sender, EventArgs e)
		{
			if (((Button)sender).Text == "Play")
			{
				_player.Play();
				((Button)sender).Text = "Pause";
			} else
			{
				_player.Pause();
				((Button)sender).Text = "Play";
			}
		}

		

		void SwitchPauseContinue(object sender, EventArgs e)
		{
			((Button)sender).IsVisible = false;
			((Button)sender).IsEnabled = false;
			if (sender == PauseButton)
			{
				ContintueButton.IsVisible = true;
				ContintueButton.IsEnabled = true;
			}
			else
			{
				PauseButton.IsVisible = true;
				PauseButton.IsEnabled = true;
			}
		}
		void StartTimer()
		{
			var timerStep = 98.3;
			Device.StartTimer(TimeSpan.FromMilliseconds(timerStep), () => {
				if ((PageState & MainPageStatus.MakeRecord) > 0)
				{
					var ts = TimeSpan.FromMilliseconds(_recordTimer++ * timerStep);
					this.TimerLabel.Text = ts.ToString("mm\\:ss\\:ff");
					return ts.Minutes < 10;
				}
				else
					return false;
			});
		}
	}
}