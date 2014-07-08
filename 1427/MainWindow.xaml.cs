using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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

		MachineErrorLog m_errorLogText = new MachineErrorLog();
		MachineIni m_machineIni = new MachineIni();
		UniqueIniCategory m_uniqueIniCategory = new UniqueIniCategory();
		MachineGameStatistics m_gameStatistics = new MachineGameStatistics();
		ServiceEnabler m_enabler = new ServiceEnabler();
		ShortTermMeters m_shortTerm = new ShortTermMeters();
		LongTermMeters m_longTerm = new LongTermMeters();
		MachineInfo m_machineData = new MachineInfo();
		GamesList m_gamesList = new GamesList();
		MachineLogsController m_logController = new MachineLogsController();
		UserSoftwareUpdate m_updateFiles = new UserSoftwareUpdate();

		public MainWindow()
        {
            InitializeComponent();
			try
			{
				InitialiseBoLib();
				m_keyDoorThread = new Thread(new ThreadStart(m_keyDoorWorker.Run));
				m_keyDoorThread.Start();
				while (!m_keyDoorThread.IsAlive) ;
				Thread.Sleep(2);
				
				m_machineIni.ParseIni();
			}
			catch (Exception err)
			{
				MessageBox.Show("Error: " + err.ToString());
			}
            RowOne.Height = new GridLength(75);
			ColumnOne.Width = new GridLength(200);
			this.Loaded += new RoutedEventHandler(WindowMain_Loaded);
        }

		#region Properties

		public MachineLogsController LogController { get { return m_logController; } }
		public UserSoftwareUpdate UpdateFiles { get { return m_updateFiles; } }
		
		public GamesList GamesList
		{
			get { return m_gamesList; }
		}

		DoorAndKeyStatus m_keyDoorWorker = new DoorAndKeyStatus();
		public DoorAndKeyStatus KeyDoorWorker
		{
			get { return m_keyDoorWorker; }
			private set { m_keyDoorWorker = value; }
		}
		
		public MachineInfo MachineData
		{
			get { return m_machineData; }
			set { m_machineData = value; }
		}

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

		public ServiceEnabler Enabler
		{
			get { return m_enabler; }
		}

		public MachineGameStatistics GameStatistics
		{
			get { return m_gameStatistics; }
			set { m_gameStatistics = value; }
		}

		public ShortTermMeters ShortTerm
		{
			get { return m_shortTerm; }
			set { m_shortTerm = value; }
		}

		public LongTermMeters LongTerm
		{
			get { return m_longTerm; }
			set { m_longTerm = value; }
		}

		#endregion

		private void btnExit_Click(object sender, RoutedEventArgs e)
        {
			Application.Current.Shutdown();
        }
		
		private void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			BoLib.getCountryCode();
			MessageBox.Show(BoLib.getCountryCodeStr());
			MessageBox.Show(BoLib.getEDCTypeStr());
			//CommitChanges.Save();
			//MessageBox.Show("Changes Made, reboot required.", "Commit Changes", MessageBoxButton.OK, MessageBoxImage.Information);
			//CommitChanges.RebootMachine();
			//HopperUtilsWindow w = new HopperUtilsWindow(m_keyDoorWorker);
			//w.ShowDialog();
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			MyLogfiles.IsEnabled = true;
			MyLogfiles.Visibility = Visibility.Visible;
			LogController.setEerrorLog();
			LogController.setPlayedLog();
			LogController.setWinningLog();
		}

		private void btnHopperOK_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
			}
		}

		private void Games_Click(object sender, RoutedEventArgs e)
		{
			m_gameStatistics.ParsePerfLog();
			m_enabler.GameStatistics = true;
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
				InitialiseBoLib();
				m_keyDoorThread = new Thread(new ThreadStart(m_keyDoorWorker.Run));
				m_keyDoorThread.Start();
				while (!m_keyDoorThread.IsAlive);
				Thread.Sleep(2);

				m_machineIni.ParseIni();
			}
			catch (Exception err)
			{
				MessageBox.Show("Error: " + err.ToString());
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
		
		private void modifySettingsButton_Click(object sender, RoutedEventArgs e)
		{
			m_machineIni.ParseIni();
			MessageBox.Show("Machine RTP " + m_machineIni.GetIniValue("Datapack.Dpercentage") + "%");
			MessageBox.Show("Machine Number " + m_machineIni["Server.Machine Number"]);

			// commit changes to memory
		}

		private void btnSetup_Click(object sender, RoutedEventArgs e)
		{
			TabSetup.IsEnabled = true;
			TabSetup.Visibility = Visibility.Visible;

			m_enabler.Volume = true;
			MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
			if (MasterVolumeSlider.Value > 0)
				txtVolumeSliderValue.Text = Convert.ToString(MasterVolumeSlider.Value);
		}
		
		private bool ValidateNewIniSetting()
		{
			return true;
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
				}
			}
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
			// Select a pre-defined set of regional rules. Select the region in the machine and this loads them into the shell.
		}
		
		private void MachineIniCategorys_DropDownClosed(object sender, EventArgs e)
		{
		}
		
		private void btnVolume_Click(object sender, RoutedEventArgs e)
		{
			m_enabler.Volume = true;
			MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
			if (MasterVolumeSlider.Value > 0)
				txtVolumeSliderValue.Text = Convert.ToString(MasterVolumeSlider.Value);
		}
		
		private void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			//PlaySoundOnEvent(@"./wav/volume.wav");
			uint volume = Convert.ToUInt32(MasterVolumeSlider.Value);
			BoLib.setLocalMasterVolume(volume);
		}

		private void btnReadMeters_Click(object sender, RoutedEventArgs e)
		{
			m_shortTerm.ReadMeter();
			m_longTerm.ReadMeter();
			m_enabler.Meters = true;
		}

		private void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
			m_keyDoorWorker.TestSuiteRunning = true;
			TestSuiteWindow ts = new TestSuiteWindow();
			ts.ShowDialog();
			m_keyDoorWorker.TestSuiteRunning = false;
		}

		private void btnSystem_Click(object sender, RoutedEventArgs e)
		{
			GamesList.GetGamesList();
			m_enabler.System = true;
		}

		private void btnUpdateFiles_Click(object sender, RoutedEventArgs e)
		{
			if (m_updateFiles.DoSoftwareUpdate() == false)
			{
				MessageBox.Show("Could not find update.ini " + UpdateFiles.UpdateIni);
			}
			else
			{
				btnUpdateFiles.IsEnabled = false;
				btnUpdateFiles.Visibility = Visibility.Hidden;
				btnRollback.IsEnabled = false;
				btnRollback.Visibility = Visibility.Hidden;
				

				stpUpdate.IsEnabled = true;
				stpUpdate.Visibility = Visibility.Visible;
			}
		}
    }
}
