using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PDTUtils.Native
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	static class Games
	{
		public class GameInfo
		{
			public string name = "";
			public string hash_code = "";
			public string path = "";

			public string Name { get { return name; } }
			public string Hash_code { get { return hash_code; } }
			public string Path { get { return path; } }
		}
	}

#pragma warning disable 0169, 0649
    [System.Security.SuppressUnmanagedCodeSecurity]
	static class NativeWinApi
	{
		public enum ModeNum : int
		{
			ENUM_CURRENT_SETTINGS = -1,
			ENUM_REGISTRY_SETTINGS = -2
		}

		public struct MEMORYSTATUS
		{
			public uint dwLength;
			public uint dwMemoryLoad;
			public uint dwTotalPhys;
			public uint dwAvailPhys;
			public uint dwTotalPageFile;
			public uint dwAvailPageFile;
			public uint dwTotalVirtual;
			public uint dwAvailVirtual;
		}

		[DllImport("kernel32.dll")]
		public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINTL
		{
			public int x;
			public int y;
		}

		internal enum DMCOLOR : short
		{
			DMCOLOR_UNKNOWN = 0,
			DMCOLOR_MONOCHROME = 1,
			DMCOLOR_COLOR = 2
		}

		[Flags()]
		public enum DM : int
		{
			Orientation = 0x1,
			PaperSize = 0x2,
			PaperLength = 0x4,
			PaperWidth = 0x8,
			Scale = 0x10,
			Position = 0x20,
			NUP = 0x40,
			DisplayOrientation = 0x80,
			Copies = 0x100,
			DefaultSource = 0x200,
			PrintQuality = 0x400,
			Color = 0x800,
			Duplex = 0x1000,
			YResolution = 0x2000,
			TTOption = 0x4000,
			Collate = 0x8000,
			FormName = 0x10000,
			LogPixels = 0x20000,
			BitsPerPixel = 0x40000,
			PelsWidth = 0x80000,
			PelsHeight = 0x100000,
			DisplayFlags = 0x200000,
			DisplayFrequency = 0x400000,
			ICMMethod = 0x800000,
			ICMIntent = 0x1000000,
			MediaType = 0x2000000,
			DitherType = 0x4000000,
			PanningWidth = 0x8000000,
			PanningHeight = 0x10000000,
			DisplayFixedOutput = 0x20000000
		}

		/// <summary>
		/// Specifies whether collation should be used when printing multiple copies.
		/// </summary>
		internal enum DMCOLLATE : short
		{
			/// <summary>
			/// Do not collate when printing multiple copies.
			/// </summary>
			DMCOLLATE_FALSE = 0,

			/// <summary>
			/// Collate when printing multiple copies.
			/// </summary>
			DMCOLLATE_TRUE = 1
		}

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
		public struct DEVMODE
		{
			public const int CCHDEVICENAME = 32;
			public const int CCHFORMNAME = 32;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
			[FieldOffset(0)]
			public string dmDeviceName;
			[FieldOffset(32)]
			public Int16 dmSpecVersion;
			[FieldOffset(34)]
			public Int16 dmDriverVersion;
			[FieldOffset(36)]
			public Int16 dmSize;
			[FieldOffset(38)]
			public Int16 dmDriverExtra;
			[FieldOffset(40)]
			public DM dmFields;

			[FieldOffset(44)]
			Int16 dmOrientation;
			[FieldOffset(46)]
			Int16 dmPaperSize;
			[FieldOffset(48)]
			Int16 dmPaperLength;
			[FieldOffset(50)]
			Int16 dmPaperWidth;
			[FieldOffset(52)]
			Int16 dmScale;
			[FieldOffset(54)]
			Int16 dmCopies;
			[FieldOffset(56)]
			Int16 dmDefaultSource;
			[FieldOffset(58)]
			Int16 dmPrintQuality;

			[FieldOffset(44)]
			public POINTL dmPosition;
			[FieldOffset(52)]
			public Int32 dmDisplayOrientation;
			[FieldOffset(56)]
			public Int32 dmDisplayFixedOutput;

			[FieldOffset(60)]
			public short dmColor; // See note below!
			[FieldOffset(62)]
			public short dmDuplex; // See note below!
			[FieldOffset(64)]
			public short dmYResolution;
			[FieldOffset(66)]
			public short dmTTOption;
			[FieldOffset(68)]
			public short dmCollate; // See note below!
			[FieldOffset(72)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
			public string dmFormName;
			[FieldOffset(102)]
			public Int16 dmLogPixels;
			[FieldOffset(104)]
			public Int32 dmBitsPerPel;
			[FieldOffset(108)]
			public Int32 dmPelsWidth;
			[FieldOffset(112)]
			public Int32 dmPelsHeight;
			[FieldOffset(116)]
			public Int32 dmDisplayFlags;
			[FieldOffset(116)]
			public Int32 dmNup;
			[FieldOffset(120)]
			public Int32 dmDisplayFrequency;
		}

		[DllImport("user32.dll")]
		public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

		[DllImport("user32.dll")]
		public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

		public const int ENUM_CURRENT_SETTINGS = -1;
		public const int CDS_UPDATEREGISTRY = 0x01;
		public const int CDS_TEST = 0x02;
		public const int DISP_CHANGE_SUCCESSFUL = 0;
		public const int DISP_CHANGE_RESTART = 1;
		public const int DISP_CHANGE_FAILED = -1;

		public struct OSVERSIONINFO
		{
			public uint dwOSVersionInfoSize;
			public uint dwMajorVersion;
			public uint dwMinorVersion;
			public uint dwBuildNumber;
			public uint dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
			public Int16 wServicePackMajor;
			public Int16 wServicePackMinor;
			public Int16 wSuiteMask;
			public Byte wProductType;
			public Byte wReserved;
		}

		[DllImport("kernel32")]
		public static extern bool GetVersionEx(ref OSVERSIONINFO osvi);
       
       
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
                                                               uint nSize,
                                                               string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileString(string lpAppName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          StringBuilder lpReturnedString,
                                                          int nSize,
                                                          string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileString(string lpAppName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          [In, Out] char[] lpReturnedString,
                                                          int nSize,
                                                          string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileString(string lpAppName,
                                                         string lpKeyName,
                                                         string lpDefault,
                                                         IntPtr lpReturnedString,
                                                         uint nSize,
                                                         string lpFileName);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileInt(string lpAppName,
                                                      string lpKeyName,
                                                      int lpDefault,
                                                      string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileSection(string lpAppName,
                                                          IntPtr lpReturnedString,
                                                          uint nSize,
                                                          string lpFileName);

        // We explicitly enable the SetLastError attribute here because
        // WritePrivateProfileString returns errors via SetLastError.
        // Failure to set this can result in errors being lost during 
        // the marshal back to managed code.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrivateProfileSection(string lpAppName,
                                                             string lpString,
                                                             string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrivateProfileString(string lpAppName,
                                                            string lpKeyName, 
                                                            string lpString, 
                                                            string lpFileName);
        
		[DllImport("kernel32.dll")]
		public static extern bool SetFileAttributes(string lpFileName, 
													uint dwFileAttributes);
        
		public const int FILE_ATTRIBUTE_NORMAL = 0x80;
        
		public struct SYSTEMTIME
		{
			public short year;
			public short month;
			public short dayOfWeek;
			public short day;
			public short hour;
			public short minute;
			public short second;
			public short milliseconds;
		}
        
		[DllImport("coredll.dll")]
		private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("coredll.dll")]
		private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		public static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

		[DllImport("kernel32.dll")]
		public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);
	}
}
