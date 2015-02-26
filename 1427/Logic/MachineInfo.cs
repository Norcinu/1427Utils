﻿using System.Collections.ObjectModel;
using System.IO;
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
		public MachineInfo()
		{
			QueryMachine();
		}
		
		public void ProbeUsb()
		{
			
		}

		private string GetMachineIp()
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
			NativeWinApi.Memorystatus ms = new NativeWinApi.Memorystatus();
			NativeWinApi.GlobalMemoryStatus(ref ms);

			var str = new StringBuilder("Total Physical Memory: " + (ms.DwTotalPhys / 1024) / 1024 + " MB");
			str.Append("\tFree Physical Memory: " + (ms.DwAvailPhys / 1024) / 1024 + " MB");
			str.Append("\nTotal Virtual Memory: " + (ms.DwTotalVirtual / 1024) / 1024 + " MB");
			str.Append("\tFree Virtual Memory: " + (ms.DwAvailVirtual / 1024) / 1024 + " MB");

			return str.ToString();
		}
		
		public string GetScreenResolution()
		{
			string errorString = "Screen Not Active/Fitted.\n";

			var str = new StringBuilder("Top Screen:\t "); 
			NativeWinApi.Devmode dm = new NativeWinApi.Devmode();

			var result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display2", 
				(int)NativeWinApi.ModeNum.EnuCurrentSettings, ref dm);
			
			if (result == true)
			{
				str.Append("Resolution: " + dm.dmPelsWidth + "x" + dm.dmPelsHeight + ". ");
				str.Append("BPP: " + dm.dmBitsPerPel + ".\n");
			}
			else
				str.Append(errorString);
			
			str.Append("Bottom Screen:\t "); 
			NativeWinApi.Devmode dm2 = new NativeWinApi.Devmode();
			result = NativeWinApi.EnumDisplaySettings("\\\\.\\Display1", 
				(int)NativeWinApi.ModeNum.EnuCurrentSettings, ref dm2);
			
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

		public string GetCpuId()
		{
			return "CPU-ID: " + BoLib.GetUniquePcbID(0);
		}

		public string GetEdc()
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
			NativeWinApi.Osversioninfo os = new NativeWinApi.Osversioninfo();
			os.DwOsVersionInfoSize = (uint)Marshal.SizeOf(os);
			NativeWinApi.GetVersionEx(ref os);
			return "OS Version:\nWindows XPe - " + os.SzCsdVersion.ToString();
		}

		public string GetLastMd5Check()
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
			Add(new SystemInfo(GetMachineIp()));
			Add(new SystemInfo(GetMachineSerial()));
			Add(new SystemInfo(GetMemoryInfo()));
			Add(new SystemInfo(GetScreenResolution()));
			Add(new SystemInfo(GetCpuId()));
			Add(new SystemInfo("Game Provider: Project Coin"));
			Add(new SystemInfo(GetOsVersion()));
			Add(new SystemInfo(GetLastMd5Check()));
			Add(new SystemInfo(GetUpdateKey()));
			Add(new SystemInfo(GetCountryCode()));
			Add(new SystemInfo(GetEdc()));
						
			// 20 fields platform -> utils.
			// x games is seperate. + maybe include shell, utils + menu here. if so above will be 17 fields.
		}
	}
}
