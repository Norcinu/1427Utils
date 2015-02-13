﻿using System;
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
        private readonly int[] _ukStakeValues = new int[4] { 25, 50, 100, 200 };        
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
                    RaisePropertyChangedEvent("IsActiveGame");
                    RaisePropertyChangedEvent("IsPromoGame");
                    RaisePropertyChangedEvent("StakeOne");
                    RaisePropertyChangedEvent("StakeTwo");
                    RaisePropertyChangedEvent("StakeThree");
                    RaisePropertyChangedEvent("StakeFour");
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
            "Number", "Title", "Active", "Stake1", "Stake2", "Stake3", "Stake4", /*"Stake5", 
            "Stake6", "Stake7", "Stake8", "Stake9", "Stake10",*/ "StakeMask", "Promo", 
            "ModelDirectory", "Exe", "HashKey"
        };
        
        public object chk;
         
        public GameSettingViewModel()
        {
            SelectionChanged = false;
            if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                IsBritishMachine = false;
            else
                IsBritishMachine = true;
            AddGame();

            if (_gameSettings.Count > 0)
                SelectedIndex = 0;
            else
                SelectedIndex = -1;
        }
        
        public void AddGame()
        {
            if (_gameSettings.Count > 0)
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
                string[] model;
                IniFileUtility.GetIniProfileSection(out model, "Model" + (i + 1), _manifest, true);
                System.Text.StringBuilder sb = new System.Text.StringBuilder(8);
                NativeWinApi.GetPrivateProfileString("Game" + (i + 1), "Promo", "", sb, 8, @Properties.Resources.machine_ini);
                string isPromo = sb.ToString();

                GameSettingModel m  = new GameSettingModel();
                m.ModelNumber       = Convert.ToUInt32(model[0]);
                m.Title             = model[1].Trim(" \"".ToCharArray());
                m.Active            = (model[2] == "1") ? true : false;
                m.StakeOne          = Convert.ToInt32(model[3]);
                m.StakeTwo          = Convert.ToInt32(model[4]);
                m.StakeThree        = Convert.ToInt32(model[5]);
                m.StakeFour         = Convert.ToInt32(model[6]);                
                m.StakeMask         = (Convert.ToUInt32(model[9]));
                m.Promo             = (isPromo == "100" || isPromo == "200") ? true : false;
                m.ModelDirectory    = model[11];
                m.Exe               = model[12];
                m.HashKey           = model[13];
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
                   else if (promoCount >= 2)
                        g.Promo = false;
                }
                
                if (promoCount == 0)
                {
                    Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                    _gameSettings[r.Next(_gameSettings.Count)].Promo = true;
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
                        NativeWinApi.WritePrivateProfileString(temp, _fields[3], m.StakeOne.ToString(), _manifest);
                        NativeWinApi.WritePrivateProfileString(temp, _fields[4], m.StakeTwo.ToString(), _manifest);
                        NativeWinApi.WritePrivateProfileString(temp, _fields[5], m.StakeThree.ToString(), _manifest);

                        NativeWinApi.WritePrivateProfileString(temp, _fields[6], m.StakeFour.ToString(), _manifest);
                    }
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], m.Promo.ToString(), _manifest);
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

