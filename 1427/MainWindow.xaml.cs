using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Logic;
using PDTUtils.Native;


namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		bool Connected = false;
		string errorMessage = "";
		DoorAndKeyStatus k = new DoorAndKeyStatus();
		BackgroundWorker w = new BackgroundWorker();
		ErrorLog ErrorLogText = new ErrorLog();
		System.Timers.Timer doorStatusTimer;
		System.Timers.Timer uiUpdateTimer;
		Thread t;
		MachineIni _machineIni = new MachineIni();
		UniqueIniCategory _uniqueIniCategory = new UniqueIniCategory();

		public MainWindow()
        {
            InitializeComponent();
            RowOne.Height = new GridLength(75);
			ColumnOne.Width = new GridLength(200);
			this.Loaded += new RoutedEventHandler(WindowMain_Loaded);
			//this.Cursor = Cursors.None;
        }

		public MachineIni GetMachineIni
		{ 
			get { return _machineIni; } 
		}

		public UniqueIniCategory GetUniqueCategories
		{
			get { return _uniqueIniCategory; }
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
				string filename = "D:\\1199\\1199L27U010R.exe";
				MessageBox.Show(FileHashing.GetFileHash(filename), "MD5 " + filename, MessageBoxButton.OK);

				if (lblUptime.IsEnabled == false)
				{
					lblUptime.Visibility = System.Windows.Visibility.Visible;
					lblUptime.IsEnabled = true;
					lblUptime.FontSize = 36;
					lblUptime.Foreground = Brushes.Magenta;
					lblUptime.Background = Brushes.Black;
				//	GetSystemUptime();
				}
			}
			catch (SystemException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void GetSystemUptime()
		{
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			var u = uptimeSpan.ToString().Split(".".ToCharArray());
			lblUptime.Content = u[0];
		}

		private void WindowMain_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				//InitialiseBoLib();
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

		private void Logfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
			int con = Convert.ToInt32(b.Content);
			con += 1;
			b.Content = this.VisualChildrenCount.ToString();
			//b.Content = con.ToString();
		}

		private void modifySettingsButton_Click(object sender, RoutedEventArgs e)
		{
			_machineIni.ParseIni();
			MessageBox.Show("Machine RTP " + _machineIni.GetIniValue("Datapack.Dpercentage") + "%");
			MessageBox.Show("Machine Number " + _machineIni["Server.Machine Number"]);

			// commit changes to memory
		}

		private void btnMachineIni_Click(object sender, RoutedEventArgs e)
		{
			RemoveChildrenFromStackPanel();
		
			_machineIni.ParseIni();
			_uniqueIniCategory.Find(_machineIni);

			stpButtonPanel.Orientation = Orientation.Vertical;
	
			MachineIniCategorys.IsEnabled = true;
			MachineIniCategorys.Visibility = Visibility.Visible;
			var secondOrigParent = VisualTreeHelper.GetParent(MachineIniCategorys);
			var secondParentPanel = secondOrigParent as Panel;
			secondParentPanel.Children.Remove(MachineIniCategorys);
			stpButtonPanel.Children.Add(MachineIniCategorys);

			MachineIniListView.IsEnabled = true;
			MachineIniListView.Visibility = Visibility.Visible;
			var originalParent = VisualTreeHelper.GetParent(MachineIniListView);
			var parentAsPanel = originalParent as Panel;
			parentAsPanel.Children.Remove(MachineIniListView);
			stpButtonPanel.Children.Add(MachineIniListView);
		}

		private void RemoveChildrenFromStackPanel()
		{
			int childCount = stpButtonPanel.Children.Count;
			if (childCount > 0)
			{
				stpButtonPanel.Children.RemoveRange(0, childCount);
			}
		}
			/*ManagementClass W32_OS = new ManagementClass("Win32_OperatingSystem");
			ManagementBaseObject inParams, outParams;
			int result;
			W32_OS.Scope.Options.EnablePrivileges = true;

			foreach(ManagementObject obj in W32_OS.GetInstances())
			{
				inParams = obj.GetMethodParameters("Win32Shutdown");
				inParams["Flags"] = 6; //ForcedReboot;
				inParams["Reserved"] = 0;

				outParams = obj.InvokeMethod("Win32Shutdown", inParams, null);
				result = Convert.ToInt32(outParams["returnValue"]);
				if (result != 0) 
					throw new Win32Exception(result);
			}
			 */
		//}
    }
}
