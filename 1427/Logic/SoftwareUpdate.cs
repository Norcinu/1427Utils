using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.Native;

namespace PDTUtils
{
	public class FileImpl
	{
		public string Name { get; set; }
        public string Avatar { get; set; }
		public bool? IsFile { get; set; }

		public FileImpl(string name, string av)
		{
			Name = name;
			Avatar = av;
			IsFile = null;
		}
        
        public FileImpl(string name, string av, bool isFile)
        {
            Name = name;
            Avatar = av;
            IsFile = isFile;
        }
	}
    
	public class UserSoftwareUpdate : BaseNotifyPropertyChanged
	{
		string _rollbackIni; 
		string _updateIni;
        
		DriveInfo _updateDrive;
        
		#region PROPERTIES
		public uint FileCount { get; set; }

		public string UpdateIni
		{
			get { return _updateIni; }
			set { _updateIni = value; }
		}

        ObservableCollection<string> _filesNotCopied;

        public bool HasUpdateStarted { get; set; }
        public bool HasUpdateFinished { get; set; }

        public ObservableCollection<FileImpl> FilesToUpdate { get; set; }
        public string LogText { get; set; }
        #endregion

        public ICommand UpdatePrep { get; set; }
        public ICommand Update { get; set; }
        public ICommand Rollback { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Reboot { get; set; }
        
        public UserSoftwareUpdate(FrameworkElement element)
		{
            FilesToUpdate = new ObservableCollection<FileImpl>();
            _filesNotCopied = new ObservableCollection<string>();
            
            HasUpdateStarted = false;
            HasUpdateFinished = false;

            UpdatePrep  = new RoutedCommand();
            Update      = new RoutedCommand();
            Rollback    = new RoutedCommand();
            Cancel      = new RoutedCommand();
            Reboot      = new RoutedCommand();
            
            CommandManager.RegisterClassCommandBinding(element.GetType(), new CommandBinding(UpdatePrep, DoSoftwareUpdatePreparation));
            CommandManager.RegisterClassCommandBinding(element.GetType(), new CommandBinding(Update, DoSoftwareUpdate));
            CommandManager.RegisterClassCommandBinding(element.GetType(), new CommandBinding(Rollback, DoRollBack));
            CommandManager.RegisterClassCommandBinding(element.GetType(), new CommandBinding(Cancel, DoCancelUpdate));
            CommandManager.RegisterClassCommandBinding(element.GetType(), new CommandBinding(Reboot, DoSaveReboot));//
		}
        
		public void DoRollBack(object o, RoutedEventArgs e)
		{
            LogText += "Performing RollBack.\r\n----------------------------\r\n";
            OnPropertyChanged("LogText");
            FileCount = 0;
            FilesToUpdate.Clear();
            _filesNotCopied.Clear();
            HasUpdateFinished = false;
            HasUpdateStarted = false;
            OnPropertyChanged("HasUpdateFinished");
            OnPropertyChanged("HasUpdateStarted");
		}
        
        public void DoSoftwareUpdatePreparation(object o, ExecutedRoutedEventArgs e)
		{
            if (CanChangeToUsbDrive())
            {
                // we can look for update.ini
                if (File.Exists(_updateIni))
                {
                    HasUpdateStarted = true;
                    OnPropertyChanged("HasUpdateStarted");
                    
                    string[] foldersSection = null;
                    string[] filesSection = null;
                    bool[] quit = new bool[2] { false, false };

                    BoLib.setFileAction();

                    quit[0] = ReadIniSection(out foldersSection, "Folders");
                    quit[1] = ReadIniSection(out filesSection, "Files");
          
                    BoLib.clearFileAction();
          
                    if (quit[0] || quit[1])
                        return;
                    
                    LogText = String.Format("Finding Files. {0} Total Files.\r\n", filesSection.Length);
                    OnPropertyChanged("LogText");
                    
                    foreach (var str in filesSection)
                    {
                        var ret = GetImagePathString(str);
                        FilesToUpdate.Add(new FileImpl(str, ret, true));
                        FileCount++;
                        OnPropertyChanged("UpdateFiles");
                    }

                    LogText += String.Format("Finding Folders. {0} Total Folders.\r\n", foldersSection.Length);
                    OnPropertyChanged("LogText");

                    foreach (var str in foldersSection)
                    {
                        var ret = GetImagePathString(str);
                        FilesToUpdate.Add(new FileImpl(str, ret, false));
                        FileCount++;
                        OnPropertyChanged("UpdateFiles");
                    }
                    
                    LogText += "Backup Created.\r\n----------------------------\r\n";
                    OnPropertyChanged("LogText");
                    OnPropertyChanged("UpdateFiles");
                }
            }
            else
                SetNoUsbDriveMessage();
		}

        private void SetNoUsbDriveMessage()
        {
            LogText = "USB Update Not Found.\r\nPlease connect USB device and try again.\r\n";
            OnPropertyChanged("LogText");
        }
	    
		public void DoSoftwareUpdate(object o, RoutedEventArgs e)
		{
            if (FilesToUpdate.Count > 0)
            {
                FilesToUpdate.Clear();
                FileCount = 0;
            }
            
            if (CanChangeToUsbDrive())
			{
				// we can look for update.ini
				if (File.Exists(_updateIni))
				{
					string[] foldersSection = null;
					string[] filesSection = null;
					bool[] quit = new bool[2] { false, false };
				    
					BoLib.setFileAction();
					
                    quit[0] = ReadIniSection(out foldersSection, "Folders");
					quit[1] = ReadIniSection(out filesSection, "Files");
                   
					BoLib.clearFileAction();
                    
					if (quit[0] || quit[1])
						return;
                    
					foreach (var str in filesSection)
					{
						var ret = GetImagePathString(str);
						FilesToUpdate.Add(new FileImpl(str, ret, true));
                        if (DoCopyFile(str))
                            FileCount++;
					}
					
                    foreach (var str in foldersSection)
					{
						var ret = GetImagePathString(str);
						FilesToUpdate.Add(new FileImpl(str, ret, false));
                        //new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory(@"e:\1111", @"d:\1111_VB", true);
                        DoCopyDirectory(str, 0);
					}

                    // Move old files back.
                    // Read rollback.ini.
                    // That copy works nicely. Odd that there isnt a C# version.
                    // Move old files back.
                    
                    HasUpdateFinished = true;
                    OnPropertyChanged("HasUpdateFinished");
					OnPropertyChanged("UpdateFiles");
                    
                    CleanUp();
				}
			}
		}
        
        void CleanUp()
        {          
            //run through looking for _old files + folders and delete them.
            DirectoryInfo dDrive = new DirectoryInfo(@"D:\");
            var fileList = dDrive.GetFiles();
            var dirList = dDrive.GetDirectories();
            foreach (var dir in dirList)
            {
                try
                {
                    if (dir.Name.Contains("_old"))
                        dir.Delete(true);         
                }
                catch (Exception ex)
                {
                    LogText = ex.Message;
                    OnPropertyChanged("LogText");
                }
            }
            
        }
        
        bool ReadIniSection(out string[] section, string field)
		{
            bool? result = IniFileUtility.GetIniProfileSection(out section, field, _updateIni);
			if (result == false || section == null)
				return true;
            return false;
		}
		
        private static string GetImagePathString(string str)
		{
			CustomImagePathConverter conv = new CustomImagePathConverter();
			var ret = conv.Convert(str, typeof(string), null, CultureInfo.InvariantCulture) as string;
			return ret;
		}
		
		private bool GetIniProfileSection(out string[] section, string field)
		{
			uint bufferSize = 4048;
			IntPtr retStringPtr = Marshal.AllocCoTaskMem((int)bufferSize * sizeof(char));
			var bytesReturned = NativeWinApi.GetPrivateProfileSection(field, retStringPtr, bufferSize, _updateIni);
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
					_rollbackIni = d.Name + @"rollback.ini";
					_updateIni = d.Name + "update.ini";
					_updateDrive = new DriveInfo(d.Name);
					return true;
				}
			}
			return false;
		}

