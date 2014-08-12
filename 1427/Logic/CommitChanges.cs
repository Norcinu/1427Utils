using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace PDTUtils
{
    /// <summary>
    /// This class commits any changes that are made to shared memory.
    /// </summary>
	public static class DiskCommit
	{
        /// <summary>
        /// Runs the EWFMGR command which commits changes to shared memory.
        /// This must always be called when changes are made and the machine must
        /// be rebooted.
        /// </summary>
		public static void Save()
		{
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.FileName = "cmd.exe";
			startInfo.Arguments = "/C EWFMGR C: -COMMIT";
			process.StartInfo = startInfo;
			process.Start();
		}

        /// <summary>
        /// Forces the machine to reboot.
        /// </summary>
		public static void RebootMachine()
		{
			ManagementClass W32_OS = new ManagementClass("Win32_OperatingSystem");
			ManagementBaseObject inParams, outParams;
			int result;
			W32_OS.Scope.Options.EnablePrivileges = true;

			foreach(ManagementObject obj in W32_OS.GetInstances())
			{
				inParams = obj.GetMethodParameters("Win32Shutdown");
				inParams["Flags"] = 6; //ForcedReboot;
				inParams["Reserved"] = 0;

				outParams = obj.InvokeMethod("Win32Shutdown", inParams, null);
				result = Convert.ToInt32(outParams["returnValue"]);
				if (result != 0) 
					throw new Win32Exception(result);
			}
		}

        public static void SaveAndReboot()
        {
            Save();
            RebootMachine();
        }
	}
}
