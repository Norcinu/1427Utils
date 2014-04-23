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
		public unsafe static extern int Bo_SetEnvironment();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int Bo_Shutdown();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int Bo_GetDoorStatus();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int Bo_RefillKeyStatus();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int Bo_GetError();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string Bo_GetCurrentError();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string Bo_GetErrorText();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string Bo_GetLastGame(int index);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string Bo_GetWinningGame(int index);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint Bo_GetPerformanceMeter(byte Offset);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint Bo_GetGamePerformanceMeter(uint Offset, uint MeterType);
	}
}
