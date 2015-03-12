using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using PDTUtils.Native;

namespace PDTUtils
{
	public partial class MainWindow
	{
		private void InitialiseBoLib()
		{
			try
			{
				var shell = BoLib.setEnvironment();
				switch (shell)
				{
				    case 0:
				        _sharedMemoryOnline = true;
                    
				        //	UpdateDoorStatusLabel();
				        _doorStatusTimer = new Timer(500);
				        _doorStatusTimer.Elapsed += DoorTimerEvent;
				        _doorStatusTimer.Enabled = true;
                    
				        //	GetSystemUptime();
				        _uiUpdateTimer = new Timer(1000);
				        _uiUpdateTimer.Elapsed += UpdateUiLabels;
				        _uiUpdateTimer.Enabled = true;
                    
				        return;
				    case 1:
				        _errorMessage = "Shell Out of Date. Check If Running.";
				        break;
				    case 2:
				        _errorMessage = "Bo Lib Out of Date.";
				        break;
				    default:
				        _errorMessage = "Unknown Error Occurred.";
				        break;
				}

				_errorMessage += "\nFix the issue and restart program.";

				if (MessageBox.Show(_errorMessage, "Error", MessageBoxButton.OK,
									MessageBoxImage.Error, MessageBoxResult.OK) == MessageBoxResult.OK)
				{
					Application.Current.Shutdown();
					throw new Exception();
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

	    private delegate void DelegateUpdate();

	    private void DoorTimerEvent(object sender, ElapsedEventArgs e)
        {
            if (LblDoorStatus != null)
                LblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
            if (LblBottom != null)
                LblBottom.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
        }

	    private void UpdateUiLabels(object sender, ElapsedEventArgs e)
		{
            LblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
            LblBottom.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
		}
        
		public void UpdateTimeAndDate()
		{
			LblBottom.FontSize = 22;
			LblBottom.Foreground = Brushes.Pink;
			LblBottom.Content = DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + " :::: Uptime ";
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			var s = string.Format("{0:H:mm:ss}", new DateTime(uptimeSpan.Ticks));
			LblBottom.Content += s;
		}
		//looks like goiter
		void UpdateDoorStatusLabel()
		{
			var status = "Door Status : ";
			if (BoLib.getDoorStatus() == 0)
			{
				if (_keyDoorWorker.HasChanged)
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
				if (_keyDoorWorker.HasChanged)
				{
					BoLib.disableNoteValidator();
					_keyDoorWorker.HasChanged = false;
				}
				//DetectDoorChange(@"./wav/util_exit.wav");
				status += "Open";
				LblDoorStatus.HorizontalContentAlignment = HorizontalAlignment.Center;
				LblDoorStatus.Background = Brushes.Aquamarine;
				LblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
			}			
			LblDoorStatus.Content = status;
		}
        
		private void DetectDoorChange(object sender, ElapsedEventArgs e)
		{
		    if (!_keyDoorWorker.HasChanged) return;
		    PlaySoundOnEvent(Properties.Resources.util_exit.ToString());
		    _keyDoorWorker.HasChanged = false;
		    if (_keyDoorWorker.DoorStatus == false)
		        BoLib.disableNoteValidator();
		}
		
		private void ChangeVolume(int newVolume)
		{
			var volume = BoLib.getLocalMasterVolume();
		}
		
		private void PlaySoundOnEvent(string filename)
		{
			// replace this with synth_sound? // if (AdjustingVolume) synth else soundplayer
			//SoundPlayer sound = new SoundPlayer(filename); 
			//sound.Play();
		}
	}
}
