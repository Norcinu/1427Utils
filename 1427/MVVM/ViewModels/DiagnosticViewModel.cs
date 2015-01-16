using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{   
    class DiagnosticViewModel : ObservableObject
    {
        ObservableCollection<SoftwareInfo> _software = new ObservableCollection<SoftwareInfo>();
        public ObservableCollection<SoftwareInfo> Software { get { return _software; } }

        ObservableCollection<HardwareInfo> _hardware = new ObservableCollection<HardwareInfo>();
        public ObservableCollection<HardwareInfo> Hardware { get { return _hardware; } }

        public DiagnosticViewModel()
        {
            _software.Add(new SoftwareInfo("1424", "d4ec0b9ecb3a9cef0e34f8f0baaf5b13", "Authed OK"));
            _software.Add(new SoftwareInfo("1427", "d56fc00bbb954950f9259bba663f783a", "Authed OK"));
            _software.Add(new SoftwareInfo("1222", "cb6865575732a4f2b34d88296acbc139", "Authed FAILED"));
            _software.Add(new SoftwareInfo("1202", "5ad2f9afb460663caef2e40092a81cf4", "Authed OK"));
            _software.Add(new SoftwareInfo("1172", "197a5baaaa1d62d1ce5393036d72ea5e", "Authed OK"));
            _software.Add(new SoftwareInfo("1074", "d1dffea7f44516b35a1b082de43de56c", "Authed OK"));
            _software.Add(new SoftwareInfo("1077", "f5de9a3527b0bd6e5ca739732e5c3dc5", "Authed OK"));
            _software.Add(new SoftwareInfo("1199", "5f23e92850e30f5d9eee14d5cd407d1f", "Authed OK"));

            _hardware.Add(new HardwareInfo("76505", "TERMINAL_01", "Development", "S430", "TS22 - L29"));

            RaisePropertyChangedEvent("Hardware");
            RaisePropertyChangedEvent("Software");
        }
    }
}
