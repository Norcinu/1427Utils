using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using PDTUtils.Logic;
using PDTUtils.Native;

namespace PDTUtils
{
	public class FileImpl
	{
		public string Name { get; set; }
		public string Avatar { get; set; }

		public FileImpl(string _name, string _av)
		{
			Name = _name;
			Avatar = _av;
		}
	}

	public class UserSoftwareUpdate : BaseNotifyPropertyChanged
	{
		string m_rollbackIni; 
		string m_updateIni;
		
		DriveInfo m_updateDrive;
		//ObservableCollection<string> m_filesToUpdate = new ObservableCollection<string>();
		ObservableCollection<FileImpl> m_filesToUpdate = new ObservableCollection<FileImpl>();
		
		#region PROPERTIES
		public string UpdateIni
		{
			get { return m_updateIni; }
			set { m_updateIni = value; }
		}

		public ObservableCollection<FileImpl> FilesToUpdate
		{
			get { return m_filesToUpdate; }
			set { m_filesToUpdate = value; }
		}
		#endregion

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
					bool quit = false;

					BoLib.setFileAction();
					bool? b = GetIniProfileSection(out folders_section, "Folders");
					if (b == false || folders_section == null)
						quit = true;

					b = GetIniProfileSection(out files_section, "Files");
					if (b == false || folders_section == null)
						quit = true;
					
					BoLib.clearFileAction();
					
					if (quit == true)
						return false;

					foreach (var str in files_section)
					{
						CustomImagePathConverter conv = new CustomImagePathConverter();
						conv.Convert(str, typeof(string), null, CultureInfo.InvariantCulture);
						m_filesToUpdate.Add(new FileImpl(str, str));
					}

					foreach (var str in folders_section)
						m_filesToUpdate.Add(new FileImpl(str, str));

					this.OnPropertyChanged("UpdateFiles");
					MyDebug<string>.WriteCollectionToFile("debug.txt", files_section);
					MyDebug<string>.WriteCollectionToFile("debug2.txt", folders_section);
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

		public override void ParseGame(int gameNo)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
