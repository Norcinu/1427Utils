using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using PDTUtils.Native;
using System.Windows.Interop;


namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		bool Connected = false;
		string error_message = "";
		DoorAndKeyStatus k = new DoorAndKeyStatus();
		BackgroundWorker w = new BackgroundWorker();
		ErrorLog ErrorLogText = new ErrorLog();
		System.Timers.Timer timer;
		Thread t;
		
		public MainWindow()
        {
            InitializeComponent();
            RowOne.Height = new GridLength(75);
			ColumnOne.Width = new GridLength(200);
			this.Loaded += new RoutedEventHandler(WindowMain_Loaded);
			//this.Cursor = Cursors.None;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
			Application.Current.Shutdown();
        }

		private void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			if (MyTab.Visibility == Visibility.Visible)
			{
				MyTab.IsEnabled = false;
				MyTab.Visibility = Visibility.Hidden;
			}
			else
			{
				MyTab.IsEnabled = true;
				MyTab.Visibility = Visibility.Visible;
			}
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			if (!Logfile.IsEnabled)
			{
				Logfile.IsEnabled = true;
				Logfile.Visibility = Visibility.Hidden;

				if (Logfile.SelectedItem == tabErrorLog)
					PresentErrorLog();				
			}
		}

		private void btnHopperOK_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				MyTab.IsEnabled = false;
				MyTab.Visibility = Visibility.Hidden;
			}
		}

		private void Games_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (BoLib.Bo_RefillKeyStatus() == 0)
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
				t = new Thread(new ThreadStart(k.Run));
				t.Start();
				while (!t.IsAlive);
				Thread.Sleep(2);
			}
			catch (Exception err)
			{
				MessageBox.Show("Error: " + err.ToString());
			}
		}

		private void Logfile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (Logfile.IsEnabled)
			{
				if (Logfile.SelectedItem == tabErrorLog)
					PresentErrorLog();
				else if (Logfile.SelectedItem == tabLastGamesLog)
					PresentLastGames();
				else if (Logfile.SelectedItem == tabWinGamesLog)
					tabWinGamesLog.Content = "Wins Go Here!";
			}
		}

		private void WindowMain_Closing(object sender, CancelEventArgs e)
		{
			if (k.Running == true)
				k.Running = false;

			if (t != null)
			{
				if (t.IsAlive)
				{
					t.Abort();
					t.Join();
				}
			}

			if (Connected)
				BoLib.Bo_Shutdown();
		}
    }
}
