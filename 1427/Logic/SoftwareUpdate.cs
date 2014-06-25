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
		public bool? IsFile { get; set; }

		public FileImpl(string _name, string _av)
		{
			Name = _name;
			Avatar = _av;
			IsFile = null;
		}
	}

	public class UserSoftwareUpdate : BaseNotifyPropertyChanged
	{
		string m_rollbackIni; 
		string m_updateIni;
		
		DriveInfo m_updateDrive;
		ObservableCollection<FileImpl> m_filesToUpdate = new ObservableCollection<FileImpl>();
		ObservableCollection<string> m_filesNotCopied = new ObservableCollection<string>();

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
						var ret = conv.Convert(str, typeof(string), null, CultureInfo.InvariantCulture) as string;
						m_filesToUpdate.Add(new FileImpl(str, ret));
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

		void AddToRollBack(string path, int flag)
		{
			BoLib.setFileAction();
			if (flag == 1)
				NativeWinApi.WritePrivateProfileSection("Folders", path, m_rollbackIni);
			else
				NativeWinApi.WritePrivateProfileSection("Files", path, m_rollbackIni);
			BoLib.clearFileAction();
		}

		bool DoCopyFile(string fileToCopy)
		{
			var attributes = File.GetAttributes(fileToCopy);
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				return false;

			var source = fileToCopy;
			var destination = @"D:\" + fileToCopy;
			var rename = destination + "_old";
			
			try
			{
				Directory.Move(source, destination);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				AddToRollBack(rename, 0);
			}

			if (NativeMD5.CheckHash(source) || !NativeMD5.CheckFileType(source))
			{
				File.SetAttributes(destination, FileAttributes.Normal);
				var destAttr = File.GetAttributes(destination);
				if ((destAttr & FileAttributes.Normal) == FileAttributes.Normal)
				{
					var retries = 10;
					while (!NativeMD5.CheckHash(destination) && retries > 0)
					{
						NativeMD5.AddHashToFile(destination);
						retries--;
					}
					return true;
				}
			}
			MyDebug<string>.WriteToFile("destinations.txt", destination);
			return false;
		}

		bool DoCopyDirectory(string path, int dirFlag)
		{
			var source = m_updateDrive.Name + path;
			var destination = @"D:\" + path;
			var renameDir = destination + "_old";
			
			var flag = 0;
			if (path == "1224" || path == "1227")
				flag = 1;
			else
				flag = 0;
			
			if (Directory.Exists(destination) == false)
			{
				Directory.CreateDirectory(destination);
				Directory.SetCurrentDirectory(destination);
			}
			else if (dirFlag == flag)
			{

			}
			
			var allFiles = Directory.GetFiles(path, "*.*");
			foreach(var f in allFiles)
			{
				MyDebug<string>.WriteToFile("paths.txt", f);
				//var fullSourcePath = m_updateDrive.Name+path
				if (NativeMD5.CheckHash(source) || !NativeMD5.CheckFileType(source))
				{
					File.SetAttributes(destination, FileAttributes.Normal);
					var destAttr = File.GetAttributes(destination);
					if ((destAttr & FileAttributes.Normal) == FileAttributes.Normal)
					{
						var retries = 10;
						while (!NativeMD5.CheckHash(destination) && retries > 0)
						{
							NativeMD5.AddHashToFile(destination);
							retries--;
						}
						return true;
					}
				}
			}
			return true;
		}
	}
}

