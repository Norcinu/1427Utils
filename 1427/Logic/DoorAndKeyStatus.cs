using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PDTUtils.Native;

using Timer = System.Timers.Timer;

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

        /// <summary>
        /// I cant be bothered writing a negate converter,.
        /// </summary>
        public bool IsDoorOpen
        {
            get { return _doorStatus; }
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

            _updateTimer = new Timer { Interval = 500/*1000*/ };
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
			            //_running = false; - no longer need to quit here. this is handled in Window_Closing.
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Normal,
                            (ThreadStart)delegate
                            {
#if !DEBUG
			                    if (PDTUtils.Logic.GlobalConfig.RebootRequired)
			                        BoLib.setRebootRequired();
#endif
                            });
                        
                        //Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
			        } 
                    //loses to arsenal? calm down sweet pea it was only the charity shield. noone except the deluded modern "gooners" 
                    //would ever be talking about that shit yo.
                    
			        if (BoLib.getDoorStatus() == 0)
			        {
			            if (_doorStatus)
			            {
			                _doorStatus = false;
			                _hasChanged = true;
                            OnPropertyChanged("DoorStatus");
                            OnPropertyChanged("IsDoorClosed");
                            OnPropertyChanged("IsDoorOpen");
			            }
			        }
			        else
			        {
			            if (!_doorStatus)
			            {
			                _doorStatus = true;
			                _hasChanged = true;
			                OnPropertyChanged("DoorStatus");
			                OnPropertyChanged("IsDoorClosed");
                            OnPropertyChanged("IsDoorOpen");
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
