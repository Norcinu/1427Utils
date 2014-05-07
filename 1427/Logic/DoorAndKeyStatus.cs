using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PDTUtils.Native;

namespace PDTUtils
{
	public class DoorAndKeyStatus
	{
		volatile bool m_doorStatus;
		volatile bool m_running;
		volatile bool m_hasChanged;
		System.Timers.Timer updateTimer;

		#region Properties
		public bool HasChanged
		{
			get { return m_hasChanged; }
			set { m_hasChanged = value; }
		}

		public bool DoorStatus
		{
			get { return m_doorStatus; }
			set { m_doorStatus = value; }
		}

		public bool Running
		{
			get { return m_running; }
			set { m_running = value; }
		}
		#endregion

		public DoorAndKeyStatus()
		{
			m_doorStatus = false;
			m_running = true;
			m_hasChanged = false;

			updateTimer = new System.Timers.Timer(1000);
			updateTimer.Elapsed += CheckForUpdate;
			updateTimer.Enabled = true;
		}

		public void Run()
		{
			while (m_running)
			{
				Random r = new Random();
				if (r.Next(1000) < 100)
				{
					/*if (BoLib.refillKeyStatus() == 0)
					{
						m_running = false;
						Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
					}

					if (BoLib.getDoorStatus() == 0)
					{
						if (m_doorStatus == true)
						{
							m_doorStatus = false;
							m_hasChanged = true;
						}
					}
					else
					{
						if (m_doorStatus == false)
						{
							m_doorStatus = true;
							m_hasChanged = true;
						}
					}*/
				}
				
				Thread.Sleep(2);
			}
		}

		private delegate void TimerUpdate();
		private void CheckForUpdate(object sender, EventArgs e)
		{

		}
	}
}
