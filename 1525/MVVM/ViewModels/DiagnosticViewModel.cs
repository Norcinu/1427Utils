using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Text;
using PDTUtils.Access;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class DiagnosticViewModel : ObservableObject
    {
        public ObservableCollection<SoftwareInfo> Software { get; private set; }
        public ObservableCollection<HardwareInfo> Hardware { get; private set; }
        public ObservableCollection<string> GeneralList { get; set; }
        public string License { get { return Hardware[0].License; } }
        
        System.Windows.Visibility _viewVisibility;
        public System.Windows.Visibility ViewVisibility
        {
            get { return _viewVisibility; }
            set
            {
                _viewVisibility = value;
                RaisePropertyChangedEvent("ViewVisibility");
            }
        }
        
        MachineInfo _machineData;
        
        int _accessLevel;// = (int)SmartCardGroups.Manufacturer & (int)SmartCardGroups.Distributor;
        public bool AccessLevel
        {
            get
            {
                if (((_accessLevel >> (int)SmartCardGroups.Manufacturer) & 1) == 1)
                    return true;

                if (((_accessLevel >> (int)SmartCardGroups.Distributor) & 1) == 1)
                    return true;

                return false;
            }
        }
        //
        public DiagnosticViewModel(MachineInfo machineData)
        {
            _accessLevel |= 1 << (int)SmartCardGroups.Manufacturer;
            _accessLevel |= 1 << (int)SmartCardGroups.Distributor;
            
            var _arr = new System.Collections.BitArray(8);
            _arr[(int)SmartCardGroups.Distributor] = true;
            
            _machineData = machineData;
            Hardware = new ObservableCollection<HardwareInfo>();
            Software = new ObservableCollection<SoftwareInfo>();
            GeneralList = new ObservableCollection<string>();
            
            var ini = Properties.Resources.machine_ini;
            
            var buffer = new StringBuilder(64);
            NativeWinApi.GetPrivateProfileString("Exe", "Game Exe", "", buffer, 64, ini);
            var hash = "";
            var status = CheckHashIsAuthed(buffer, ref hash);
            Software.Add(new SoftwareInfo("1524", hash, status));
                
            for (var i = 0; i < BoLib.getNumberOfGames(); i++)
            {
                var exe = new StringBuilder(64);
                var dir = new StringBuilder(64);

                NativeWinApi.GetPrivateProfileString("Game" + (i + 1), "Exe", "", exe, 64, ini);
                NativeWinApi.GetPrivateProfileString("Game" + (i + 1), "GameDirectory", "", dir, 64, ini);
                
                var fullPath = new StringBuilder(dir + @"\" + exe);
                status = CheckHashIsAuthed(fullPath, ref hash);
                Software.Add(new SoftwareInfo(dir.ToString().TrimStart("\\".ToCharArray()), hash, status));
            }
            
            char[] licenseBuffer = new char[128];
            NativeWinApi.GetPrivateProfileString("Keys", "License", "", licenseBuffer, 128, Properties.Resources.machine_ini);
            //NativeWinApi.GetPrivateProfileString("Key", "License", "", licenseBuffer, 128, Properties.Resources.machine_ini);

            string license = new string(licenseBuffer, 0, 128).Trim("\0".ToCharArray());
            for (int i = 0; i < license.Length; i++)
            {
                if ((i % 15 == 0) && i > 0)
                    license = license.Insert(i, "-");
            }
            
            //var serial = BoLib.getSerialNumber();
            Hardware.Add(new HardwareInfo()
            {
                SerialKey = BoLib.getSerialNumber(),//serial,
                MachineName = System.Environment.MachineName, //"TERMINAL_01",
                License = license,
                CpuType = "S430",
                CabinetType =/* (BoLib.GetUniquePcbID(0)) ? */"INNOCORE TS22 L29" /*: "AXIS - L29"*/,
                CpuID = BoLib.GetUniquePcbID(0)
            });
            
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    Hardware[0].IPAddress = ip.Address.ToString();
                    Hardware[0].Subnet = ip.IPv4Mask.ToString();
                    Hardware[0].DefGateway = ni.GetIPProperties().GatewayAddresses[0].Address.ToString();
                }
            }

            var code = MachineDescription.CountryCode; //BoLib.getCountryCode();
            GeneralList.Add("Country Code: (" + code + ") " + BoLib.getCountryCodeStrLiteral("", code));
            GeneralList.Add("Printer Port: COM2");
            GeneralList.Add(BoLib.getEDCTypeStr());
            GeneralList.Add(_machineData.GetScreenResolution());
            GeneralList.Add(_machineData.GetOsVersion());
            GeneralList.Add(_machineData.GetMemoryInfo());
            GeneralList.Add("Game Provider: Project Coin");
            GeneralList.Add(_machineData.GetUpdateKey());
            GeneralList.Add("Last Security Check: " + _machineData.GetLastMd5Check());
            
            RaisePropertyChangedEvent("PropertyChanged");
            RaisePropertyChangedEvent("Hardware");
            RaisePropertyChangedEvent("Software");
            RaisePropertyChangedEvent("GeneralList");
        }
        
        string CheckHashIsAuthed(StringBuilder buffer, ref string hash)
        {
            var isAuthed = NativeMD5.CheckHash(@"d:" + buffer);
            
            if (isAuthed)
            {
                var h = NativeMD5.CalcHashFromFile(buffer.ToString());
                buffer[0] = '0';
                var hex = NativeMD5.HashToHex(h);
                hash = hex;
                
                if (hex != null)
                    return "AUTHED OK";
            
                return "ERROR CALCULATING HASH CODE";
            }
            
            return "AUTH FAILED";
        }
    }
}
