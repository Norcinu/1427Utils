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
        private uint _numberOfGames = 0;
        private string _manifest = Properties.Resources.model_manifest;
        private int _currentModelID = -1;
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

        #region commands
        public ICommand SetGameOptions
        {
            get { return new DelegateCommand(o => AddGame()); }
        }

        public ICommand SaveGameOptions
        {
            get { return new DelegateCommand(o => SaveChanges()); }
        }
        #endregion

        string[] _fields = new string[17] {"Number", "Title", "Active", "Stake1", "Stake2", "Stake3",
            "Stake4", "Stake5", "Stake6", "Stake7", "Stake8", "Stake9", "Stake10", "Promo", "ModelDirectory",
            "Exe", "HashKey"
        };
        
        public object chk;
        
        public GameSettingViewModel()
        {
           
        }
        
        public void AddGame()
        {
            if (_gameSettings.Count > 0)
                _gameSettings.Clear();
            
            _currentCulture = CultureInfo.CurrentCulture;
            
            string[] modelNumber;
            IniFileUtility.GetIniProfileSection(out modelNumber, "Models", _manifest, true);
            _numberOfGames = Convert.ToUInt32(modelNumber[0]);
            
            for (int i = 0; i < _numberOfGames; i++)
            {
                string[] models;
                IniFileUtility.GetIniProfileSection(out models, "Model" + (i + 1), _manifest, true);
                
                GameSettingModel m = new GameSettingModel();
                m.ModelNumber = Convert.ToUInt32(models[0]);
                m.Title = models[1];
                m.Active = (models[2] == "True") ? true : false;
                m.StakeOne = Convert.ToDecimal(models[3]) / 100;
                m.StakeTwo = Convert.ToDecimal(models[4]) / 100;
                m.StakeThree = Convert.ToDecimal(models[5]) / 100;
                m.StakeFour = Convert.ToDecimal(models[6]) / 100;
                m.StakeFive = Convert.ToDecimal(models[7]) / 100;
                m.StakeSix = Convert.ToDecimal(models[8]) / 100;
                m.StakeSeven = Convert.ToDecimal(models[9]) / 100;
                m.StakeEight = Convert.ToDecimal(models[10]) / 100;
                m.StakeNine = Convert.ToDecimal(models[11]) / 100;
                m.StakeTen = Convert.ToDecimal(models[12]) / 100;
                m.Promo = (models[13] == "True") ? true : false;
                m.ModelDirectory = models[14];
                m.Exe = models[15];
                m.HashKey = models[16];
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
                    NativeWinApi.WritePrivateProfileString(temp, _fields[3], (m.StakeOne * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[4], (m.StakeTwo * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[5], (m.StakeThree * 100).ToString(), _manifest);

                    NativeWinApi.WritePrivateProfileString(temp, _fields[6], (m.StakeFour * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[7], (m.StakeFive * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[8], (m.StakeSix * 100).ToString(), _manifest);
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[9], (m.StakeSeven * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[10], (m.StakeEight * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[11], (m.StakeNine * 100).ToString(), _manifest);
                    NativeWinApi.WritePrivateProfileString(temp, _fields[12], (m.StakeTen * 100).ToString(), _manifest);
                    //---- End of prices of play
                    
                    NativeWinApi.WritePrivateProfileString(temp, _fields[13], m.Promo.ToString(), _manifest);
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

