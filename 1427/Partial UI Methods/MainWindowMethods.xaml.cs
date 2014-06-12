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
			this.lblTop.Dispatcher.Invoke((DelegateUpdate)UpdateTimeAndDate);
		}

		public void UpdateUiLabels(object sender, ElapsedEventArgs e)
		{
			this.lblUptime.Dispatcher.Invoke((DelegateUpdate)GetSystemUptime);
		}

		public void UpdateTimeAndDate()
		{
			lblTop.FontSize = 22;
			lblTop.Foreground = Brushes.Pink;
			lblTop.Content = DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString();
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
				lblDoorStatus.Background = Brushes.Aquamarine;//new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
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
			int volume = BoLib.getLocalMasterVolume();
		}

		private void PlaySoundOnEvent(string filename)
		{
			// replace this with synth_sound? // if (AdjustingVolume) synth else soundplayer
			SoundPlayer sound = new SoundPlayer(filename); 
			sound.Play();
		}

		private void PresentErrorLog()
		{
			var txtErrorLog = GetTabTextBlock(Brushes.Black, Brushes.LightGoldenrodYellow);
			string errLogLocation = @"D:\machine\GAME_DATA\TerminalErrLog.log";
			settingsTab.Visibility = Visibility.Visible;
			//txtErrorLog.ActualHeight = MyLogfiles.ActualHeight;
			//txtErrorLog.Width = MyLogfiles.ActualWidth;
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
			//tb.Width = MyLogfiles.ActualWidth;
			//tb.Height = MyLogfiles.Height;
			var textContent = new string[10];
			int MaxLength = 0;
			for (int i = 0; i < 10; i++)
			{
				textContent[i] = i.ToString() + " : " + BoLib.getLastGame(i);
				int tempLength = textContent[i].Length;
				if (tempLength > MaxLength)
					MaxLength = tempLength;
			}

			for (int i = 0; i < 10; i++)
			{
				if (textContent[i].Length < MaxLength)
				{
					int diff = MaxLength - textContent[i].Length;
					for (int j = 0; j < diff; j++)
						textContent[i] += ".";
				}
			}

			for (int i = 0; i < 10; i++)
			{
			
				tb.Text += textContent[i] + "\r\n";
			}
		}

		private void SetMultipleVars(out int one, out int two)
		{
			one = 1;
			two = 2;
		}

		private void PresentWinningGames()
		{
			TextBlock tb = GetTabTextBlock(Brushes.LightBlue, Brushes.Salmon);
			int counter = 0;
			for (int i = 0; i < BoLib.getNumberOfGames(); i++)
			{
				if (BoLib.getWinningGame(i) > 0)
				{
					var today = DateTime.Today;
					counter++;
					var gameDate = BoLib.getGameDate(i);
					tb.Text += "Win Number " + counter.ToString() + " : ";
					tb.Text += BoLib.getGameModel(i) + " : ";
					uint day = gameDate >> 16;
					uint month = gameDate & 0x0000FFFF;
					int year = (month > today.Month) ? today.Year - 1 : today.Month;
					string win = Convert.ToString(BoLib.getWinningGame(i));
					string final = win.Insert(win.Length - 2, ".");
					string date = Convert.ToString(day) + @"/" + Convert.ToString(month) + @"/" + year;
					tb.Text += date + " : £" + final + "\r\n";
				}
			}
		}

		private ScrollViewer CreateSimpleTabTextBlock(SolidColorBrush bg, SolidColorBrush fg)
		{
			TextBlock txtContentBlock = new TextBlock()
			{
				Text = "",
				FontSize = 20,
				Foreground = fg,
				Background = bg,
				TextAlignment = TextAlignment.Center,
				FontFamily = new FontFamily("Consolas")
			};
			
			//txtContentBlock.FontFamily = new FontFamily("Consolas");
			
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
