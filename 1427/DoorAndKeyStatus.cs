using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

using PDTUtils.Native;

namespace PDTUtils
{
	public class DoorAndKeyStatus
	{
		volatile bool doorStatus;
		volatile bool running;

		public bool DoorStatus
		{
			get { return doorStatus; }
			set { doorStatus = value; }
		}

		public bool Running
		{
			get { return running; }
			set { running = value; }
		}
		
		public DoorAndKeyStatus()
		{
			doorStatus = false;
			running = true;
		}

		public void Run()
		{
			while (running)
			{
				Random r = new Random();
				if (r.Next(1000) < 100) // could have a timer -> test every second or so.
				{
					if (BoLib.Bo_RefillKeyStatus() == 0)
					{
						running = false;
						Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
					}

					if (BoLib.Bo_GetDoorStatus() == 0)
					{
						if (doorStatus == true)
							doorStatus = false; // delegate to set this in the other thread? or just read straight from?
					}
					else
					{
						if (doorStatus == false)
							doorStatus = true;
					}
				}
				
				Thread.Sleep(2);
			}
		}
	}
}
