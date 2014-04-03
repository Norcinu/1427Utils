using System;
using System.Timers;
using System.Windows;
using PDTUtils.Native;
using System.Windows.Media;

namespace PDTUtils
{
	public partial class MainWindow : Window
	{
		private void InitialiseBoLib()
		{
			try
			{
				int shell = BoLib.Bo_SetEnvironment();
				if (shell == 0)
				{
					UpdateValues();
					Connected = true;
					timer = new System.Timers.Timer(500);
					timer.Elapsed += UpdateTimer;
					timer.Enabled = true;
					
					return;
				}
				else if (shell == 1)
				{
					error_message = "Shell Out of Date. Check If Running.";
				}
				else if (shell == 2)
				{
					error_message = "Bo Lib Out of Date.";
				}
				else
				{
					error_message = "Unknown Error Occurred.";
				}

				error_message += "\nFix the issue and restart program.";

				if (MessageBox.Show(error_message, "Error", MessageBoxButton.OK,
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

		public delegate void delUpdate();
		public void UpdateTimer(object sender, ElapsedEventArgs e)
		{
			this.lblDoorStatus.Dispatcher.Invoke((delUpdate)UpdateValues);
		}

		void UpdateValues()
		{
			string status = "Door Status : ";
			if (BoLib.Bo_GetDoorStatus() == 0)
			{
				status += "Closed";
				SolidColorBrush b = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				SolidColorBrush f = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
				lblDoorStatus.Background = b;
				lblDoorStatus.Foreground = f;
			}
			else
			{
				status += "Open";
				SolidColorBrush b = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
				SolidColorBrush f = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
				lblDoorStatus.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				lblDoorStatus.Background = b;
				lblDoorStatus.Foreground = f;
			}
			lblDoorStatus.Content = status;
		}

		private void PresentLastGames()
		{

		}

		private void PresentErrorLog()
		{
			txtErrorLog.Text = "";
			string err_log_location = @"D:\machine\GAME_DATA\TerminalErrLog.log";
			Logfile.Visibility = Visibility.Visible;
			try
			{
				string[] lines = System.IO.File.ReadAllLines(err_log_location);
				string[] reveresed = new string[lines.Length - 1];

				int ctr = 0;
				for (int i = lines.Length - 1; i > 0; i--)
				{
					reveresed[ctr] = lines[i];
					ctr++;
				}

				//lstErrLog.it

				txtErrorLog.Text += "Date\t\t  ErrCode\tDescription\r\n";
				foreach (string s in reveresed)
				{
					try
					{
						//char[] delims = new char[2]{" ", "\t"};
						var sub_str = s.Split("\t ".ToCharArray());
						
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
				txtErrorLog.Text += "Could not load file " + err_log_location + "\r\n";
			}
		}
	}
}