#define REMOTE

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
		bool m_requiresSave = false;
		bool m_connected = false;
		string m_errorMessage = "";
		
		System.Timers.Timer m_doorStatusTimer;
		System.Timers.Timer m_uiUpdateTimer;
		Thread m_keyDoorThread;

		DoorAndKeyStatus m_keyDoorWorker = new DoorAndKeyStatus();
		ErrorLog m_errorLogText = new ErrorLog();
		MachineIni m_machineIni = new MachineIni();
		UniqueIniCategory m_uniqueIniCategory = new UniqueIniCategory();
		MachineGameStatistics m_gameStatistics = new MachineGameStatistics();

		public MainWindow()
        {
            InitializeComponent();
            RowOne.Height = new GridLength(75);
			ColumnOne.Width = new GridLength(200);
			this.Loaded += new RoutedEventHandler(WindowMain_Loaded);
			//this.Cursor = Cursors.None;
        }

		#region Properties
		public bool RequiresSave
		{
			get { return m_requiresSave; }
			set { m_requiresSave = value; }
		}

		public MachineIni GetMachineIni
		{ 
			get { return m_machineIni; } 
		}

		public UniqueIniCategory GetUniqueCategories
		{
			get { return m_uniqueIniCategory; }
		}
		#endregion

		private void btnExit_Click(object sender, RoutedEventArgs e)
        {
			Application.Current.Shutdown();
        }

		private void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			HopperUtilsWindow w = new HopperUtilsWindow(m_keyDoorWorker);
			w.ShowDialog();
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
				string filename = "D:\\1199\\1199L27U010D.exe";
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
#if REMOTE
				InitialiseBoLib();
