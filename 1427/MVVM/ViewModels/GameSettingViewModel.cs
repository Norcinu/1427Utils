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
        public int Count 
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
            AddGame();
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
                string[] models;
                IniFileUtility.GetIniProfileSection(out models, "Model" + (i + 1), _manifest, true);
                
                GameSettingModel m  = new GameSettingModel();
                m.ModelNumber       = Convert.ToUInt32(models[0]);
                m.Title             = models[1].Trim(" \"".ToCharArray());
                m.Active            = (models[2] == "True") ? true : false;
                /*m.StakeOne          = (Convert.ToDecimal(models[3]) / 100).ToString("C", _nfi);
                m.StakeTwo          = (Convert.ToDecimal(models[4]) / 100).ToString("C", _nfi);
                m.StakeThree        = (Convert.ToDecimal(models[5]) / 100).ToString("C", _nfi);
                m.StakeFour         = (Convert.ToDecimal(models[6]) / 100).ToString("C", _nfi);
                m.StakeFive         = (Convert.ToDecimal(models[7]) / 100).ToString("C", _nfi);
                m.StakeSix          = (Convert.ToDecimal(models[8]) / 100).ToString("C", _nfi);
                m.StakeSeven        = (Convert.ToDecimal(models[9]) / 100).ToString("C", _nfi);
                m.StakeEight        = (Convert.ToDecimal(models[10]) / 100).ToString("C", _nfi);
                m.StakeNine         = (Convert.ToDecimal(models[11]) / 100).ToString("C", _nfi);
                m.StakeTen          = (Convert.ToDecimal(models[12]) / 100).ToString("C", _nfi);*/
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
                for (int i = 0; i < _numberOfGames; i++)
                {
                    var m = _gameSettings[i];

                    string temp = "Model" + (i + 1);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[0], m.ModelNumber.ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[1], m.Title, _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[2], m.Active.ToString(), _manifest);
                    
                    //---- Prices of play
                    /*NativeWinApi.WritePrivateProfileString(temp, _fields[3], (m.StakeOne * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[4], (m.StakeTwo * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[5], (m.StakeThree * 100).ToString(), _manifest);
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[6], (m.StakeFour * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[7], (m.StakeFive * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], (m.StakeSix * 100).ToString(), _manifest);
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[9], (m.StakeSeven * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[10], (m.StakeEight * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[11], (m.StakeNine * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[12], (m.StakeTen * 100).ToString(), _manifest);*/
                    //---- End of prices of play
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[10], m.Promo.ToString(), _manifest);

                    IniFileUtility.HashFile(_manifest);
                }
            }
        }
        
        public ICommand ToggleActive { get { return new DelegateCommand(o => DoToggleActive(chk)); } }
        public void DoToggleActive(object chk)
        {
            
        }
        
        public ICommand ToggleStake { get { return new DelegateCommand(o => DoToggleStake()); } }
        void DoToggleStake()
        {
            System.Diagnostics.Debug.WriteLine("HELLO I'M JOHNNY CASH!");
        }
    }
}

