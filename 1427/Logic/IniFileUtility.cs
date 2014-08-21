using System;
using System.Runtime.InteropServices;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
    static class IniFileUtility
    {
        public static bool GetIniProfileSection(out string[] section, string field, string file)
        {
            uint bufferSize = 4048;
            IntPtr retStringPtr = Marshal.AllocCoTaskMem((int)bufferSize * sizeof(char));
            uint bytesReturned = NativeWinApi.GetPrivateProfileSection(field, retStringPtr, bufferSize, @file);
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
    }
}
