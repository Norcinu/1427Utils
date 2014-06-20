using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.IO;
using PDTUtils.Native;
using System.Runtime.InteropServices;

namespace PDTUtils.Logic
{
	public class UserSoftwareUpdate
	{
		string m_rollbackIni; 
		string m_updateIni;
		public string UpdateIni
		{
			get { return m_updateIni; }
			set { m_updateIni = value; }
		}
		DriveInfo m_updateDrive;

		public UserSoftwareUpdate()
		{

		}

		public void DoRollBack()
		{

		}

		public bool DoSoftwareUpdate()
		{
			if (CanChangeToUsbDrive())
			{
				// we can look for update.ini
				if (File.Exists(m_updateIni))
				{
					string[] folders_section = null;
					string[] files_section = null;

					bool? b = GetIniProfileSection(out folders_section, "Folders");
					if (b == false || folders_section == null)
						return false;

					b = GetIniProfileSection(out files_section, "Files");
					if (b == false || folders_section == null)
						return false;

					
				}
				else
				{
					return false;
				}
				return true;
			}
			return false;
		}

		private bool GetIniProfileSection(out string[] section, string field)
		{
			uint bufferSize = 4048;
			IntPtr retStringPtr = Marshal.AllocCoTaskMem((int)bufferSize * sizeof(char));
			uint bytesReturned = NativeWinApi.GetPrivateProfileSection(field, retStringPtr, bufferSize, m_updateIni);
			if ((bytesReturned == bufferSize - 2) || (bytesReturned == 0))
			{
				section = null;
				Marshal.FreeCoTaskMem(retStringPtr);
				return false;
			}

			string retString = Marshal.PtrToStringAuto(retStringPtr, (int)bytesReturned - 1);
			section = retString.Split('\0');
			Marshal.FreeCoTaskMem(retStringPtr);
			return true;
		}
		
		bool CanChangeToUsbDrive()
		{
			var allDrives = DriveInfo.GetDrives();
			foreach (var d in allDrives)
			{
				if (d.Name[0] > 'D' && d.DriveType == DriveType.Removable)
				{
					m_rollbackIni = d.Name + @"rollback.ini";
					m_updateIni = d.Name + "update.ini";
					m_updateDrive = new DriveInfo(d.Name);
					return true;
				}
			}
			return false;
		}
	}
}
