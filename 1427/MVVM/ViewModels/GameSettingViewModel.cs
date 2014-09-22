using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{
    class GameSettingViewModel : ObservableObject
    {
        private readonly ObservableCollection<GameSettingModel> _gameSettings
            = new ObservableCollection<GameSettingModel>();
        private int _count = 0;
        private string _errorText = "";

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
        #endregion

        public GameSettingViewModel()
        {
        }
        
        public IEnumerable<GameSettingModel> Settings { get { return _gameSettings; } }
        public ICommand SetGameOptions
        {
            get { return new DelegateCommand(o => AddGame()); }
        }

        public void AddGame()
        {
            if (_gameSettings.Count > 0)
                _gameSettings.Clear();

            string manifest = @"D:\machine\ModelManifest.ini";
            
            string[] models;
            IniFileUtility.GetIniProfileSection(out models, "ModelNumber", manifest, true);

            string[] titles;
            IniFileUtility.GetIniProfileSection(out titles, "ModelTitle", manifest);

            string[] active;
            IniFileUtility.GetIniProfileSection(out active, "ModelActive", manifest);
            
            int activeCount = 0;
            foreach (var str in models)
            {
                if (str != "" || str != null)
                    activeCount++;
            }
            
            for (int i = 0; i < models.Length; i++)
            {
                GameSettingModel m = new GameSettingModel();
                m.Title = "Game Number " + (i + 1).ToString();
                m.Promo = true;
                _gameSettings.Add(m);
            }
        }
    }
}
