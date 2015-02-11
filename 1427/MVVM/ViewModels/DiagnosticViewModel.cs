using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;
using PDTUtils.Logic;
using PDTUtils.Native;
using System.Diagnostics;

namespace PDTUtils.MVVM.ViewModels
{   
    class DiagnosticViewModel : ObservableObject
    {
        ObservableCollection<SoftwareInfo> _software = new ObservableCollection<SoftwareInfo>();
        public ObservableCollection<SoftwareInfo> Software { get { return _software; } }

        ObservableCollection<HardwareInfo> _hardware = new ObservableCollection<HardwareInfo>();
        public ObservableCollection<HardwareInfo> Hardware { get { return _hardware; } }
        //needs more details. screen size, os, 
        
        public DiagnosticViewModel()
        {
            string ini = Properties.Resources.machine_ini;
            
            StringBuilder buffer = new StringBuilder(64);
            NativeWinApi.GetPrivateProfileString("Exe", "Game Exe", "", buffer, 64, ini);
            string status = "";
            string hash = "";
            status = CheckHashIsAuthed(buffer, ref hash);
            _software.Add(new SoftwareInfo("1524", hash, status));
            
            for (int i = 0; i < BoLib.getNumberOfGames(); i++)
            {
                StringBuilder exe = new StringBuilder(64);
                StringBuilder dir = new StringBuilder(64);
                
                NativeWinApi.GetPrivateProfileString("Game" + (i+1), "Exe", "", exe, 64, ini);
                NativeWinApi.GetPrivateProfileString("Game" + (i+1), "GameDirectory", "", dir, 64, ini);
                
                StringBuilder fullPath = new StringBuilder(dir + @"\" + exe);
                status = CheckHashIsAuthed(fullPath, ref hash);
                _software.Add(new SoftwareInfo(dir.ToString().TrimStart("\\".ToCharArray()), hash, status));
            }

            _hardware.Add(new HardwareInfo("76505", "TERMINAL_01", "Development", "S430", "TS22 - L29"));

            RaisePropertyChangedEvent("Hardware");
            RaisePropertyChangedEvent("Software");
        }
        
        private static string CheckHashIsAuthed(StringBuilder buffer, ref string hash)
        {
            bool isAuthed = NativeMD5.CheckHash(@"d:" + buffer);

            if (isAuthed)
            {
                var h = NativeMD5.CalcHashFromFile(buffer.ToString());
                buffer[0] = '0';
                var hex = NativeMD5.HashToHex(h);
                hash = hex;

                if (hex != null)
                {
                    Debug.WriteLine("AUTHED OK");
                    return "AUTHED OK"; 
                }
                else
                {
                    Debug.WriteLine("ERROR CALCULATING HASH CODE");
                    return "ERROR CALCULATING HASH CODE";
                }
            }
            else
            {
                Debug.WriteLine("AUTH FAILED");
                return "AUTH FAILED";
            }
        }
    }
}
