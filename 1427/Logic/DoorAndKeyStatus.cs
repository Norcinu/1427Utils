using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PDTUtils.Logic;
using PDTUtils.Native;

namespace PDTUtils
{
    /// <summary>
    /// Handles status of door open/closed, key turns and card reader status.
    /// </summary>
	public class DoorAndKeyStatus : INotifyPropertyChanged
	{
		volatile bool _doorStatus;
		volatile bool _running;
		volatile bool _hasChanged;
		volatile bool _isTestSuiteRunning;
        volatile bool _prepareForReboot;
        
        System.Timers.Timer _updateTimer;

		#region Properties
		public bool TestSuiteRunning
		{
			get { return _isTestSuiteRunning; }
			set { _isTestSuiteRunning = value; }
		}

		public bool HasChanged
		{
			get { return _hasChanged; }
			set { _hasChanged = value; }
		}

		public bool DoorStatus
		{
			get { return _doorStatus; }
			set
			{
				_doorStatus = value;
				OnPropertyChanged("DoorStatus");
				OnPropertyChanged("IsDoorClosed");
			}
		}

		public bool IsDoorClosed
		{
			get { return !_doorStatus; }
		}

		public bool Running
		{
			get { return _running; }
			set { _running = value; }
		}

        public bool PrepareForReboot
        {
            get { return _prepareForReboot; }
            set { _prepareForReboot = value; }
        }
		#endregion
        
		public DoorAndKeyStatus()
		{
			_doorStatus = false;
			_running = true;
			_hasChanged = false;
			_isTestSuiteRunning = false;

			_updateTimer = new System.Timers.Timer(1000);
			_updateTimer.Enabled = true;
		}
        
        public void Run()
		{
			while (_running)
			{
				var r = new Random();
			    if (r.Next(1000) < 100 && !_isTestSuiteRunning)
			    {
			        if (BoLib.refillKeyStatus() == 0 && !_prepareForReboot)
			        {
			            _running = false;
			            Application.Current.Dispatcher.Invoke(
			                DispatcherPriority.Normal,
			                (ThreadStart) delegate
			                {
#if !DEBUG
			                    if (GlobalConfig.RebootRequired)
			                        BoLib.setRebootRequired();
#endif
			                });
			            Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
			        }

			        if (BoLib.getDoorStatus() == 0)
			        {
			            if (_doorStatus)
			            {
			                _doorStatus = false;
			                _hasChanged = true;
			                OnPropertyChanged("DoorStatus");
			                OnPropertyChanged("IsDoorClosed");
			            }
			        }
			        else
			        {
			            if (_doorStatus == false)
			            {
			                _doorStatus = true;
			                _hasChanged = true;
			                OnPropertyChanged("DoorStatus");
			                OnPropertyChanged("IsDoorClosed");
			            }
			        }
			    }
			    Thread.Sleep(2);
			}
		}
        
		public void Clone(DoorAndKeyStatus kd)
		{
			DoorStatus = kd.DoorStatus;
			HasChanged = kd.HasChanged;
			Running = kd.Running;
		}

		#region Property Changed events
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}
