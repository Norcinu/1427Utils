using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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

			// reboot machine.
		}
	}
}
