using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PDTUtils.Access;
using PDTUtils.Native;

using Timer = System.Timers.Timer;

namespace PDTUtils
{
    /// <summary>
    /// Handles status of door open/closed, key turns and card reader status.
    /// </summary>
	public class DoorAndKeyStatus : INotifyPropertyChanged
	{
        int _currentValue = -1;
        string[] _strings = new string[8] {"Player", "Technician", "C - Cashier", "Admin", "Operator", 
                                           "D - Distributor", "E - Manufacturer", "None"};
        
		volatile bool _doorStatus;
		volatile bool _running;
		volatile bool _hasChanged;
		volatile bool _isTestSuiteRunning;
        volatile bool _prepareForReboot;
        
        string _smartCardString = "";
        string _commandProperty = "";
        
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
        
        public string SmartCardString
        {
            get { return _smartCardString;}
        }

        public string CommandProperty { get { return _commandProperty; } }

        public bool CanViewManufacturer { get; set; }
        public bool CanViewDistributor { get; set; }
        public bool CanViewCashier { get; set; }
        public bool CanViewDistOrManu { get; set; }
        public bool AnyAuthedCard { get; set; }
        
        #endregion
        
        
		public DoorAndKeyStatus()
		{
			_doorStatus = false;
			_running = true;
			_hasChanged = false;
			_isTestSuiteRunning = false;

            CanViewCashier = false;
            CanViewDistributor = false;
            CanViewManufacturer = false;
            CanViewDistOrManu = false;
            AnyAuthedCard = false;
		}
        
        public void Run()
		{
			while (_running)
			{
				var r = new Random();
                if (r.Next(1000) < 100 && !_isTestSuiteRunning)
                {
                    if (BoLib.getUtilRefillAccess() && !_prepareForReboot)
                    {
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

                    if (!BoLib.getUtilDoorAccess()) //if door is open
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
                    else // door closed.
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
                
                GlobalAccess.Level = BoLib.getUtilsAccessLevel() & 0x0F;
                _smartCardString = _strings[GlobalAccess.Level];
                GlobalAccess.Level = GlobalAccess.Level;
                //property old + new. just for updating the xaml.
                //converter still uses global access.
                //if level != globalaccess.level
                //raise.
                if (GlobalAccess.Level == 2)
                {
                    CanViewManufacturer = false;
                    CanViewDistributor = false;
                    CanViewCashier = true;
                    _commandProperty = "on|on|on";
                }
                else if (GlobalAccess.Level == 5)
                {
                    CanViewManufacturer = false;
                    CanViewDistributor = true;
                    CanViewCashier = false;
                    _commandProperty = "off|on|on";
                }
                else if (GlobalAccess.Level == 6)
                {
                    CanViewManufacturer = true;
                    CanViewDistributor = false;
                    CanViewCashier = false;
                    _commandProperty = "off|off|on";
                }
                else
                {
                    CanViewManufacturer = false;
                    CanViewDistributor = false;
                    CanViewCashier = false;
                }
                
                if (CanViewDistributor || CanViewManufacturer)
                    CanViewDistOrManu = true;
                else
                    CanViewDistOrManu = false;

                if (CanViewDistOrManu || CanViewCashier)
                    AnyAuthedCard = true;
                else
                    AnyAuthedCard = false;
                
                OnPropertyChanged("CanViewManufacturer");
                OnPropertyChanged("CanViewDistributor");
                OnPropertyChanged("CanViewCashier");
                OnPropertyChanged("CommandProperty");
                OnPropertyChanged("CanViewDistOrManu");
                OnPropertyChanged("AnyAuthedCard");
			    
                Thread.Sleep(150);
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
