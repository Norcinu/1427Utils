using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using PDTUtils.Native;
using PDTUtils.BoLibNative;
using System.Runtime.InteropServices;

namespace PDTUtils.Logic
{
	public class SystemInfo
	{
		public string Field { get; set; }
		public SystemInfo(string name)
		{
			this.Field = name;
		}
	}
	/// <summary>
	/// Class to represent the hardware and operating system settings.
	/// </summary>
	public class MachineInfo : ObservableCollection<SystemInfo>
	{
		//ManagementClass os = new ManagementClass("Win32_OperatingSystem");
		ManagementBaseObject inParams;
		ManagementBaseObject outParams;

		string m_ipAddress;
		string m_subnet;
		bool isUsbDriveConnected; // power this by event/delegate.

		public MachineInfo()
		{
			QueryMachine();
		}

		public void ProbeUsb()
		{
	
		}

		private string GetMachineIP()
		{
			string address = "IP Address: ";
			foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
					ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
					{
						if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							address += ip.Address.ToString();
						}
					}
				}
			}
			return address;
		}

		private string GetComputerName()
		{
			return System.Environment.MachineName;
		}

		public string GetScreenOneResolution()
		{
			return Screen.AllScreens[0].ToString();
		}

		public string GetScreenTwoResolution()
		{
			if (System.Windows.SystemParameters.VirtualScreenHeight != System.Windows.SystemParameters.PrimaryScreenHeight)
				return "1";
			return "";
			//return Screen.AllScreens[1].ToString() + "x" + System.Windows.SystemParameters.pr
		}

		public string GetMemoryInfo()
		{
			/*string memory = "Physical Memory: ";
			memory += "\nFree Physical Memory: ";
			memory += "\nVirtual Memory: ";
			memory += "\nFree Virtual Memory: ";
			return memory;*/
		
			/*NativeMemLayout memory = new NativeMemLayout();
			//BoLib.getMemoryStatus(ref memory);
			memory.dwLength = (uint)Marshal.SizeOf(typeof(NativeMemLayout));
			BoLib.getMemoryStatus(ref memory);
			var totalPhys = ComputerInfo.TotalPhysicalMemory;//((memory.dwTotalPhys / 1024) / 1024).ToString();
			StringBuilder str = new StringBuilder("Physical Memory: " + totalPhys);//(memory.dwTotalPhys/1024)/1024);
			str.Append("\n");
			str.Append("Free Physical Memory: " + memory.dwAvailPhys);
			return str.ToString();*/
			ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_LogicalMemoryConfiguration");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

			double memory = 0;
			foreach (ManagementObject item in searcher.Get())
			{
				memory = double.Parse(item["TotalPhysicalMemory"].ToString());
			}
			return (memory/1024).ToString() + "MB";
		}

		public string GetMachineSerial()
		{
			return BoLib.getSerialNumber();
		}

		public void QueryMachine()
		{
			Add(new SystemInfo(GetComputerName()));
			Add(new SystemInfo(GetMachineIP()));
			Add(new SystemInfo(GetMachineSerial()));
			Add(new SystemInfo(GetMemoryInfo()));
			// 20 fields platform -> utils.
			// x games is seperate. + maybe include shell, utils + menu here. if so above will be 17 fields.

		}

		/*public void ProbeMachine()
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
		}*/
	}
}
