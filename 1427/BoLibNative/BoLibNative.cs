using System.Runtime.InteropServices;
using System;

namespace PDTUtils.Native
{
	static class BoLib
	{
#if DEBUG
		const string dllName = "BoLibDllD.dll";
#else
        const string dllName = "BoLibDll.dll";
#endif
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int setEnvironment();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int closeSharedMemory();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int getDoorStatus();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int refillKeyStatus();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int getError();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string getCurrentError();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string getErrorText();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string getLastGame(int index);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		public unsafe static extern UInt32 getWinningGame(int index);

		/*[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string Bo_GetWinningGame(int index);*/

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint getPerformanceMeter(byte Offset);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint getGamePerformanceMeter(uint Offset, uint MeterType);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int getLocalMasterVolume();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void setLocalMasterVolume(uint val);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		public unsafe static extern UInt32 getGameModel(int index);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern UInt32 getGameDate(int index);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint getSwitchStatus(byte offset, byte mask);

		/************************************************************************/
		/*							Set methods                                 */
		/************************************************************************/
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void setLampStatus(byte offset, byte mask, byte state);
	}
}
