using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class GameSettingViewModel : ObservableObject
    {
        readonly ObservableCollection<GameSettingModel> _gameSettings = new ObservableCollection<GameSettingModel>();
        readonly int[] _ukStakeValues = new int[4] { 25, 50, 100, 200 };
        readonly string _manifest = (BoLib.getCountryCode() == 9) ? Properties.Resources.model_manifest_esp 
                                                                  : Properties.Resources.model_manifest;

        int _count = 0;
        int _currentFirstSel = -1;
        int _currentSecondSel = -1;
        uint _numberOfGames = 0;
        string _errorText = "";
        CultureInfo _currentCulture;       
        
        public NumberFormatInfo Nfi { get; set; }

        #region properties
        public int ActiveCount
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChangedEvent("Count");
            }
        }
        
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                RaisePropertyChangedEvent("ErrorText");
            }
        }
        
        public CultureInfo SettingsCulture { get { return _currentCulture; } }
        public IEnumerable<GameSettingModel> Settings { get { return _gameSettings; } }
        int _selectedIndex;
        public bool SelectionChanged { get; set; }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (_selectedIndex >= 0)
                {
                    SelectedModelNumber = _gameSettings[_selectedIndex].ModelNumber.ToString();
                    SelectedGameName = _gameSettings[_selectedIndex].Title;
                    
                    RaisePropertyChangedEvent("IsActiveGame");
                    RaisePropertyChangedEvent("IsPromoGame");
                    RaisePropertyChangedEvent("StakeOne");
                    RaisePropertyChangedEvent("StakeTwo");
                    RaisePropertyChangedEvent("StakeThree");
                    RaisePropertyChangedEvent("StakeFour");
                    RaisePropertyChangedEvent("IsFirstPromo");
                    RaisePropertyChangedEvent("IsSecondPromo");
                    RaisePropertyChangedEvent("FirstPromo");
                    RaisePropertyChangedEvent("SecondPromo");
                }
            }
        }

        public string SelectedGameName
        {
            get 
            {
                if (_selectedIndex == -1)
                    return "";
                return _gameSettings[SelectedIndex].Title; 
            }
            set
            {
                _gameSettings[SelectedIndex].Title = value;
                RaisePropertyChangedEvent("SelectedGameName");
            }
        }
        
        public string SelectedModelNumber
        {
            get 
            {
                if (SelectedIndex == -1)
                    return "";
                return _gameSettings[SelectedIndex].ModelNumber.ToString();
            }
            set
            {
                _gameSettings[SelectedIndex].ModelNumber = Convert.ToUInt32(value);
                RaisePropertyChangedEvent("SelectedModelNumber");
            }
        }
        
        int _numberOfPromos = 0;
        public int NumberOfPromos
        {
            get { return _numberOfPromos; }
            set
            {
                _numberOfPromos = value;
                RaisePropertyChangedEvent("NumberOfPromos");
            }
        }
        
        bool _isBritish = false;
        public bool IsBritishMachine
        {
            get { return _isBritish; }
            set
            {
                _isBritish = value;
                RaisePropertyChangedEvent("IsBritishMachine");
            }
        }
        
        public bool IsActiveGame
        {
            get
            {
                if (SelectedIndex >= 0)
                    return (bool)_gameSettings[SelectedIndex].Active;
                else
                    return false;
            }
            set
            {
                if (SelectedIndex >= 0)
                {
                    _gameSettings[SelectedIndex].Active = value;
                    RaisePropertyChangedEvent("IsActiveGame");
                }
            }
        }

        public bool IsPromoGame
        {
            get
            {
                if (SelectedIndex >= 0)
                    return _gameSettings[SelectedIndex].Promo;
                else
                    return false;
            }
            set
            {
                if (SelectedIndex >= 0)
                {
                    _gameSettings[SelectedIndex].Promo = value;
                    RaisePropertyChangedEvent("IsPromoGame");
                }
            }
        }

        public bool StakeOne
        {
            get
            {
                if (SelectedIndex >= 0)
                {
                    if (_gameSettings[SelectedIndex].StakeOne > 0 && _gameSettings[SelectedIndex] != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            set
            {
                if (value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeOne = _ukStakeValues[0];
                else if (!value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeOne = 0;
            }
        }

        public bool StakeTwo
        {
            get
            {
                if (SelectedIndex >= 0)
                {
                    if (_gameSettings[SelectedIndex].StakeTwo > 0 && _gameSettings[SelectedIndex] != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            set
            {
                if (value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeTwo = _ukStakeValues[1];
                else if (!value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeTwo = 0;
            }
        }

        public bool StakeThree
        {
            get
            {
                if (SelectedIndex >= 0)
                {
                    if (_gameSettings[SelectedIndex].StakeThree > 0 && _gameSettings[SelectedIndex] != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            set
            {
                if (value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeThree = _ukStakeValues[2];
                else if (!value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeThree = 0;
            }
        }

        public bool StakeFour
        {
            get
            {
                if (SelectedIndex >= 0)
                {
                    if (_gameSettings[SelectedIndex].StakeFour > 0 && _gameSettings[SelectedIndex] != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            set
            {
                if (value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeFour = _ukStakeValues[3];
                else if (!value && SelectedIndex >= 0)
                    _gameSettings[SelectedIndex].StakeFour = 0;
            }
        }

        public bool IsFirstPromo
        {
            get { return _gameSettings[SelectedIndex].IsFirstPromo; }
            set
            {
                if (_currentFirstSel != SelectedIndex)
                {
                    foreach (var gs in _gameSettings)
                    {
                        gs.Promo = false;
                        gs.IsFirstPromo = false;
                    }
    
                    _currentFirstSel = SelectedIndex;
                }
                _gameSettings[SelectedIndex].IsFirstPromo = value;
                RaisePropertyChangedEvent("IsFirstPromo");
            }
        }
        
        public bool IsSecondPromo
        {
            get { return _gameSettings[SelectedIndex].IsSecondPromo; }
            set
            {
                if (_currentSecondSel != SelectedIndex)
                {
                    _gameSettings[_currentFirstSel].IsSecondPromo = false;
                    _gameSettings[_currentSecondSel].Promo = false;
                    _currentSecondSel = SelectedIndex;
                }
                _gameSettings[SelectedIndex].IsSecondPromo = value;
                RaisePropertyChangedEvent("IsSecondPromo");
            }
        }

        public string FirstPromo
        {
            get;
            /*{
                if (_currentFirstSel != -1)
                    return _gameSettings[_currentFirstSel].Title;
                else
                    return "NOT SELECTED";
            }*/
            set;
        }

        public string SecondPromo
        {
            get;
            set;
            /*{
                if (_currentSecondSel != -1)
                    return _gameSettings[_currentSecondSel].Title;
                else
                    return "NOT SELECTED 2";
            }*/
        }

        #endregion

        #region Commands
        public ICommand SetGameOptions
        {
            get { return new DelegateCommand(o => AddGame()); }
        }

        public ICommand SaveGameOptions
        {
            get { return new DelegateCommand(o => SaveChanges()); }
        }
        #endregion
        
        string[] _fields = new string[12]
        {
            "Number", "Title", "Active", "Stake1", "Stake2", "Stake3", "Stake4", 
            "StakeMask", "Promo", "ModelDirectory", "Exe", "HashKey"
        };
        
        public object Chk;
        
        public GameSettingViewModel()
        {
            SelectionChanged = false;
            IsBritishMachine = BoLib.getCountryCode() != BoLib.getSpainCountryCode();
            AddGame();
            
            RaisePropertyChangedEvent("FirstPromo");
            RaisePropertyChangedEvent("SecondPromo");

            SelectedIndex = _gameSettings.Count > 0 ? 0 : -1;
        }

        public void AddGame()
        {
            if (!System.IO.File.Exists(_manifest))
            {
                var msg = new WpfMessageBoxService();
                msg.ShowMessage("Cannot find ModelManifest.ini", "ERROR");
                return;
            }

            if (_gameSettings.Count > 0)
                _gameSettings.Clear();

            _currentCulture = BoLib.getCountryCode() == BoLib.getUkCountryCodeB3() ||
                              BoLib.getCountryCode() == BoLib.getUkCountryCodeC()
                ? new CultureInfo("en-GB")
                : new CultureInfo("es-ES");

            Nfi = _currentCulture.NumberFormat;

            string[] modelNumber;
            IniFileUtility.GetIniProfileSection(out modelNumber, "Models", _manifest, true);
            _numberOfGames = Convert.ToUInt32(modelNumber[0]);

            for (var i = 0; i < _numberOfGames; i++)
            {
                string[] model;
                IniFileUtility.GetIniProfileSection(out model, "Model" + (i + 1), _manifest, true);
                var sb = new System.Text.StringBuilder(8); //dis be going wrong yo.
                NativeWinApi.GetPrivateProfileString("Model" + (i + 1), "Promo", "", sb, 8, _manifest);
                var isPromo = sb.ToString();
                //!!!! remove combo box and using some sort of buttoning system !!!!
                var m = new GameSettingModel
                {
                    ModelNumber = Convert.ToUInt32(model[0]),
                    Title = model[1].Trim(" \"".ToCharArray()),
                    Active = (model[2] == "1"),
                    StakeOne = Convert.ToInt32(model[3]),
                    StakeTwo = Convert.ToInt32(model[4]),
                    StakeThree = Convert.ToInt32(model[5]),
                    StakeFour = Convert.ToInt32(model[6]),
                    StakeMask = Convert.ToUInt32(model[9]),
                    ModelDirectory = model[11],
                    Exe = model[12],
                    HashKey = model[13]
                };
                
                _gameSettings.Add(m);
                
                switch (isPromo)
                {
                    case "100":
                        m.Promo = true;
                        m.IsFirstPromo = true;
                        FirstPromo = m.Title;
                        break;
                    case "200":
                        m.Promo = true;
                        m.IsSecondPromo = true;
                        if (m.Active) SecondPromo = m.Title;
                        break;
                    default:
                        m.Promo = false;
                        break;
                }
            }
        }
        
        public void SaveChanges()
        {
            if (_gameSettings.Count <= 0) return;
            var promoCount = 0;
            foreach (var g in _gameSettings)
            {
                if (g.Promo && promoCount < 2)
                    ++promoCount;
                else if (promoCount >= 2)
                    g.Promo = false;
            }
                 
            if (promoCount == 0)
            {
                var r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                _gameSettings[r.Next(_gameSettings.Count)].Promo = true;
            }
            
            bool isFirstSet = false;
            bool isSecondSet = false;
            uint activeCount = _numberOfGames;

            for (var i = 0; i < _numberOfGames; i++)
            {
                var m = _gameSettings[i];
                
                var temp = "Model" + (i + 1);
                var active = 0;

                if (m.Active)
                    active = 1;
                else
                {
                    active = 0;
                    activeCount--;
                }
             
                NativeWinApi.WritePrivateProfileString(temp, _fields[0], m.ModelNumber.ToString(), _manifest);
                NativeWinApi.WritePrivateProfileString(temp, _fields[1], m.Title, _manifest);
                NativeWinApi.WritePrivateProfileString(temp, _fields[2], active.ToString(), _manifest);
                
                if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
                {
                    NativeWinApi.WritePrivateProfileString(temp, _fields[3], m.StakeOne.ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[4], m.StakeTwo.ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[5], m.StakeThree.ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[6], m.StakeFour.ToString(), _manifest);
                }

                if (FirstPromo == m.Title && m.Active)
                {
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], "100", _manifest);
                    isFirstSet = true;
                }
                else if (SecondPromo == m.Title && m.Active)
                {
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], "200", _manifest);
                    isSecondSet = true;
                }
                else
                {
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], "0", _manifest);
                }
            }

            if (!isFirstSet)
            {
                for (int i = 0; i < _gameSettings.Count; i++)
                {
                    if (_gameSettings[i].Active)
                    {
                        NativeWinApi.WritePrivateProfileString("Model" + (i + 1), "Promo", "100", _manifest);
                        break;
                    }
                }
            }

            if (!isSecondSet)
            {
                for (int i = 0; i < _gameSettings.Count; i++)
                {
                    if (_gameSettings[i].Active && !_gameSettings[i].Promo)
                    {
                        NativeWinApi.WritePrivateProfileString("Model" + (i + 1), "Promo", "200", _manifest);
                        break;
                    }
                }
            }
            
            NativeWinApi.WritePrivateProfileString("General", "Update", "1", _manifest);
            NativeWinApi.WritePrivateProfileString("General", "NoActive", activeCount.ToString(), _manifest);

            IniFileUtility.HashFile(_manifest);

            isFirstSet = false;
            isSecondSet = false;

            GlobalConfig.RebootRequired = true;
        }
        
        public ICommand ToggleActive { get { return new DelegateCommand(o => DoToggleActive(Chk)); } }
        public void DoToggleActive(object chk)
        {
            _gameSettings[SelectedIndex].Active = !_gameSettings[SelectedIndex].Active;
        }
        
        public ICommand ToggleStake { get { return new DelegateCommand(DoToggleStake); } }
        void DoToggleStake(object amount)
        {
            var str = amount as string;
            if (SelectedIndex < 0 || str == "") return;
            
            var stake = Convert.ToInt32(amount);
            switch (stake)
            {
                case 25:
                    _gameSettings[SelectedIndex].StakeOne = (_gameSettings[SelectedIndex].StakeOne > 0) ? stake : 0;
                    break;
                case 50:
                    _gameSettings[SelectedIndex].StakeTwo = (_gameSettings[SelectedIndex].StakeTwo > 0) ? stake : 0;
                    break;
                case 100:
                    _gameSettings[SelectedIndex].StakeThree = (_gameSettings[SelectedIndex].StakeThree > 0) ? stake : 0;
                    break;
                case 200:
                    _gameSettings[SelectedIndex].StakeFour = (_gameSettings[SelectedIndex].StakeFour > 0) ? stake : 0;
                    break;
            }
        }
        
        public void UpdatePromoSelection(GameSettingModel first, GameSettingModel second)
        {
            FirstPromo = first.Title;
            if (first.Title == SecondPromo) SecondPromo = "";
            if (second != null && second.Title != "")
            {
                SecondPromo = second.Title;
                if (second.Title == FirstPromo) SecondPromo = "";
            }

            foreach (var gsm in _gameSettings)
            {
                if (gsm.Title != first.Title)// && gsm.Title != second.Title)
                {
                    gsm.Promo = false;
                }
            }
            
            RaisePropertyChangedEvent("FirstPromo");
            RaisePropertyChangedEvent("SecondPromo");
        }
    }
}

