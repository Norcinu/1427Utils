using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PDTUtils.BoLibNative
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private class NativeMemLayout
	{
		/*public ulong dwAvailPageFile;
		public ulong dwAvailPhys;
		public ulong dwAvailVirtual;
		public ulong dwLength;
		public ulong dwMemoryLoad;
		public ulong dwTotalPageFile;
		public ulong dwTotalPhys;
		public ulong dwTotalVirtual;*/
		public uint dwLength;
		public uint dwMemoryLoad;
		public ulong dwTotalPhys;
		public ulong dwAvailPhys;
		public ulong dwTotalPageFile;
		public ulong dwAvailPageFile;
		public ulong dwTotalVirtual;
		public ulong dwAvailVirtual;

		public NativeMemLayout()
		{
			this.dwLength = (uint)Marshal.SizeOf(typeof(NativeMemLayout));
		}
	}
}
