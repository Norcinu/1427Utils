using System.Collections.ObjectModel;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils.Logic
{
	public class SystemInfo
	{
		public string Field { get; set; }
		public bool IsEditable { get; set; }
		
		public SystemInfo(string name)
		{
			this.Field = name;
			this.IsEditable = false;
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
			return "Computer Name: " + System.Environment.MachineName;
		}

		public string GetMemoryInfo()
		{
			NativeWinApi.MEMORYSTATUS ms = new NativeWinApi.MEMORYSTATUS();
			NativeWinApi.GlobalMemoryStatus(ref ms);

			var str = new StringBuilder("Total Physical Memory: " + (ms.dwTotalPhys / 1024) / 1024 + " MB");
			str.Append("\tFree Physical Memory: " + (ms.dwAvailPhys / 1024) / 1024 + " MB");
			str.Append("\nTotal Virtual Memory: " + (ms.dwTotalVirtual / 1024) / 1024 + " MB");
			str.Append("\tFree Virtual Memory: " + (ms.dwAvailVirtual / 1024) / 1024 + " MB");

			return str.ToString();
		}

		public string GetScreenResolution()
		{
			string errorString = "Screen Not Active/Fitted.\n";

			var str = new StringBuilder("Top Screen:\t "); // \n
			NativeWinApi.DEVMODE dm = new NativeWinApi.DEVMODE();

			var result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display2", 
				(int)NativeWinApi.ModeNum.ENUM_CURRENT_SETTINGS, ref dm);
			
			if (result == true)
			{
				str.Append("Resolution: " + dm.dmPelsWidth + "x" + dm.dmPelsHeight + ". "); //\n
				str.Append("BPP: " + dm.dmBitsPerPel + ".\n");
			}
			else
				str.Append(errorString);
			
			str.Append("Bottom Screen:\t "); // \n
			NativeWinApi.DEVMODE dm2 = new NativeWinApi.DEVMODE();
			result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display1", 
				(int)NativeWinApi.ModeNum.ENUM_CURRENT_SETTINGS, ref dm2);
			
			if (result == true)
			{
				str.Append("Resolution: " + dm2.dmPelsWidth + "x" + dm2.dmPelsHeight + ". ");//\n
				str.Append("BPP: " + dm2.dmBitsPerPel + ".\n");
			}
			else
				str.Append(errorString);
			
			return str.ToString();
		}

		public string GetMachineSerial()
		{
			return BoLib.getSerialNumber();
		}

		public string GetCpuID()
		{
			return "CPU-ID: " + BoLib.GetUniquePcbID(0);
		}

		public string GetEDC()
		{
			return BoLib.getEDCTypeStr();
		}

		public string GetCountryCode()
		{
			try
			{
				return BoLib.getCountryCodeStr();
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		public string GetOsVersion()
		{
			NativeWinApi.OSVERSIONINFO os = new NativeWinApi.OSVERSIONINFO();
			os.dwOSVersionInfoSize = (uint)Marshal.SizeOf(os);
			NativeWinApi.GetVersionEx(ref os);
			return "OS Version:\nWindows XPe - " + os.szCSDVersion.ToString();
		}

		public string GetLastMD5Check()
		{
			return ReadFileLine(Resources.security_log);
		}

		public string GetUpdateKey()
		{
			var strs = ReadFileLine(Resources.update_log,1).Split("=".ToCharArray());
			var final = new StringBuilder(strs[1]);
			
			for (int i = 0; i < 68; i++)
			{
				if ((i % 15 == 0) && (i > 0))
					final.Insert(i, "-");
			}

			return "Update Key: " + final.ToString();
		}

		private string ReadFileLine(string filename, int index = 0)
		{
			string line = "";

			try
			{
				using (StreamReader stream = new StreamReader(filename))
				{
					for (int i = 0; i < index; i++)
						stream.ReadLine();
					line = stream.ReadLine();
				}
			}
			catch (System.Exception ex)
			{
				line = ex.Message;
			}

			return line;
		}
		
		public void QueryMachine()
		{
			Add(new SystemInfo(GetComputerName()));
			Add(new SystemInfo(GetMachineIP()));
			Add(new SystemInfo(GetMachineSerial()));
			Add(new SystemInfo(GetMemoryInfo()));
			Add(new SystemInfo(GetScreenResolution()));
			Add(new SystemInfo(GetCpuID()));
			Add(new SystemInfo("Game Provider: Project Coin"));
			Add(new SystemInfo(GetOsVersion()));
			Add(new SystemInfo(GetLastMD5Check()));
			Add(new SystemInfo(GetUpdateKey()));
			Add(new SystemInfo(GetCountryCode()));
			Add(new SystemInfo(GetEDC()));
						
			// 20 fields platform -> utils.
			// x games is seperate. + maybe include shell, utils + menu here. if so above will be 17 fields.
		}
	}
}
