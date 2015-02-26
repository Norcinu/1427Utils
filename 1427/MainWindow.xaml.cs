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
		bool _requiresSave = false;
        bool _sharedMemoryOnline = false;
        string _errorMessage = "";
        public double WindowHeight { get; set; }
		System.Timers.Timer _doorStatusTimer;
		System.Timers.Timer _uiUpdateTimer;
		Thread _keyDoorThread;
	    
		MachineErrorLog _errorLogText = new MachineErrorLog();
		MachineIni _machineIni = new MachineIni();
		UniqueIniCategory _uniqueIniCategory = new UniqueIniCategory();
		MachineGameStatistics _gameStatistics = new MachineGameStatistics();
		ServiceEnabler _enabler = new ServiceEnabler();
		ShortTermMeters _shortTerm = new ShortTermMeters();
		LongTermMeters _longTerm = new LongTermMeters();
        TitoMeters _titoMeter = new TitoMeters();
        MachineInfo _machineData = new MachineInfo();
		GamesList _gamesList = new GamesList();
		MachineLogsController _logController = new MachineLogsController();
        UserSoftwareUpdate _updateFiles = null;
        
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
                
                _updateFiles = new UserSoftwareUpdate(this);
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
		
		public MachineLogsController LogController { get { return _logController; } }
		public UserSoftwareUpdate UpdateFiles { get { return _updateFiles; } }
		
		public GamesList GamesList
		{
			get { return _gamesList; }
		}
		
		DoorAndKeyStatus _keyDoorWorker = new DoorAndKeyStatus();
		public DoorAndKeyStatus KeyDoorWorker
		{
			get { return _keyDoorWorker; }
			private set { _keyDoorWorker = value; }
		}
		
		public MachineInfo MachineData
		{
			get { return _machineData; }
			set { _machineData = value; }
		}

		public bool RequiresSave
		{
			get { return _requiresSave; }
			set { _requiresSave = value; }
		}

		public MachineIni GetMachineIni
		{ 
			get { return _machineIni; } 
		}

		public UniqueIniCategory GetUniqueCategories
		{
			get { return _uniqueIniCategory; }
		}

		public ServiceEnabler Enabler
		{
			get { return _enabler; }
		}

		public MachineGameStatistics GameStatistics
		{
			get { return _gameStatistics; }
			set { _gameStatistics = value; }
		}

		public ShortTermMeters ShortTerm
		{
			get { return _shortTerm; }
			set { _shortTerm = value; }
		}

		public LongTermMeters LongTerm
		{
			get { return _longTerm; }
			set { _longTerm = value; }
		}

        public TitoMeters TitoMeter
        {
            get { return _titoMeter; }
            set { _titoMeter = value; }
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
                    LogController.SetErrorLog();
                    LogController.SetWarningLog();
                    LogController.SetPlayedLog();
                    LogController.SetWinningLog();
                    LogController.SetHandPayLog();
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
			_gameStatistics.ParsePerfLog();
			Enabler.EnableCategory(Categories.GameStatistics);
		}
        
		private void GetSystemUptime()
		{
			var ticks = Stopwatch.GetTimestamp();
			var uptime = ((double)ticks) / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);
			var u = uptimeSpan.ToString().Split(".".ToCharArray());
			LblUptime.Content = u[0];
		}
		
		private void WindowMain_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				_keyDoorThread = new Thread(new ThreadStart(_keyDoorWorker.Run));
				_keyDoorThread.Start();
				while (!_keyDoorThread.IsAlive);
				Thread.Sleep(2);
			}
			catch (Exception err)
			{
				MessageBox.Show("Error: " + err.ToString());
			}
		}

        
        private void WindowMain_Closing(object sender, CancelEventArgs e)
        {
            if (_keyDoorWorker.Running == true)
                _keyDoorWorker.Running = false;
                
            if (_keyDoorThread != null)
            {
                try
                {
                    if (_keyDoorThread.IsAlive)
                    {
                        //_keyDoorThread.Abort();
                        _keyDoorThread.Join();
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
            if (_sharedMemoryOnline)
            {
                _sharedMemoryOnline = false;
                BoLib.closeSharedMemory();
            }
        }
        
		private void modifySettingsButton_Click(object sender, RoutedEventArgs e)
		{
			/*_machineIni.ParseIni();
            if (_machineIni.ChangesPending == false)
            {
                // commit changes to memory
                Thread saver = new Thread(new ThreadStart(DiskCommit.SaveAndReboot));
                MessageBox.Show("Cabinet restarting", "System Notice");
            }*/
		}

		private void btnSetup_Click(object sender, RoutedEventArgs e)
		{
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

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
                    case ChangeType.Amend:
                        AmendOption(w, sender, ref c);
                        break;
                    case ChangeType.Cancel:
                        break;
                    case ChangeType.Comment:
                        CommentEntry(w, sender, ref c);
                        break;
                    case ChangeType.Uncomment:
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
			int childCount = StpButtonPanel.Children.Count;
			if (childCount > 0)
			{
				StpButtonPanel.Children.RemoveRange(0, childCount);
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
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = !UcPerformance.IsEnabled;
            if (UcPerformance.IsEnabled)
                UcPerformance.Visibility = Visibility.Visible;
            else
                UcPerformance.Visibility = Visibility.Hidden;
		}
		
		private void btnFunctionalTests_Click(object sender, RoutedEventArgs e)
		{
            if (BoLib.getDoorStatus() == 1)
            {
                _enabler.ClearAll();

                UcMainPage.IsEnabled = false;
                UcMainPage.Visibility = Visibility.Hidden;

                UcDiagnostics.IsEnabled = false;
                UcDiagnostics.Visibility = Visibility.Hidden;

                UcPerformance.IsEnabled = false;
                UcPerformance.Visibility = Visibility.Hidden;

                _keyDoorWorker.TestSuiteRunning = true;
                TestSuiteWindow ts = new TestSuiteWindow();
                ts.ShowDialog();
                _keyDoorWorker.TestSuiteRunning = false;
            }
            else
            {
                MessageBox.Show("Please Open Door.", "INFO", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
		}
		
		private void btnSyste_Click(object sender, RoutedEventArgs e)
		{
            GamesList.GetGamesList();
            
            Enabler.ClearAll();
            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            Enabler.EnableCategory(Categories.System);
		}
        
		private void btnUpdateFiles_Click(object sender, RoutedEventArgs e)
		{
		//	if (_updateFiles.DoSoftwareUpdatePreparation() == false)
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
			if (_updateFiles.FileCount > 0)
				_updateFiles.FileCount--;
            
			//if (btnPerformUpdate.IsEnabled == true && _updateFiles.FileCount == 0)
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
            
			_updateFiles.DoSoftwareUpdate();*/
		}
        
		private void btnPerformUpdateCancel_Click(object sender, RoutedEventArgs e)
        {
		/*	_updateFiles.DoCancelUpdate();
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

            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;

            ScreenshotWindow w = new ScreenshotWindow();
            w.ShowDialog();
        }
        
        private void btnCreditManage_Click(object sender, RoutedEventArgs e)
        {
            UcMainPage.IsEnabled = !UcMainPage.IsEnabled;
            if (UcMainPage.IsEnabled)
                UcMainPage.Visibility = Visibility.Visible;
            else
                UcMainPage.Visibility = Visibility.Hidden;
            
            Enabler.ClearAll();

            UcDiagnostics.IsEnabled = false;
            UcDiagnostics.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;
        }

        private void btnDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            UcDiagnostics.IsEnabled = !UcDiagnostics.IsEnabled;
            if (UcDiagnostics.IsEnabled)
                UcDiagnostics.Visibility = Visibility.Visible;
            else
                UcDiagnostics.Visibility = Visibility.Hidden;
            
            Enabler.ClearAll();

            UcMainPage.IsEnabled = false;
            UcMainPage.Visibility = Visibility.Hidden;

            UcPerformance.IsEnabled = false;
            UcPerformance.Visibility = Visibility.Hidden;
        }
        
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
        }
        
        // refactor this
        // set globalconfig.reboot = true
        private void btnReboot_Click(object sender, RoutedEventArgs e)
        {
            _keyDoorWorker.PrepareForReboot = true;
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
                _keyDoorWorker.PrepareForReboot = false;
                DiskCommit.SaveAndReboot(); 
            }
        }
	}
}
