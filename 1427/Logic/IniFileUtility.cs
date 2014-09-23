using System;
using System.Runtime.InteropServices;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
    static class IniFileUtility
    {
        public static bool GetIniProfileSection(out string[] section, string field, string file, bool removeField=false)
        {
            uint bufferSize = 4048;
            IntPtr retStringPtr = Marshal.AllocCoTaskMem((int)bufferSize * sizeof(char));
            var bytesReturned = NativeWinApi.GetPrivateProfileSection(field, retStringPtr, bufferSize, @file);
            if ((bytesReturned == bufferSize - 2) || (bytesReturned == 0))
            {
                section = null;
                Marshal.FreeCoTaskMem(retStringPtr);
                return false;
            }
            
            string retString = Marshal.PtrToStringAuto(retStringPtr, bytesReturned - 1);
            if (!removeField)
                section = retString.Split('\0');
            else
            {
                section = retString.Split('\0');
                for (int i = 0; i < section.Length; i++)
                {
                    if (section[i].Length > 4 )
                        section[i] = section[i].Substring(section[i].IndexOf("=")+1);
                }
             }

            Marshal.FreeCoTaskMem(retStringPtr);
            return true;
        }
    }
}
