using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PDTUtils.Native;

namespace PDTUtils
{
    /// <summary>
    /// Handles status of door open/closed, key turns and card reader status.
    /// </summary>
	public class DoorAndKeyStatus : INotifyPropertyChanged
	{
		volatile bool m_doorStatus;
		volatile bool m_running;
		volatile bool m_hasChanged;
		volatile bool m_isTestSuiteRunning;
        volatile bool m_prepareForReboot;

        System.Timers.Timer updateTimer;

		#region Properties
		public bool TestSuiteRunning
		{
			get { return m_isTestSuiteRunning; }
			set { m_isTestSuiteRunning = value; }
		}

		public bool HasChanged
		{
			get { return m_hasChanged; }
			set { m_hasChanged = value; }
		}

		public bool DoorStatus
		{
			get { return m_doorStatus; }
			set
			{
				m_doorStatus = value;
				this.OnPropertyChanged("DoorStatus");
				this.OnPropertyChanged("IsDoorClosed");
			}
		}

		public bool IsDoorClosed
		{
			get { return !m_doorStatus; }
		}

		public bool Running
		{
			get { return m_running; }
			set { m_running = value; }
		}

        public bool PrepareForReboot
        {
            get { return m_prepareForReboot; }
            set { m_prepareForReboot = value; }
        }
		#endregion
        
		public DoorAndKeyStatus()
		{
			m_doorStatus = false;
			m_running = true;
			m_hasChanged = false;
			m_isTestSuiteRunning = false;

			updateTimer = new System.Timers.Timer(1000);
			updateTimer.Enabled = true;
		}
        
        public void Run()
		{
			while (m_running)
			{
				Random r = new Random();
				if (r.Next(1000) < 100)
				{
					if (m_isTestSuiteRunning == false)
					{
						if (BoLib.refillKeyStatus() == 0 && !m_prepareForReboot)
						{
                            m_running = false;
                            Application.Current.Dispatcher.Invoke(
                                    DispatcherPriority.Normal, 
                                    (ThreadStart)delegate {
                                        System.Diagnostics.Debug.WriteLine("DISPATCHER INVOKE");
                            });
							Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
						}
                        
                        if (BoLib.getDoorStatus() == 0)
						{
							if (m_doorStatus == true)
							{
								m_doorStatus = false;
								m_hasChanged = true;
								this.OnPropertyChanged("DoorStatus");
								this.OnPropertyChanged("IsDoorClosed");
							}
						}
						else
						{				
							if (m_doorStatus == false)
							{
								m_doorStatus = true;
								m_hasChanged = true;
								this.OnPropertyChanged("DoorStatus");
								this.OnPropertyChanged("IsDoorClosed");
							}
						}
					}
				}
				Thread.Sleep(2);
			}
		}
        
		public void Clone(DoorAndKeyStatus kd)
		{
			this.DoorStatus = kd.DoorStatus;
			this.HasChanged = kd.HasChanged;
			this.Running = kd.Running;
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
