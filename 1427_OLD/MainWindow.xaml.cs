using System;
using System.Windows;
using System.Collections.Generic;

using PDTUtils.Native;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private string error_message = "";

        public MainWindow()
        {
            InitializeComponent();
            RowOne.Height = new GridLength(75);
            ColumnOne.Width = new GridLength(200);

			this.Loaded += new RoutedEventHandler(WindowMain_Loaded);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
        }

		private void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			if (MyTab.Visibility == System.Windows.Visibility.Visible)
				MyTab.Visibility = System.Windows.Visibility.Hidden;
			else
				MyTab.Visibility = System.Windows.Visibility.Visible;
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			Logfile.Visibility = System.Windows.Visibility.Visible;
			string[] lines = System.IO.File.ReadAllLines(@"D:\\machine\\GAME_DATA\\TerminalErrLog.log");
			string[] reveresed = new string[lines.Length];
						
			int ctr = 0;
			for (int i = lines.Length-1; i > 0; i--)
			{
				reveresed[ctr] = lines[i];
				ctr++;
			}
			
			foreach (string s in reveresed)
			{
				try
				{
					bool? b = s.Contains("TimeStamp");
					if (b == false)
						txtErrorLog.Text += s + "\r\n";
				}
				catch (System.Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private void btnHopperOK_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
				MyTab.Visibility = System.Windows.Visibility.Hidden;
		}

		private void Games_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (BoLibNative.Bo_RefillKeyStatus() == 0)
					MessageBox.Show("Refill Key Off");
				else
					MessageBox.Show("Refill Key on");
			}
			catch (SystemException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void WindowMain_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				InitialiseBoLib();
			}
			catch (Exception err)
			{
				MessageBox.Show("Error: " + err.ToString());
			}
		}

		private void InitialiseBoLib()
		{
			int shell = 0;// BoLibNative.Bo_SetEnvironment();

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

			if (MessageBox.Show(error_message, "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
			{
				Application.Current.Shutdown();
			}
		}
    }
}
