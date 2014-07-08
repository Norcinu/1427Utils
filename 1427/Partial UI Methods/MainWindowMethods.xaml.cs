using System;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Native;
using System.Diagnostics;

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
					m_connected = true;
					
					UpdateDoorStatusLabel();
					m_doorStatusTimer = new System.Timers.Timer(500);
					m_doorStatusTimer.Elapsed += DoorTimerEvent;
					m_doorStatusTimer.Enabled = true;

					GetSystemUptime();
					m_uiUpdateTimer = new System.Timers.Timer(1000);
					m_uiUpdateTimer.Elapsed += UpdateUiLabels;
					//m_uiUpdateTimer.Elapsed += DetectDoorChange;
					m_uiUpdateTimer.Enabled = true;
					
					return;
				}
				else if (shell == 1)
				{
					m_errorMessage = "Shell Out of Date. Check If Running.";
				}
				else if (shell == 2)
				{
					m_errorMessage = "Bo Lib Out of Date.";
				}
				else
				{
					m_errorMessage = "Unknown Error Occurred.";
				}

				m_errorMessage += "\nFix the issue and restart program.";

				if (MessageBox.Show(m_errorMessage, "Error", MessageBoxButton.OK,
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
					throw new System.Exception(); // is this needed?
				}
			}
		}

		public delegate void DelegateUpdate();
		public void DoorTimerEvent(object sender, ElapsedEventArgs e)
		{
			this.lblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
			this.lblBottom.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
		}

		public void UpdateUiLabels(object sender, ElapsedEventArgs e)
		{
			//this.lblUptime.Dispatcher.Invoke((DelegateUpdate)GetSystemUptime);
		}

		public void UpdateTimeAndDate()
		{
			lblBottom.FontSize = 22;
			lblBottom.Foreground = Brushes.Pink;
			lblBottom.Content = DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + "\t\t ::::\t\t Uptime ";
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			string s = string.Format("{0:H:mm:ss}", new DateTime(uptimeSpan.Ticks));
			lblBottom.Content += s;
		}
		
		void UpdateDoorStatusLabel()
		{
			string status = "Door Status : ";
			if (BoLib.getDoorStatus() == 0)
			{
				if (m_keyDoorWorker.HasChanged == true)
				{
					BoLib.enableNoteValidator();
					m_keyDoorWorker.HasChanged = false;

				}
				//DetectDoorChange(@"./wav/util_exit.wav");	
				status += "Closed";
				lblDoorStatus.Background = Brushes.Black;//new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				lblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
			}
			else
			{
				if (m_keyDoorWorker.HasChanged == true)
				{
					BoLib.disableNoteValidator();
					m_keyDoorWorker.HasChanged = false;
				}
				//DetectDoorChange(@"./wav/util_exit.wav");
				status += "Open";
				lblDoorStatus.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				lblDoorStatus.Background = Brushes.Aquamarine;
				lblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
			}			
			lblDoorStatus.Content = status;
		}

		private void DetectDoorChange(object sender, ElapsedEventArgs e)
		{
			if (m_keyDoorWorker.HasChanged == true)
			{
				PlaySoundOnEvent(Properties.Resources.util_exit.ToString());
				m_keyDoorWorker.HasChanged = false;
				if (m_keyDoorWorker.DoorStatus == false)
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
			SoundPlayer sound = new SoundPlayer(filename); 
			sound.Play();
		}
	}
}
