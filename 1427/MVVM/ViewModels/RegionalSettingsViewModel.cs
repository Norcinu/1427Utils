using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;


namespace PDTUtils.MVVM.ViewModels
{
    class RegionalSettingsViewModel : ObservableObject
    {
        bool _liveHasBeenEdited = false;
        int _arcadeSelectedIndex = -1;
        int _marketSelectedIndex = -1;
        
        ObservableCollection<SpanishRegionalModel> _arcades = new ObservableCollection<SpanishRegionalModel>();
        ObservableCollection<SpanishRegionalModel> _street = new ObservableCollection<SpanishRegionalModel>();
        SpanishRegionalModel _editableLiveRegion;
        SpanishRegionalModel _regionBackup; // restore _editable to this if user presses cancel imo.
        SpainRegionSelection _selected = new SpainRegionSelection();

        readonly string _espRegionIni = Properties.Resources.esp_live_ini; 
        readonly string[] _streetMarketRegions = new string[20]
        {
            "Andalucia", "Aragón", "Asturias", "Baleares", "País Vasco", "Cantabria", "Castilla-La Mancha",
            "Castilla León", "Catalonia", "Catalonia Light", "Extremadura", "Madrid", "Murcia", "Navarra",
            "La Rioja", "Valencia", "Valencia 500", "Valencia Light", "Canarias", "Galicia"
        };
        
        readonly string[] _arcadeRegions = new string[38]
        {
            "Andalucia-1000", "Aragon-1000","Aragon-2000","Asturia-1000","Asturias-2000","Baleares-1000","Baleares-3000-(specialB)",
            "Basque-1000","Basque-2000","Basque-3000-(specialBS)","Cantabria-1000","Cantabria-2000","Castilla-la-Mancha-1000",
            "Castilla-la-Mancha-2000","Castilla-La-Mancha-6000(special)","Castilla-La-Mancha-3000(special)","Castilla-Leon-1000",
            "Catalonia-2000","Catalonia-arcade-500","Extremadura-1000","Madrid-1000","Madria-2000","Madrid-3000","Murcia-600",
            "Murcia-6000-(arcade,reservate area)","Murcia-3000-(arcade,reservate area)","Murcia-arcade-500","Navarra-1000",
            "Navarra-2000","La Rioja-1000","La Rioja-2000","Valencia-2000","Valencia-3000","Valencia-600","Valencia-1000",
            "Canarias-1000","Galicia-3600","Galicia-1800"            
        };
        
        #region Properties
        public bool FirstScreen { get; set; }
        public bool SecondScreen { get; set; }
        
        public IEnumerable<SpanishRegionalModel> Arcades { get { return _arcades; } }
        public IEnumerable<SpanishRegionalModel> Street { get { return _street; } }
        public SpainRegionSelection Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        public SpanishRegionalModel EditableLiveRegion
        {
            get { return _editableLiveRegion; }
            set { _editableLiveRegion = value; }
        }
        
        public int ArcadeSelectedIndex
        {
            get { return _arcadeSelectedIndex; }
            set
            {
                if (_marketSelectedIndex >= 0)
                    MarketSelectedIndex = -1;
                _arcadeSelectedIndex = value;
                SetRegion();

                RaisePropertyChangedEvent("ArcadeSelectedIndex");
            }
        }
        
        public int MarketSelectedIndex
        {
            get { return _marketSelectedIndex;}
            set 
            {
                if (_arcadeSelectedIndex >= 0)
                    ArcadeSelectedIndex = -1;
                _marketSelectedIndex = value;
                SetRegion();
                
                RaisePropertyChangedEvent("MarketSelectedIndex");
            }
        }
        #endregion
        
        #region Commands
        public ICommand Save { get { return new DelegateCommand(o => SaveChanges()); } }
        public ICommand Load { get { return new DelegateCommand(o => LoadSettings()); } }
        public ICommand Increment { get { return new DelegateCommand(DoIncrement); } }
        public ICommand Decrement { get { return new DelegateCommand(DoDecrement); } }
        public ICommand ResetLiveToDefault { get { return new DelegateCommand(o => DoResetLiveToDefault()); } }
        #endregion
        
