﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.ComponentModel;

namespace PDTUtils
{
	public static class CommitChanges
	{
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
	}
}
