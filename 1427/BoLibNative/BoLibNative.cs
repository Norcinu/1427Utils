using System.Runtime.InteropServices;
using System;

namespace PDTUtils.Native
{
	static class BoLibNative
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
		public unsafe static extern int Bo_GetCurrentError();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern ulong[] GetLastGames();
	}
}
