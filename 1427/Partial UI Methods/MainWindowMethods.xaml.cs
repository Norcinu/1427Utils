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

                        //BoLib.setUtilBit((int)UtilBits.Allow);
                        BoLib.setUtilRequestBitState((int)UtilBits.Allow);
                        
                        if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
                        {
                            _isSpanishMachine = false;
                            regionalSubTab.SelectedIndex = 1; //region settings dont exist so set focus to next tab.
                        }
                        
                        _doorStatusTimer = new Timer() { Interval = 500, Enabled = true };
                        _doorStatusTimer.Elapsed += DoorTimerEvent;
                        
                        _uiUpdateTimer = new Timer() { Interval = 1000, Enabled = true };
                        _uiUpdateTimer.Elapsed += UpdateUiLabels;
                        return;
                    case 1:
                        _errorMessage = "Could not connect to Shell. Check If Running.";
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
			LblBottom.Content = DateTime.Now.ToLongDateString() + " - " + 
                DateTime.Now.ToLongTimeString() + " :::: Uptime ";
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			var s = string.Format("{0:H:mm:ss}", new DateTime(uptimeSpan.Ticks));
			LblBottom.Content += s;
		}
		
		void UpdateDoorStatusLabel()
		{
			var status = "Door Status : ";
			if (BoLib.getDoorStatus() == 0)
			{
				if (_keyDoorWorker.HasChanged)
				{
					_keyDoorWorker.HasChanged = false;
				}
				status += "Closed";
				LblDoorStatus.Background = Brushes.Black;
				LblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
			}
			else
			{
				if (_keyDoorWorker.HasChanged)
				{
                    PlaySoundOnEvent(Properties.Resources.door_open_sound);
					_keyDoorWorker.HasChanged = false;
				}
                status += "Open";
				LblDoorStatus.HorizontalContentAlignment = HorizontalAlignment.Center;
				LblDoorStatus.Background = Brushes.Aquamarine;
				LblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
			}
			LblDoorStatus.Content = status;
		}
        
		void DetectDoorChange(object sender, ElapsedEventArgs e)
		{
		    if (!_keyDoorWorker.HasChanged) return;
#if !DEBUG
            PlaySoundOnEvent(Properties.Resources.door_open_sound);
#endif
		    _keyDoorWorker.HasChanged = false;
            //if (_keyDoorWorker.DoorStatus == false)
		    //    BoLib.disableNoteValidator();
		}
        
		void ChangeVolume(int newVolume)
		{
			var volume = BoLib.getLocalMasterVolume();
		}
		
		void PlaySoundOnEvent(string filename)
		{
			System.Media.SoundPlayer sound = new System.Media.SoundPlayer(filename); 
			sound.Play();
		}
	}
}
