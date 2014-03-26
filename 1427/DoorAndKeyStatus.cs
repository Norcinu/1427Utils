using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using PDTUtils.Native;

namespace PDTUtils
{
	class DoorAndKeyStatus
	{
		void Run()
		{
			while (true)
			{
				if (BoLibNative.Bo_RefillKeyStatus() == 0)
				{
					Application.Current.Shutdown();
				}

				if (BoLibNative.Bo_GetDoorStatus() == 0)
				{
					
				}
				else
				{
										
				}
			}
		}
	}
}
