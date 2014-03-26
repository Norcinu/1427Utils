using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading;

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
			if (MyTab.Visibility == Visibility.Visible)
				MyTab.Visibility = Visibility.Hidden;
			else
				MyTab.Visibility = Visibility.Visible;
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			//var ptr = BoLibNative.GetLastGames();
			//MessageBox.Show(Convert.ToString(ptr[2]));
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
				MyTab.Visibility = Visibility.Hidden;
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


		private void Logfile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (Logfile.IsEnabled)
			{
				if (Logfile.SelectedItem == tabErrorLog)
					PresentErrorLog();
				else if (Logfile.SelectedItem == tabLastGamesLog)
					PresentLastGames();
				else if (Logfile.SelectedItem == tabWinGamesLog)
				{
					tabWinGamesLog.Content = "Wins Go Here!";
				}
			}
		}
    }
}
