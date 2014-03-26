using System;
using System.Windows;
using System.Collections.Generic;
using PDTUtils.Native;

namespace PDTUtils
{
	public partial class MainWindow : Window
	{
		private void InitialiseBoLib()
		{
			try
			{
				int shell = BoLibNative.Bo_SetEnvironment();
				if (shell == 0)
				{
					//UpdateValues();
					//Connected = true;
					//timer = new System.Timers.Timer(1000);
					//timer.Elapsed += UpdateTimer;
					//timer.Enabled = true;
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
				}
			}
			catch (Exception ex)
			{
				//	if (MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK,
				//					MessageBoxImage.Error, MessageBoxResult.OK) == MessageBoxResult.OK)
				//{
				//	Application.Current.Shutdown();
				//}
			}
		}

		private void PresentLastGames()
		{

		}

		private void PresentErrorLog()
		{
			string err_log_location = @"D:\\machine\\GAME_DATA\\TerminalErrLog.log";
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

				txtErrorLog.Text += "Date\t\t\t  ErrCode\tDescription\r\n";
				foreach (string s in reveresed)
				{
					try
					{
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