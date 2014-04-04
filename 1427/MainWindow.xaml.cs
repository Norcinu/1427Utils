using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using PDTUtils.Native;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;


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
			if (stpButtonPanel.Children.Count == 0) // need to clear these when other button is pressed.
			{
				for (int i = 0; i < 5; i++)
				{
					Button b = new Button();
					b.Content = i.ToString();
					b.Name = "Button" + i.ToString();
					b.Click += new RoutedEventHandler(dynamicButton_Click);
					stpButtonPanel.Children.Add((UIElement)b);
				}
			}
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			if (!settingsTab.IsEnabled)
			{
				settingsTab.IsEnabled = true;
				settingsTab.Visibility = Visibility.Hidden;

				var headers = new Dictionary<string, string>();
				headers.Add("tabErrorLog", "Error Log");
				headers.Add("tabGameLog", "Last Game Log");
				headers.Add("tabWinLog", "Wins Log");
				var brushes = new SolidColorBrush[3]{Brushes.Red, Brushes.Yellow, Brushes.Green};

				var i = 0;
				foreach (var entry in headers)
				{
					TabItem tab = new TabItem();
					tab.Name = entry.Key;
					tab.Header = entry.Value;
					tab.Foreground = brushes[i++];
					settingsTab.Items.Add(tab);
				}

				PresentErrorLog();
			}
		}

		private void btnHopperOK_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				settingsTab.IsEnabled = false;
				settingsTab.Visibility = Visibility.Hidden;
			}
		}

		private void Games_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (BoLib.Bo_RefillKeyStatus() == 0)
					MessageBox.Show("Refill Key Off");
				else
					MessageBox.Show("Refill Key On");
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
			if (settingsTab.IsEnabled)
			{
				TabItem item = settingsTab.SelectedItem as TabItem;
				
				if (item.Name == "tabErrorLog")
					PresentErrorLog();
				else if (item.Name == "tabGameLog")
					PresentLastGames();
				else if (item.Name == "tabWinLog")
					PresentWinningGames();
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

		private void dynamicButton_Click(Object sender, EventArgs e)
		{
			Button b = sender as Button;
			// identify button and do the relevant stuff
			int con = Convert.ToInt32(b.Content);
			con += 1;
			b.Content = con.ToString();
		}

		private void updateUiControls(object sender)
		{

		}
    }
}
