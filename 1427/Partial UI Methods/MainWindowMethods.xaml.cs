using System;
using System.Diagnostics;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using PDTUtils.Native;


namespace PDTUtils
{
	public partial class MainWindow : Window
	{
		private void InitialiseBoLib()
		{
			try
			{
				int shell = BoLib.setEnvironment();
				if (shell == 0)
				{
					_sharedMemoryOnline = true;
                    
				//	UpdateDoorStatusLabel();
					_doorStatusTimer = new System.Timers.Timer(500);
                    _doorStatusTimer.Elapsed += DoorTimerEvent;
					_doorStatusTimer.Enabled = true;
                    
				//	GetSystemUptime();
					_uiUpdateTimer = new System.Timers.Timer(1000);
					_uiUpdateTimer.Elapsed += UpdateUiLabels;
					_uiUpdateTimer.Enabled = true;
                    
					return;
				}
				else if (shell == 1)
					_errorMessage = "Shell Out of Date. Check If Running.";
				else if (shell == 2)
					_errorMessage = "Bo Lib Out of Date.";
				else
					_errorMessage = "Unknown Error Occurred.";

				_errorMessage += "\nFix the issue and restart program.";

				if (MessageBox.Show(_errorMessage, "Error", MessageBoxButton.OK,
									MessageBoxImage.Error, MessageBoxResult.OK) == MessageBoxResult.OK)
				{
					Application.Current.Shutdown();
					throw new System.Exception();
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK,
									MessageBoxImage.Error, MessageBoxResult.OK) == MessageBoxResult.OK)
				{
					Application.Current.Shutdown();
				}
			}
		}

		public delegate void DelegateUpdate();
        public void DoorTimerEvent(object sender, ElapsedEventArgs e)
        {
            if (LblDoorStatus != null)
                LblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
            if (LblBottom != null)
                LblBottom.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
        }
        
		public void UpdateUiLabels(object sender, ElapsedEventArgs e)
		{
            this.LblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
            this.LblBottom.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
		}
        
		public void UpdateTimeAndDate()
		{
			LblBottom.FontSize = 22;
			LblBottom.Foreground = Brushes.Pink;
			LblBottom.Content = DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + " :::: Uptime ";
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			string s = string.Format("{0:H:mm:ss}", new DateTime(uptimeSpan.Ticks));
			LblBottom.Content += s;
		}
		
		void UpdateDoorStatusLabel()
		{
			string status = "Door Status : ";
			if (BoLib.getDoorStatus() == 0)
			{
				if (_keyDoorWorker.HasChanged == true)
				{
					BoLib.enableNoteValidator();
					_keyDoorWorker.HasChanged = false;

				}
				//DetectDoorChange(@"./wav/util_exit.wav");	
				status += "Closed";
				LblDoorStatus.Background = Brushes.Black;//new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				LblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
			}
			else
			{
				if (_keyDoorWorker.HasChanged == true)
				{
					BoLib.disableNoteValidator();
					_keyDoorWorker.HasChanged = false;
				}
				//DetectDoorChange(@"./wav/util_exit.wav");
				status += "Open";
				LblDoorStatus.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				LblDoorStatus.Background = Brushes.Aquamarine;
				LblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
			}			
			LblDoorStatus.Content = status;
		}
        
		private void DetectDoorChange(object sender, ElapsedEventArgs e)
		{
			if (_keyDoorWorker.HasChanged == true)
			{
				PlaySoundOnEvent(Properties.Resources.util_exit.ToString());
				_keyDoorWorker.HasChanged = false;
				if (_keyDoorWorker.DoorStatus == false)
					BoLib.disableNoteValidator();
			}
		}
		
		private void ChangeVolume(int newVolume)
		{
			int volume = BoLib.getLocalMasterVolume();
		}
		
		private void PlaySoundOnEvent(string filename)
		{
			// replace this with synth_sound? // if (AdjustingVolume) synth else soundplayer
			//SoundPlayer sound = new SoundPlayer(filename); 
			//sound.Play();
		}
	}
}
