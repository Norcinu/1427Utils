using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public double WindowHeight { get; set; }
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
        TitoMeters m_titoMeter = new TitoMeters();
        MachineInfo m_machineData = new MachineInfo();
		GamesList m_gamesList = new GamesList();
		MachineLogsController m_logController = new MachineLogsController();
        UserSoftwareUpdate m_updateFiles = null;
        
        public MainWindow()
        {
            FullyLoaded = false;
            try
            {
                InitialiseBoLib();
                InitializeComponent();
                CultureInfo ci = null;

                if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                    ci = new CultureInfo("es-ES");
                else
                    ci = new CultureInfo("en-GB");
                
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                
                m_updateFiles = new UserSoftwareUpdate(this);
                WindowHeight = this.Height;
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err.ToString());
                Application.Current.Shutdown();
            }
            
            RowOne.Height = new GridLength(75);
            ColumnOne.Width = new GridLength(200);
            Loaded += new RoutedEventHandler(WindowMain_Loaded);
            FullyLoaded = true;
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

        public TitoMeters TitoMeter
        {
            get { return m_titoMeter; }
            set { m_titoMeter = value; }
        }
        
        public bool FullyLoaded { get; set; }

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
            if (!LogController.IsLoaded)
            {
            //    Thread t = new Thread(() =>
            //    {
                    LogController.setErrorLog();
                    LogController.setWarningLog();
                    LogController.setPlayedLog();
                    LogController.setWinningLog();
                    LogController.setHandPayLog();
                    LogController.IsLoaded = true;
              //  });
              //  t.Start();
            }
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
                try
                {
                    if (m_keyDoorThread.IsAlive)
                    {
                        //m_keyDoorThread.Abort();
                        m_keyDoorThread.Join();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            

#if DEBUG
            if (GlobalConfig.RebootRequired)
                Debug.WriteLine("WE SHOULD BE RE-BOOTING.");
#else
            if (GlobalConfig.RebootRequired)
                BoLib.setRebootRequired();
#endif       
            if (m_sharedMemoryOnline)
            {
                m_sharedMemoryOnline = false;
                BoLib.closeSharedMemory();
            }
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
            ucMainPage.IsEnabled = false;
            ucMainPage.Visibility = Visibility.Hidden;

            ucDiagnostics.IsEnabled = false;
            ucDiagnostics.Visibility = Visibility.Hidden;

            ucPerformance.IsEnabled = false;
            ucPerformance.Visibility = Visibility.Hidden;

            Enabler.ClearAll();
			Enabler.EnableCategory(Categories.Setup);
            
            TabSetup.SelectedIndex = 0;
			MasterVolumeSlider.Value = BoLib.getLocalMasterVolume();
		}
		
		private bool ValidateNewIniSetting()
		{   
			return true;
		}
        
        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            UpdateIniItem(sender);
		}
        
        private void UpdateIniItem(object sender)
        {
            var l = sender as ListView;
            if (l.SelectedIndex == -1)
                return;
            
            var c = l.Items[l.SelectedIndex] as IniElement;
            var items = l.ItemsSource;
            
            IniSettingsWindow w = new IniSettingsWindow(c.Field, c.Value);
            if (w.ShowDialog() == false)
            {
                switch (w.RetChangeType)
                {
                    case ChangeType.AMEND:
                        AmendOption(w, sender, ref c);
                        break;
                    case ChangeType.CANCEL:
                        break;
                    case ChangeType.COMMENT:
                        CommentEntry(w, sender, ref c);
                        break;
                    case ChangeType.UNCOMMENT:
                        UnCommentEntry(w, sender, ref c);
                        break;
                    default:
                        break;
                }
                l.SelectedIndex = -1;
            }
        }
        
        void AmendOption(IniSettingsWindow w, object sender, ref IniElement c)
        {
            string newValue = w.OptionValue;
            Debug.WriteLine(newValue);
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;
            
            if (newValue != c.Value || (newValue == c.Value && current.Field[0] == '#'))
            {
                current.Value = newValue;
                if (current.Field[0] == '#')
                {
                    IniFile ini = new IniFile(Properties.Resources.machine_ini);
                    ini.DeleteKey(current.Category, current.Field);
                    current.Field = current.Field.Substring(1);
                }
                current.Value = newValue;
                listView.Items.Refresh();
                
                GetMachineIni.WriteMachineIni(current.Category, current.Field);
                GetMachineIni.ChangesPending = true;
            }
        }
        
        void CommentEntry(IniSettingsWindow w, object sender, ref IniElement c)
        {
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;
            
            if (current.Field[0] != '#')
            {
                IniFile ini = new IniFile(Properties.Resources.machine_ini);
                ini.DeleteKey(current.Category, current.Field);
                current.Field = "#" + current.Field;

                listView.Items.Refresh();
                GetMachineIni.WriteMachineIni(current.Category, current.Field);
                GetMachineIni.ChangesPending = true;
            }
        }

        private void UnCommentEntry(IniSettingsWindow w, object sender, ref IniElement c)
        {
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as IniElement;
            
            if (current.Field[0] == '#')
            {
                listView.Items.Refresh();
                GetMachineIni.WriteMachineIni(current.Category, current.Field);
                GetMachineIni.ChangesPending = true;
                
                current.Field = current.Field.Substring(1);

                listView.Items.Refresh();
                GetMachineIni.WriteMachineIni(current.Category, current.Field);
                GetMachineIni.ChangesPending = true;
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
            Enabler.ClearAll();
            ucMainPage.IsEnabled = false;
            ucMainPage.Visibility = Visibility.Hidden;

            ucDiagnostics.IsEnabled = false;
            ucDiagnostics.Visibility = Visibility.Hidden;

            ucPerformance.IsEnabled = !ucPerformance.IsEnabled;
            if (ucPerformance.IsEnabled)
                ucPerformance.Visibility = Visibility.Visible;
            else
                ucPerformance.Visibility = Visibility.Hidden;
		}
		
		private void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
            if (BoLib.getDoorStatus() == 1)
            {
                m_enabler.ClearAll();

                ucMainPage.IsEnabled = false;
                ucMainPage.Visibility = Visibility.Hidden;

                ucDiagnostics.IsEnabled = false;
                ucDiagnostics.Visibility = Visibility.Hidden;

                ucPerformance.IsEnabled = false;
                ucPerformance.Visibility = Visibility.Hidden;

                m_keyDoorWorker.TestSuiteRunning = true;
                TestSuiteWindow ts = new TestSuiteWindow();
                ts.ShowDialog();
                m_keyDoorWorker.TestSuiteRunning = false;
            }
            else
            {
                MessageBox.Show("Please Open Door.", "INFO", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
		}
		
		private void btnSystem_Click(object sender, RoutedEventArgs e)
		{
            GamesList.GetGamesList();
            
            Enabler.ClearAll();
            ucMainPage.IsEnabled = false;
            ucMainPage.Visibility = Visibility.Hidden;

            ucDiagnostics.IsEnabled = false;
            ucDiagnostics.Visibility = Visibility.Hidden;

            Enabler.EnableCategory(Categories.System);
		}
        
		private void btnUpdateFiles_Click(object sender, RoutedEventArgs e)
		{
		//	if (m_updateFiles.DoSoftwareUpdatePreparation() == false)
		//	{
		//		MessageBox.Show("Could not find update.ini " + UpdateFiles.UpdateIni);
		//	}
		//	else
		//	{   
                /*btnUpdateFiles.IsEnabled = false;
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
				lblUpdateSelect.Visibility = Visibility.Visible;*/
		//	}
		}
		
		private void UpdateCheckBoxSelected_Checked(object sender, RoutedEventArgs e)
		{
		//	if (btnPerformUpdate.IsEnabled == false)
		//		btnPerformUpdate.IsEnabled = true;
		}
		
		private void UpdateCheckBoxSelected_UnChecked(object sender, RoutedEventArgs e)
		{
			if (m_updateFiles.FileCount > 0)
				m_updateFiles.FileCount--;
            
			//if (btnPerformUpdate.IsEnabled == true && m_updateFiles.FileCount == 0)
			//	btnPerformUpdate.IsEnabled = false;
		}
		
		private void btnPerformUpdate_Click(object sender, RoutedEventArgs e)
		{
			/*var checkboxes = Extension.GetChildOfType<CheckBox>(treeUpdateSelectFiles);
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
            
			m_updateFiles.DoSoftwareUpdate();*/
		}
        
		private void btnPerformUpdateCancel_Click(object sender, RoutedEventArgs e)
        {
		/*	m_updateFiles.DoCancelUpdate();
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
			btnUpdateFiles.Visibility = Visibility.Visible;*/
		}
        
        ICommand EnableScreenshot
        {
            get
            {
                return new DelegateCommand(o => DoScreenshot());
            }
        }

        void DoScreenshot() { }

        private void btnScreenShots_Click(object sender, RoutedEventArgs e)
        {
            Enabler.ClearAll();

            ucMainPage.IsEnabled = false;
            ucMainPage.Visibility = Visibility.Hidden;

            ucDiagnostics.IsEnabled = false;
            ucDiagnostics.Visibility = Visibility.Hidden;

            ucPerformance.IsEnabled = false;
            ucPerformance.Visibility = Visibility.Hidden;

            ScreenshotWindow w = new ScreenshotWindow();
            w.ShowDialog();
        }
        
        private void btnCreditManage_Click(object sender, RoutedEventArgs e)
        {
            ucMainPage.IsEnabled = !ucMainPage.IsEnabled;
            if (ucMainPage.IsEnabled)
                ucMainPage.Visibility = Visibility.Visible;
            else
                ucMainPage.Visibility = Visibility.Hidden;
            
            Enabler.ClearAll();

            ucDiagnostics.IsEnabled = false;
            ucDiagnostics.Visibility = Visibility.Hidden;

            ucPerformance.IsEnabled = false;
            ucPerformance.Visibility = Visibility.Hidden;
        }

        private void btnDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            ucDiagnostics.IsEnabled = !ucDiagnostics.IsEnabled;
            if (ucDiagnostics.IsEnabled)
                ucDiagnostics.Visibility = Visibility.Visible;
            else
                ucDiagnostics.Visibility = Visibility.Hidden;
            
            Enabler.ClearAll();

            ucMainPage.IsEnabled = false;
            ucMainPage.Visibility = Visibility.Hidden;

            ucPerformance.IsEnabled = false;
            ucPerformance.Visibility = Visibility.Hidden;
        }
        
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
        }
        
        // refactor this
        // set globalconfig.reboot = true
        private void btnReboot_Click(object sender, RoutedEventArgs e)
        {
            m_keyDoorWorker.PrepareForReboot = true;
            if (BoLib.refillKeyStatus() == 1 && BoLib.getDoorStatus() == 1)
            {
                MessageBox.Show("Please Turn Refill Key and Close Door", "INFO");
            }
            else if (BoLib.refillKeyStatus() == 1)
            {
                MessageBox.Show("Please Turn Refill Key.", "INFO");
            }
            else if (BoLib.getDoorStatus() == 1)
            {
                MessageBox.Show("Please Close the Door", "INFO");
            }
            
            if (BoLib.refillKeyStatus() == 0 && BoLib.getDoorStatus() == 0)
            {
                m_keyDoorWorker.PrepareForReboot = false;
                DiskCommit.SaveAndReboot(); 
            }
        }
	}
}
