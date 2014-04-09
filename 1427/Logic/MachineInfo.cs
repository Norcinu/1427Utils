using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Class to represent the hardware and operating system settings.
	/// </summary>
	class MachineInfo
	{
		//ManagementClass os = new ManagementClass("Win32_OperatingSystem");
		ManagementBaseObject inParams;
		ManagementBaseObject outParams;

		public MachineInfo()
		{
		}

		public void ProbeMachine()
		{
			SelectQuery netObjQry = new SelectQuery("Win32_NetworkAdapter");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(netObjQry);

			foreach (ManagementObject netAdapters in searcher.Get())
			{
				Object adapterTypeObj = netAdapters.GetPropertyValue("AdapterType");
				string adapterType = "";
				if (adapterTypeObj != null)
				{
					adapterType = adapterTypeObj.ToString();
				}

				Object adapterTypeIDObj = netAdapters.GetPropertyValue("AdapterTypeID");
				string adapterTypeID = "";
				if (adapterTypeIDObj != null)
				{
					adapterTypeID = adapterTypeIDObj.ToString();
				}

				if ((UInt16)netAdapters.GetPropertyValue("AdapterTypeID") == 0)
				{
					MessageBox.Show("Network Adapter Name: " + netAdapters.GetPropertyValue("Name").ToString());
					MessageBox.Show("Adapter Type: " + adapterTypeObj);
					MessageBox.Show("Adapter Type ID:" + adapterTypeID + "\n");
				}
			}
		}
	}
}
