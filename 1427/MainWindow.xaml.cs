using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
	/// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		bool m_requiresSave = false;
		bool m_sharedMemoryOnline = false;
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
				Random r = new Random();
				CultureInfo ci = null;
				if (r.Next(2) == 0)
					ci = new CultureInfo("es-ES"); // read this from config
				else
					ci = new CultureInfo("en-GB");
				
				Thread.CurrentThread.CurrentCulture = ci;
				Thread.CurrentThread.CurrentUICulture = ci;

				InitialiseBoLib();
				m_keyDoorThread = new Thread(new ThreadStart(m_keyDoorWorker.Run));
				m_keyDoorThread.Start();
				while (!m_keyDoorThread.IsAlive) ;
				Thread.Sleep(2);
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
			Enabler.ClearAll();
		}
		
		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{
			Enabler.EnableCategory(Categories.Logfile);
			LogController.setErrorLog();
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
			Enabler.EnableCategory(Categories.GameStatistics);
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
			
			if (m_sharedMemoryOnline)
				BoLib.closeSharedMemory();
		}
		
		private void modifySettingsButton_Click(object sender, RoutedEventArgs e)
		{
			/*m_machineIni.ParseIni();
            if (m_machineIni.ChangesPending == false)
            {
                // commit changes to memory
                Thread saver = new Thread(new ThreadStart(DiskCommit.SaveAndReboot));
                MessageBox.Show("Cabinet restarting", "System Notice");
            }*/
		}
        
		private void btnSetup_Click(object sender, RoutedEventArgs e)
		{
			Enabler.EnableCategory(Categories.Setup);
            TabSetup.SelectedIndex = 0;
			MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
			if (MasterVolumeSlider.Value > 0)
				txtVolumeSliderValue.Text = Convert.ToString(MasterVolumeSlider.Value);
			btnUpdateFiles.IsEnabled = true;
			btnRollback.IsEnabled = true;
            
          /* 
           * Save and reboot machine
           * Thread saver = new Thread(new ThreadStart(DiskCommit.SaveAndReboot));
           * saver.Start();
           * AutoClosingMessageBox.Show("Rebooting System in 5 seconds.", "Rebooting", 5000);
           */
		}
		
		private bool ValidateNewIniSetting()
		{   
			return true;
		}
        
        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            var l = sender as ListView;
            int a1 = l.SelectedIndex;
            var c = l.Items[a1] as IniElement;
            var items = l.ItemsSource;
            IniSettingsWindow w = new IniSettingsWindow(c.Value);
	       
			if (w.ShowDialog() == false)
			{
				string newValue = w.OptionValue;
                if (newValue != c.Value)
                {
                    Console.WriteLine(newValue);
                    var listView = sender as ListView;
                    int a = listView.SelectedIndex;
                    var current = listView.Items[a] as IniElement;
                    current.Value = newValue;
                    if (current.Field[0] == '#')
                        current.Value = "";
                    
                    /*items[a] = current;*/
                    /*listView.ItemsSource = items;*/
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
			Enabler.EnableCategory(Categories.Meters);
		}
		
		private void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
			m_enabler.ClearAll();
			m_keyDoorWorker.TestSuiteRunning = true;
			TestSuiteWindow ts = new TestSuiteWindow();
			ts.ShowDialog();
			m_keyDoorWorker.TestSuiteRunning = false;
		}
		
		private void btnSystem_Click(object sender, RoutedEventArgs e)
		{
			GamesList.GetGamesList();
			Enabler.EnableCategory(Categories.System);
		}
		
		private void btnUpdateFiles_Click(object sender, RoutedEventArgs e)
		{
			if (m_updateFiles.DoSoftwareUpdatePreparation() == false)
			{
				MessageBox.Show("Could not find update.ini " + UpdateFiles.UpdateIni);
			}
			else
			{
				btnUpdateFiles.IsEnabled = false;
				btnUpdateFiles.Visibility = Visibility.Hidden;
				btnRollback.IsEnabled = false;
				btnRollback.Visibility = Visibility.Hidden;
				btnCancelUpdate.IsEnabled = true;
				btnCancelUpdate.Visibility = Visibility.Visible;
				btnPerformUpdate.IsEnabled = true;
				btnPerformUpdate.Visibility = Visibility.Visible;
				stpUpdate.IsEnabled = true;
				stpUpdate.Visibility = Visibility.Visible;
				treeUpdateSelectFiles.Visibility = Visibility.Visible;
				treeUpdateSelectFiles.IsEnabled = true;
				lblUpdateSelect.IsEnabled = true;
				lblUpdateSelect.Visibility = Visibility.Visible;
			}
		}
		
		private void UpdateCheckBoxSelected_Checked(object sender, RoutedEventArgs e)
		{
			if (btnPerformUpdate.IsEnabled == false)
				btnPerformUpdate.IsEnabled = true;
		}
		
		private void UpdateCheckBoxSelected_UnChecked(object sender, RoutedEventArgs e)
		{
			if (m_updateFiles.FileCount > 0)
				m_updateFiles.FileCount--;

			if (btnPerformUpdate.IsEnabled == true && m_updateFiles.FileCount == 0)
				btnPerformUpdate.IsEnabled = false;
		}
		
		private void btnPerformUpdate_Click(object sender, RoutedEventArgs e)
		{
			var checkboxes = Extension.GetChildOfType<CheckBox>(treeUpdateSelectFiles);
			var activeCount = 0;
			foreach (var chk in checkboxes)
			{
				if (chk.IsChecked.Value == true)
				{
					activeCount++;
				}
			}
            
			if (activeCount == 0)
			{
				btnPerformUpdate.IsEnabled = false;
			}
            
			m_updateFiles.DoSoftwareUpdate();
		}
        
		private void btnPerformUpdateCancel_Click(object sender, RoutedEventArgs e)
		{
			m_updateFiles.DoCancelUpdate();
			if (treeUpdateSelectFiles.Items.Count > 0)
				treeUpdateSelectFiles.Items.Clear();
		
			treeUpdateSelectFiles.IsEnabled = false;
			treeUpdateSelectFiles.Visibility = Visibility.Hidden;
			btnPerformUpdate.IsEnabled = false;
			btnPerformUpdate.Visibility = Visibility.Hidden;
			btnCancelUpdate.IsEnabled = false;
			btnCancelUpdate.Visibility = Visibility.Hidden;
			lblUpdateSelect.Visibility = Visibility.Hidden;
		    
			btnRollback.IsEnabled = true;
			btnRollback.Visibility = Visibility.Visible;
			btnUpdateFiles.IsEnabled = true;
			btnUpdateFiles.Visibility = Visibility.Visible;
		}
        
        private void btnScreenShots_Click(object sender, RoutedEventArgs e)
        {
            Enabler.ClearAll();
            ScreenshotWindow w = new ScreenshotWindow();
            w.ShowDialog();
        }
	}
}
