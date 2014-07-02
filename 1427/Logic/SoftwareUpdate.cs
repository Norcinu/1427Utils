using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using PDTUtils.Logic;
using PDTUtils.Native;
using System.Diagnostics;

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
						MyDebug<string>.WriteToFile("copy_file2.txt", "The str is " + str);
						DoCopyFile(str);
					}

					foreach (var str in folders_section)
					{
						m_filesToUpdate.Add(new FileImpl(str, str));
						DoCopyDirectory(str,0);
					}
					
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
			try
			{
				var attributes = File.GetAttributes(fileToCopy);
				if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
					return false;
			}
			catch (System.Exception ex)
			{
				MyDebug<string>.WriteToFile(Properties.Resources.usb_error_log , ex.Message);
			}

			var source = m_updateDrive.Name + fileToCopy;
			var destination = @"D:" + fileToCopy;
			var rename = destination + "_old";

			try
			{
				if (File.Exists(destination) == false)
				{
					try
					{
						File.Copy(source, destination, false);
					}
					catch (Exception ex)
					{
						MyDebug<string>.WriteToFile(Properties.Resources.usb_error_log, "The Destination file seems to exist." + ex.Message);
					}
				}
				else
				{
					try
					{
						File.Copy(destination, rename, true);
						File.Copy(source, destination, true);
						AddToRollBack(rename, 0);
					}
					catch (Exception ex)
					{
						MyDebug<string>.WriteToFile("copy_file.txt", "The Destination file does exist." + ex.Message);
					}
				}
			}
			catch (System.Exception ex)
			{
				MyDebug<string>.WriteToFile(Properties.Resources.usb_error_log, ex.Message);
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
			return false;
		}

		bool DoCopyDirectory(string path, int dirFlag)
		{
			string source_folder = m_updateDrive + path;
			string destination_folder = @"d:\";
			string rename_folder = destination_folder + @"_old";
			
			if (!Directory.Exists(destination_folder))
			{
				try
				{
					// destination doesnt exist so create folder.
					Directory.CreateDirectory(destination_folder);
					GetAndCopyAllFiles(new DirectoryInfo(source_folder), destination_folder);
				}
				catch (System.Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			else
			{
				// Need to handle the case where we are only updating part of the folder.
				// for instance updating the bmp + wav folder we obviously need the other 
				// folders or the game won't run. At current it doesnt do this we just
				// block move the entire folder.
				try
				{
					// folder does exist move it to _old
					// and create new folder
					if (Directory.Exists(rename_folder))
						Directory.Delete(rename_folder, true);
					
					Directory.Move(destination_folder, rename_folder);
					Directory.CreateDirectory(destination_folder);
					DirectoryInfo srcInfo = new DirectoryInfo(source_folder);
					AddToRollBack(rename_folder, 0);

					GetAndCopyAllFiles(srcInfo, destination_folder);
				}
				catch (System.Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			
			DirectoryInfo d = new DirectoryInfo(source_folder);
			
			var files = d.GetFiles();
			foreach (var fi in files)
			{
				if (NativeMD5.CheckHash(source_folder + fi.Name) || !NativeMD5.CheckFileType(source_folder + fi.Name))
				{
					File.SetAttributes(destination_folder + fi.Name, FileAttributes.Normal);
					var destAttr = File.GetAttributes(destination_folder + fi.Name);
					if ((destAttr & FileAttributes.Normal) == FileAttributes.Normal)
					{
						var retries = 10;
						while (!NativeMD5.CheckHash(destination_folder + fi.Name) && retries > 0)
						{
							NativeMD5.AddHashToFile(destination_folder + fi.Name);
							retries--;
						}
						return true;
					}
				}
			}
			
			return true;
		}
		// i am a anosey neighbiour
		private void GetAndCopyAllFiles(DirectoryInfo srcInfo, string destination_folder)
		{
			try 
			{
				var files = srcInfo.GetFiles();
				foreach (var f in files)
				{
					f.CopyTo(Path.Combine(destination_folder, f.Name));
				}
			}
			catch (System.Exception ex)
			{
				MyDebug<string>.WriteToFile(Properties.Resources.usb_error_log, ex.Message);
			}
		}
	}
}

