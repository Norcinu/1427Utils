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
        private readonly ObservableCollection<GameSettingModel> _gameSettings
            = new ObservableCollection<GameSettingModel>();
        private int _count = 0;
        private string _errorText = "";
        private CultureInfo _currentCulture;
        private NumberFormatInfo _nfi;
        public System.Globalization.NumberFormatInfo Nfi
        {
            get { return _nfi; }
            set { _nfi = value; }
        }
        
        private uint _numberOfGames = 0;
        private string _manifest = Properties.Resources.model_manifest;
        //private int _currentModelID = -1;
        #region properties
        public int ActiveCount
        { 
            get { return _count; }
            set
            {
                _count = value;
                this.RaisePropertyChangedEvent("Count");
            }
        }
        
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                this.RaisePropertyChangedEvent("ErrorText");
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

        string[] _fields = new string[18] 
        {
            "Number", "Title", "Active", "Stake1", "Stake2", "Stake3", "Stake4", "Stake5", 
            "Stake6", "Stake7", "Stake8", "Stake9", "Stake10", "StakeMask", "Promo", 
            "ModelDirectory", "Exe", "HashKey"
        };
        
        public object chk;
         
        public GameSettingViewModel()
        {
            SelectedIndex = -1;
            SelectionChanged = false;
            if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                IsBritishMachine = false;
            else
                IsBritishMachine = true;
            AddGame();
        }
        
        public void AddGame()
        {
            if (_gameSettings.Count > 0)//good service my jacksy imo
                _gameSettings.Clear();

            if (BoLib.getCountryCode() == BoLib.getUkCountryCodeB3() || BoLib.getCountryCode() == BoLib.getUkCountryCodeC())
                _currentCulture = new CultureInfo("en-GB");
            else
                _currentCulture = new CultureInfo("es-ES");

            _nfi = _currentCulture.NumberFormat;
            
            string[] modelNumber;
            IniFileUtility.GetIniProfileSection(out modelNumber, "Models", _manifest, true);
            _numberOfGames = Convert.ToUInt32(modelNumber[0]);
            
            for (int i = 0; i < _numberOfGames; i++)
            {
                string[] models;
                IniFileUtility.GetIniProfileSection(out models, "Model" + (i + 1), _manifest, true);
                
                GameSettingModel m  = new GameSettingModel();
                m.ModelNumber       = Convert.ToUInt32(models[0]);
                m.Title             = models[1].Trim(" \"".ToCharArray());
                m.Active            = (models[2] == "True") ? true : false;
                m.StakeOne          = Convert.ToInt32(models[3]);
                m.StakeTwo          = Convert.ToInt32(models[4]);
                m.StakeThree        = Convert.ToInt32(models[5]);
                m.StakeFour         = Convert.ToInt32(models[6]);
                /*m.StakeFive         = Convert.ToInt32(models[7]);
                m.StakeSix          = Convert.ToInt32(models[8]);*/
                m.StakeMask         = (Convert.ToUInt32(models[9]));
                m.Promo             = (models[10] == "True") ? true : false;
                m.ModelDirectory    = models[11];
                m.Exe               = models[12];
                m.HashKey           = models[13];
                _gameSettings.Add(m);
            }
        }
        
        public void SaveChanges()
        {
            if (_gameSettings.Count > 0)
            {
                int promoCount = 0;
                foreach (var g in _gameSettings)
                {
                    if (g.Promo && promoCount < 2)
                        ++promoCount;
                    else
                        g.Promo = false;
                }
                //
                if (promoCount == 0)
                {
                    Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                    _gameSettings[r.Next(_gameSettings.Count - 1)].Promo = true;
                }
                
                for (int i = 0; i < _numberOfGames; i++)
                {
                    var m = _gameSettings[i];

                    string temp = "Model" + (i + 1);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[0], m.ModelNumber.ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[1], m.Title, _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[2], m.Active.ToString(), _manifest);

                    if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
                    {
                        //---- Prices of play
                        NativeWinApi.WritePrivateProfileString(temp, _fields[3], (m.StakeOne * 100).ToString(), _manifest);
                        NativeWinApi.WritePrivateProfileString(temp, _fields[4], (m.StakeTwo * 100).ToString(), _manifest);
                        NativeWinApi.WritePrivateProfileString(temp, _fields[5], (m.StakeThree * 100).ToString(), _manifest);

                        NativeWinApi.WritePrivateProfileString(temp, _fields[6], (m.StakeFour * 100).ToString(), _manifest);
                    }
                    NativeWinApi.WritePrivateProfileString(temp, _fields[10], m.Promo.ToString(), _manifest);
                    
                    IniFileUtility.HashFile(_manifest);
                }
            }
        }
        
        public ICommand ToggleActive { get { return new DelegateCommand(o => DoToggleActive(chk)); } }
        public void DoToggleActive(object chk)
        {
            _gameSettings[SelectedIndex].Active = !_gameSettings[SelectedIndex].Active;
        }
        
        public ICommand ToggleStake { get { return new DelegateCommand(DoToggleStake); } }
        void DoToggleStake(object amount)
        {
            if (SelectedIndex >= 0)
            {
                string str = amount as string;
                if (str != "")
                {
                    int stake = Convert.ToInt32(amount);
                    if (stake == 25)
                        _gameSettings[SelectedIndex].StakeOne = (_gameSettings[SelectedIndex].StakeOne > 0) ? stake : 0;
                    else if (stake == 50)
                        _gameSettings[SelectedIndex].StakeTwo = (_gameSettings[SelectedIndex].StakeTwo > 0) ? stake : 0;
                    else if (stake == 100)
                        _gameSettings[SelectedIndex].StakeThree = (_gameSettings[SelectedIndex].StakeThree > 0) ? stake : 0;
                    else if (stake == 200)
                        _gameSettings[SelectedIndex].StakeFour = (_gameSettings[SelectedIndex].StakeFour > 0) ? stake : 0;
                }
            }
        }
    }
}

