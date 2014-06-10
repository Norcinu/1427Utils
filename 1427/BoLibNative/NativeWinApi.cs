﻿using System;
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


		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern uint GetPrivateProfileString(string lpAppName,
														  string lpKeyName,
														  string lpDefault,
												          StringBuilder lpReturnedString,
												          uint nSize,
												          string lpFileName);


		[DllImport("kernel32.dll")]
		public static extern uint GetPrivateProfileSection(string lpAppName,
														   IntPtr lpReturnedString, 
														   uint nSize, 
														   string lpFileName);



	}
}