	    protected override void ParseGame(int gameNo)
		{
			throw new Exception("The method or operation is not implemented.");
		}
        
		void AddToRollBack(string path, int flag)
		{
			BoLib.setFileAction();
			if (flag == 1)
				NativeWinApi.WritePrivateProfileSection("Folders", path, _rollbackIni);
			else
				NativeWinApi.WritePrivateProfileSection("Files", path, _rollbackIni);
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
                Debug.WriteLine(ex.Message);
			}
            
			var source = _updateDrive.Name + fileToCopy;
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
						Debug.WriteLine(ex.Message);
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
						Debug.WriteLine(ex.Message); 
					}
				}
			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(ex.Message);
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
			string sourceFolder = _updateDrive + path;
			string destinationFolder = @"d:\" + path;//no path?
			string renameFolder = destinationFolder + @"_old";
			
			if (!Directory.Exists(destinationFolder))
			{
				try
				{
					DirectoryInfo srcInfo = new DirectoryInfo(sourceFolder);
					foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*",
						SearchOption.AllDirectories))
						Directory.CreateDirectory(dirPath.Replace(sourceFolder, destinationFolder));
				    
					//Copy all the files & Replaces any files with the same name
					foreach (string newPath in Directory.GetFiles(sourceFolder, "*.*",
						SearchOption.AllDirectories))
						File.Copy(newPath, newPath.Replace(sourceFolder, destinationFolder), true);
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.Message);
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
                    if (Directory.Exists(renameFolder))
                    {
                        Directory.Delete(renameFolder, true);
                        
                        Directory.Move(destinationFolder, renameFolder);
                        Directory.CreateDirectory(destinationFolder);
                        DirectoryInfo dstInfo = new DirectoryInfo(renameFolder);
                        
                        foreach (string dirPath in Directory.GetDirectories(renameFolder, "*",
                                 SearchOption.AllDirectories))
                                 Directory.CreateDirectory(dirPath.Replace(renameFolder, destinationFolder));
                        
                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(renameFolder, "*.*",
                                 SearchOption.AllDirectories))
                                 File.Copy(newPath, newPath.Replace(renameFolder, destinationFolder), true);
                        //maybe just copy the files and folders over instead of moving.
                        DirectoryInfo srcInfo = new DirectoryInfo(sourceFolder);
                        AddToRollBack(renameFolder, 1);
                        GetAndCopyAllFiles(srcInfo, destinationFolder);
                        
                        DirectoryInfo d = new DirectoryInfo(sourceFolder);
                        var files = d.GetFiles();
                        foreach (var fi in files)
                        {
                            if (NativeMD5.CheckHash(sourceFolder + fi.Name) || !NativeMD5.CheckFileType(sourceFolder + fi.Name))
                            {
                                File.SetAttributes(destinationFolder + fi.Name, FileAttributes.Normal);
                                var destAttr = File.GetAttributes(destinationFolder + fi.Name);
                                if ((destAttr & FileAttributes.Normal) == FileAttributes.Normal)
                                {
                                    var retries = 10;
                                    while (!NativeMD5.CheckHash(destinationFolder + fi.Name) && retries > 0)
                                    {
                                        NativeMD5.AddHashToFile(destinationFolder + fi.Name);
                                        retries--;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
            
			return true;
		}

		void GetAndCopyAllFiles(DirectoryInfo srcInfo, string destinationFolder)
		{
			try 
			{
				var files = srcInfo.GetFiles();
				foreach (var f in files)
				{
					f.CopyTo(Path.Combine(destinationFolder, f.Name));
				}
			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

        //loop through backed up items and check to see if they exist in 'new' source folder.
        //if yes then do nothing, if no then copy back.
        void ReverseBackedUpItems(string path, string source)
        {
            foreach (var f in Directory.GetFiles(path))
            {
                if (!File.Exists(f))
                {
                    File.Copy(path, source);
                }
            }
        }
		
        public void DoCancelUpdate(object o, RoutedEventArgs e)
		{
            HasUpdateStarted = false;
            HasUpdateFinished = false;
			FileCount = 0;
			FilesToUpdate.Clear();
			_filesNotCopied.Clear();
            LogText = "";
            OnPropertyChanged("LogText");
            OnPropertyChanged("HasUpdateStarted");
            OnPropertyChanged("HasUpdateFinished"); 
		}
		
		public void DeleteRollBack()
		{
			string[] foldersSection = null;
			string[] filesSection = null;
            
			BoLib.setFileAction();
            bool? result = IniFileUtility.GetIniProfileSection(out foldersSection, "folders", _updateIni);
			if (result != true)
				return;
            
            result = IniFileUtility.GetIniProfileSection(out filesSection, "files", _updateIni);
			if (result != true)
				return;
			BoLib.clearFileAction();
		}
        
        public void DoSaveReboot(object o, ExecutedRoutedEventArgs e)
        {
            LogText = "Restarting Machine.\r\n\r\nPlease turn the Refill Key and remove USB device.";
            OnPropertyChanged("LogText");
            DiskCommit.SaveAndReboot();
        }
	}
}
