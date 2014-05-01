using System;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
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
				int shell =  BoLib.Bo_SetEnvironment();
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
					throw new System.Exception();
				}
			}
		}

		public delegate void DelegateUpdate();
		public void DoorTimerEvent(object sender, ElapsedEventArgs e)
		{
			this.lblDoorStatus.Dispatcher.Invoke((DelegateUpdate)UpdateDoorStatusLabel);
		}

		public void UpdateUiLabels(object sender, ElapsedEventArgs e)
		{
			this.lblUptime.Dispatcher.Invoke((DelegateUpdate)GetSystemUptime);
		}

		void UpdateDoorStatusLabel()
		{
			string status = "Door Status : ";
			if (BoLib.Bo_GetDoorStatus() == 0)
			{
				//DetectDoorChange(@"./wav/util_exit.wav");	
				status += "Closed";
				lblDoorStatus.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				lblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
			}
			else
			{
				//DetectDoorChange(@"./wav/util_exit.wav");
				status += "Open";
				lblDoorStatus.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				lblDoorStatus.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				lblDoorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
			}
			lblDoorStatus.Content = status;
		}

		private void DetectDoorChange(string filename)
		{
			if (m_keyDoorWorker.HasChanged == true)
			{
				PlaySoundOnEvent(filename);
				m_keyDoorWorker.HasChanged = false;
			}
		}

		/// <summary>
		/// Draw the option buttons for the suitable door status or
		/// the users card level.
		/// </summary>
		private void DetectDoorStatus()
		{
			if (m_keyDoorWorker.DoorStatus == false) // door closed.
			{

			}
			else // door open.
			{

			}
		}

		private void ChangeVolume(int newVolume)
		{
			int volume = BoLib.BO_GetLocalMasterVolume();
			//setlocalvolume(volume - 5);
		}

		private void PlaySoundOnEvent(string filename)
		{
			SoundPlayer sound = new SoundPlayer(filename);
			sound.Play();
		}

		private void PresentErrorLog()
		{
			var txtErrorLog = GetTabTextBlock(Brushes.Black, Brushes.LightGoldenrodYellow);
			
			string errLogLocation = @"D:\machine\GAME_DATA\TerminalErrLog.log";
			settingsTab.Visibility = Visibility.Visible;
			try
			{
				string[] lines = System.IO.File.ReadAllLines(errLogLocation);
				string[] reveresed = new string[lines.Length - 1];

				int ctr = 0;
				for (int i = lines.Length - 1; i > 0; i--)
				{
					reveresed[ctr] = lines[i];
					ctr++;
				}

				txtErrorLog.Text += "Date\t\t  ErrCode\tDescription\r\n";
				foreach (string s in reveresed)
				{
					try
					{
						var subStr = s.Split("\t ".ToCharArray());
						bool? b = s.Contains("TimeStamp");
						if (b == false && s != "")
							txtErrorLog.Text += s + "\r\n";
					}
					catch (System.Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception)
			{
				txtErrorLog.Text += "Could not load file " + errLogLocation + "\r\n";
			}
		}

		private void PresentLastGames()
		{
			TextBlock tb = GetTabTextBlock(Brushes.RosyBrown, Brushes.BurlyWood);
			for (int i = 0; i < 10; i++)
			{
				tb.Text += "I = " + i.ToString() + " : ";
				tb.Text += BoLib.Bo_GetLastGame(i) + "\r\n";
			}
		}

		private void PresentWinningGames()
		{
			TextBlock tb = GetTabTextBlock(Brushes.LightBlue, Brushes.Salmon);
			for (int i = 0; i < 10; i++)
			{
				if (BoLib.Bo_GetWinningGame(i) != "")
				{
					tb.Text += "I = " + i.ToString() + " : ";
					tb.Text += BoLib.Bo_GetWinningGame(i) + "\r\n";
				}
			}
		}

		private ScrollViewer CreateSimpleTabTextBlock(SolidColorBrush bg, SolidColorBrush fg)
		{
			TextBlock txtContentBlock = new TextBlock();
			txtContentBlock.Text = "";
			txtContentBlock.FontSize = 20;
			txtContentBlock.Foreground = fg;
			txtContentBlock.Background = bg;

			ScrollViewer sv = new ScrollViewer();
			sv.Content = txtContentBlock;

			return sv;
		}

		private TextBlock GetTabTextBlock(SolidColorBrush bg, SolidColorBrush fg)
		{
			TabItem selItem = settingsTab.SelectedItem as TabItem;
			selItem.Content = CreateSimpleTabTextBlock(bg, fg);
			ScrollViewer sv = selItem.Content as ScrollViewer;
			return sv.Content as TextBlock;
		}
	}
}