#endif
				m_keyDoorThread = new Thread(new ThreadStart(m_keyDoorWorker.Run));
				m_keyDoorThread.Start();
				while (!m_keyDoorThread.IsAlive);
				Thread.Sleep(2);

				//BoLib.printTestTicket();
				//if (BoLib.getError() > 0 && BoLib.getDoorStatus() == 0)
				//{
					//MessageBox.Show(BoLib.getErrorText());
					
				//}
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
			if (m_keyDoorWorker.Running == true)
				m_keyDoorWorker.Running = false;

			if (m_keyDoorThread != null)
			{
				if (m_keyDoorThread.IsAlive)
				{
					m_keyDoorThread.Abort();
					m_keyDoorThread.Join();
				}
			}

			if (m_connected)
				BoLib.closeSharedMemory();
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
			m_machineIni.ParseIni();
			MessageBox.Show("Machine RTP " + m_machineIni.GetIniValue("Datapack.Dpercentage") + "%");
			MessageBox.Show("Machine Number " + m_machineIni["Server.Machine Number"]);
			
			// commit changes to memory
		}

		private void btnMachineIni_Click(object sender, RoutedEventArgs e)
		{
			RemoveChildrenFromStackPanel();
			
			m_machineIni.ParseIni();
			m_uniqueIniCategory.Find(m_machineIni);
			stpButtonPanel.Orientation = Orientation.Vertical;
	
			MachineIniCategorys.IsEnabled = true;
			MachineIniCategorys.Visibility = Visibility.Visible;
			var secondOrigParent = VisualTreeHelper.GetParent(MachineIniCategorys);
			var secondParentPanel = secondOrigParent as Panel;
			secondParentPanel.Children.Remove(MachineIniCategorys);
			stpButtonPanel.Children.Add(MachineIniCategorys);
			MachineIniCategorys.SelectedIndex = 0;

			stpButtonPanel.Children.Add(SetupDynamicListBox());
		}


		private bool ValidateNewIniSetting()
		{
			return true;
		}

		private ListBox SetupDynamicListBox()
		{
			ListBox l = new ListBox();
			l.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
			l.FontSize = 22.0;
			var items = m_machineIni.GetItems;
			var count = stpButtonPanel.Children.Count;
			stpButtonPanel.Children.RemoveRange(1, count - 1);
			
			foreach (var i in items)
			{
				if (i.Category == MachineIniCategorys.Text)
				{
					ListBoxItem li = new ListBoxItem();
					li.Foreground = (l.Items.Count % 2 == 1) ? Brushes.SaddleBrown : Brushes.Plum;
					li.Background = (l.Items.Count % 2 == 1) ? Brushes.PaleGoldenrod : Brushes.Beige;
					li.Content = i.Field + " : " + i.Value;
					l.Items.Add(li);
				}
			}

			return l;
		}
		
		private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			IniSettingsWindow w = new IniSettingsWindow();
	
			if (w.ShowDialog() == false)
			{
				string newValue = w.OptionValue;
				if (newValue != "")
				{
					Console.WriteLine(newValue);
					var listBox = sender as ListBox;
					int a = listBox.SelectedIndex;
					var current = listBox.Items[a] as ListBoxItem;
					var old = Convert.ToString(current.Content);
					var dropDownBox = stpButtonPanel.Children[0] as ComboBox;
					var newContent = old.Split(":".ToCharArray());
					if (dropDownBox.SelectedIndex == 0) // need to validate inputs!
					{
						current.Content = newValue + " : " + newValue;
					}
					else
					{
						current.Content = newContent[0] + " : " + newValue;
					}
					//m_machineIni.GetItems[a]
				}
			}
		}

		private void CommitMachineIni()
		{
		
		}

		private void RemoveChildrenFromStackPanel()
		{
			int childCount = stpButtonPanel.Children.Count;
			if (childCount > 0)
			{
				stpButtonPanel.Children.RemoveRange(0, childCount);
			}
		}

		private void MachineIniCategorys_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void MachineIniCategorys_DropDownClosed(object sender, EventArgs e)
		{
			var items = m_machineIni.GetItems;
			var count = stpButtonPanel.Children.Count;
			stpButtonPanel.Children.RemoveRange(1, count - 1);

			stpButtonPanel.Children.Add(SetupDynamicListBox());
		}

		/// <summary>
		/// Call this from button_click event method
		/// If user presses OK then save changes, if user presses cancel discard and continue.
		/// </summary>
		private void IsSaveRequired()
		{
			if (RequiresSave == true)
			{
				string message = "Changes to machine.ini are pending.\nPress OK to save.\nPress Cancel to discard changes.";
				string caption = "Changes Pending.";
				var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
				if (result == MessageBoxResult.OK)
				{
					// save
				}
				else
				{
					// discard
				}
				RequiresSave = false;
			}
		}

		private void btnVolume_Click(object sender, RoutedEventArgs e)
		{
			MasterVolumeSlider.IsEnabled = true;
			MasterVolumeSlider.Visibility = Visibility.Visible;
			MasterVolumeSlider.Value =  BoLib.getLocalMasterVolume();
			txtVolumeSliderValue.Text = Convert.ToString(MasterVolumeSlider.Value);
		}

		private void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			PlaySoundOnEvent(@"./wav/volume.wav");
			uint volume = Convert.ToUInt32(MasterVolumeSlider.Value);
			BoLib.setLocalMasterVolume(volume);
		}

		private void btnReadMeters_Click(object sender, RoutedEventArgs e)
		{
			RemoveChildrenFromStackPanel();

			var a = Extension.GetChildOfType<StackPanel>(MainGrid);
			string panelName = "BottomPanel";
			var item = a.Find(sp => sp.Name == panelName);
			if (item != null)
			{
				item.Children.RemoveRange(0, item.Children.Count);
				MainGrid.Children.Remove(item);
				a.Remove(item);
				m_gameStatistics.ResetStats();
			}

			m_gameStatistics.ParsePerfLog();

			Label lblBanner = new Label();
			lblBanner.Content = "Machine Info:";
			lblBanner.FontSize = 22;
			stpButtonPanel.Children.Add(lblBanner);
			
			Label lblTotalBet = new Label();
			lblTotalBet.FontSize = 22;
			string totalBetStr = Convert.ToString(m_gameStatistics.TotalBet);
			lblTotalBet.Content = "Total Bet: £" + totalBetStr.Insert(totalBetStr.Length-2, ".");
			stpButtonPanel.Children.Add(lblTotalBet);
			stpButtonPanel.Background = Brushes.AntiqueWhite;
			
			Label lblTotalWon = new Label();
			string totalWonStr = Convert.ToString(m_gameStatistics.TotalWon);
			lblTotalWon.FontSize = 22;
			lblTotalWon.Content = "Total Won: £" + totalWonStr.Insert(totalWonStr.Length-2, ".");
			stpButtonPanel.Children.Add(lblTotalWon);

			Label lblTotalRtp = new Label();
			lblTotalRtp.FontSize = 22;
			double rtp = (double)m_gameStatistics.TotalWon / (double)m_gameStatistics.TotalGames;
			lblTotalRtp.Content = "RTP: " + Math.Round(rtp,2).ToString() + "%";
			stpButtonPanel.Children.Add(lblTotalRtp);

			StackPanel s = new StackPanel();
			s.Name = panelName;
			s.Background = Brushes.Aquamarine;
			s.HorizontalAlignment = HorizontalAlignment.Center;
			s.VerticalAlignment = VerticalAlignment.Bottom;
			
			MainGrid.Children.Add(s);
			Grid.SetColumn(s, 1);
			Grid.SetRow(s, 1);

			Label lblGameSpecific = new Label();
			lblGameSpecific.FontFamily = new FontFamily("Consolas");
			lblGameSpecific.Background = Brushes.LightGray;
			lblGameSpecific.Foreground = Brushes.DarkBlue;
			//lblGameSpecific.FontSize = 11;
			//lblGameSpecific.LayoutTransform = new ScaleTransform(2, 2);
			foreach (var gs in m_gameStatistics.Games)
			{
				lblGameSpecific.Content += "Game No:\t\t " + gs.GameNumber.ToString() + "\r\n";
				lblGameSpecific.Content += "Model:\t\t\t " + gs.ModelNumber.ToString() + "\r\n";
				lblGameSpecific.Content += "Bets:\t\t\t " + gs.Bets.ToString() + "\r\n";
				lblGameSpecific.Content += "Wins:\t\t\t " + gs.Wins.ToString() + "\r\n";
				lblGameSpecific.Content += "Percentage:\t\t " + Math.Round(gs.Percentage).ToString() + "\r\n";
				lblGameSpecific.Content += "-------------------\r\n"; 
			}

			s.Children.Add(lblGameSpecific);
		}

		private void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
			m_keyDoorWorker.TestSuiteRunning = true;
			TestSuiteWindow ts = new TestSuiteWindow();
			ts.ShowDialog();
			m_keyDoorWorker.TestSuiteRunning = false;
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