        public RegionalSettingsViewModel()
        {
            int i = 0;
            foreach (string s in _streetMarketRegions)
            {
                SpanishRegional sr = new SpanishRegional();
                BoLib.getDefaultRegionValues(i, ref sr);
                _street.Add(new SpanishRegionalModel(_streetMarketRegions[i], sr));
                i++;
            }
            
            int smLength = _streetMarketRegions.Length - 1;
            i = 0;
            foreach (string arcade in _arcadeRegions)
            {
                SpanishRegional sr = new SpanishRegional();
                BoLib.getDefaultRegionValues(smLength + i, ref sr);
                _arcades.Add(new SpanishRegionalModel(_arcadeRegions[i], sr));
                i++;
            }
            _editableLiveRegion = new SpanishRegionalModel("", new SpanishRegional());
                    
            LoadSettings();        
            
            RaisePropertyChangedEvent("EditableLiveRegion");
            RaisePropertyChangedEvent("Arcades");
            RaisePropertyChangedEvent("Street");
            RaisePropertyChangedEvent("Selected");
        }
        
        public void SaveChanges()
        {
            NativeWinApi.WritePrivateProfileString("General", "Region", Selected.Community, _espRegionIni);
            NativeWinApi.WritePrivateProfileString("General", "VenueType", Selected.VenueType, _espRegionIni);
           
            if (Selected.VenueType == "Street Market")
                Selected.Id = Array.IndexOf(_streetMarketRegions, Selected.Community);
            else
                Selected.Id = Array.IndexOf(_arcadeRegions, Selected.Community) + _streetMarketRegions.Length;

            NativeWinApi.WritePrivateProfileString("General", "CurrentRegion", Selected.Id.ToString(), _espRegionIni);
            
            NativeWinApi.WritePrivateProfileString("Settings", "MaxStakeCredits", _editableLiveRegion.MaxStakeCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxStakeBank", _editableLiveRegion.MaxStakeBank.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "StakeMask", _editableLiveRegion.StakeMask.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "WinMax", _editableLiveRegion.WinMax.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxCredits", _editableLiveRegion.MaxCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxReserveCredits", _editableLiveRegion.MaxReserveCredits.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxBank", _editableLiveRegion.MaxBank.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "EscrowState", _editableLiveRegion.EscrowState.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "Rtp", _editableLiveRegion.Rtp.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "GameTime", _editableLiveRegion.GameTime.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "GiveChangeThreshold", _editableLiveRegion.GiveChangeThreshold.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxBankNote", _editableLiveRegion.MaxBankNote.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "AllowBank2Credit", _editableLiveRegion.AllowBank2Credit.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "ConvertToPlay", _editableLiveRegion.ConvertToPlay.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "FastTRansfer", _editableLiveRegion.FastTransfer.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "CycleSize", _editableLiveRegion.CycleSize.ToString(), _espRegionIni);
            NativeWinApi.WritePrivateProfileString("Settings", "MaxPlayerPoints", _editableLiveRegion.MaxPlayerPoints.ToString(), _espRegionIni);
            
            IniFileUtility.HashFile(_espRegionIni);
            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        public void LoadSettings()
        {
            string[] temp;
            var c = IniFileUtility.GetIniProfileSection(out temp, "General", _espRegionIni);
            _selected.Id = Convert.ToInt32(temp[1].Substring(14));
            _selected.Community = temp[2].Substring(7); //0
            _selected.VenueType = temp[3].Substring(10); //1
            
            string[] liveSettings;
            var settings = IniFileUtility.GetIniProfileSection(out liveSettings, "Settings", _espRegionIni);
            
            _editableLiveRegion.MaxStakeCredits = Convert.ToUInt32(liveSettings[0].Substring(16));
            _editableLiveRegion.MaxStakeBank = Convert.ToUInt32(liveSettings[1].Substring(13));
            _editableLiveRegion.StakeMask = Convert.ToUInt32(liveSettings[2].Substring(10));
            _editableLiveRegion.WinMax = Convert.ToUInt32(liveSettings[3].Substring(7));
            _editableLiveRegion.MaxCredits = Convert.ToUInt32(liveSettings[4].Substring(11));
            _editableLiveRegion.MaxReserveCredits = Convert.ToUInt32(liveSettings[5].Substring(18));
            _editableLiveRegion.MaxBank = Convert.ToUInt32(liveSettings[6].Substring(8));
            _editableLiveRegion.EscrowState = Convert.ToUInt32(liveSettings[7].Substring(12));
            _editableLiveRegion.Rtp = Convert.ToUInt32(liveSettings[8].Substring(4));
            _editableLiveRegion.GameTime = Convert.ToUInt32(liveSettings[9].Substring(9));
            _editableLiveRegion.GiveChangeThreshold = Convert.ToUInt32(liveSettings[10].Substring(20));
            _editableLiveRegion.MaxBankNote = Convert.ToUInt32(liveSettings[11].Substring(12));
            _editableLiveRegion.AllowBank2Credit = Convert.ToUInt32(liveSettings[12].Substring(17));
            _editableLiveRegion.ConvertToPlay = Convert.ToUInt32(liveSettings[13].Substring(14));
            _editableLiveRegion.FastTransfer = Convert.ToUInt32(liveSettings[14].Substring(13).TrimStart());
            _editableLiveRegion.CycleSize = Convert.ToUInt32(liveSettings[15].Substring(10));
            _editableLiveRegion.MaxPlayerPoints = Convert.ToUInt32(liveSettings[16].Substring(16));
            
            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        public void SetRegion()
        {
            if (_arcadeSelectedIndex == -1 && _marketSelectedIndex == -1)
                return;

            int id = 0;
            if (_arcadeSelectedIndex >= 0)
            {
                Selected.VenueType = "Arcade";
                Selected.Community = _arcadeRegions[_arcadeSelectedIndex];
                id = Array.IndexOf(_arcadeRegions, Selected.Community) + _streetMarketRegions.Length;
            }
            else
            {
                Selected.VenueType = "Street Market";
                Selected.Community = _streetMarketRegions[_marketSelectedIndex];
                id = Array.IndexOf(_streetMarketRegions, Selected.Community);
            }
            
            SpanishRegional sr = new SpanishRegional();
            BoLib.getDefaultRegionValues(id, ref sr);
             
            _editableLiveRegion = new SpanishRegionalModel(Selected.Community, sr);

            SaveChanges();
            LoadSettings();

            RaisePropertyChangedEvent("Selected");
            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        void DoIncrement(object settingsName)
        {
            string setting = settingsName as string;
            if (setting.Equals("GameTime"))
            {
                uint time = BoLib.getDefaultElement(Selected.Id, 9);
                if ((EditableLiveRegion.GameTime + 3) <= (time + 3))
                {
                    EditableLiveRegion.GameTime += 3;
                }
            }
            else if (setting.Equals("RTP") && _editableLiveRegion.Rtp < 10000)
                _editableLiveRegion.Rtp += 100;
            
            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        void DoDecrement(object settingsName)
        {
            string setting = settingsName as string;
            if (setting.Equals("GameTime"))
            {
                uint time = BoLib.getDefaultElement(Selected.Id, 9);
                if ((int)(time - EditableLiveRegion.GameTime) < 3)
                {
                    EditableLiveRegion.GameTime -= 3;
                    SaveChanges();
                    LoadSettings();
                }
            }
            else if (setting == "RTP")
            {
                if (_editableLiveRegion.Rtp < 10000)
                    _editableLiveRegion.Rtp += 100;
            }

            RaisePropertyChangedEvent("EditableLiveRegion");
        }
        
        void DoResetLiveToDefault()
        {
            int id = Selected.Id;
            SpanishRegional sr = new SpanishRegional();
            BoLib.getDefaultRegionValues(id, ref sr);
            _editableLiveRegion = new SpanishRegionalModel(Selected.Community, sr);
        
            SaveChanges();
            LoadSettings();

            RaisePropertyChangedEvent("EditableLiveRegion");
        }
    }
}
