using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class GameSelectionViewModel : ObservableObject
    {
        ObservableCollection<GameSelectionModel> _models = new ObservableCollection<GameSelectionModel>();
        public ObservableCollection<GameSelectionModel> Models = new ObservableCollection<GameSelectionModel>();
        public int NumberOfModels { get; set; }
        public int Update { get; set; }
        public int NoActive { get; set; }

        public GameSelectionViewModel()
        {
            ReadManifest();
        }

        #region Commands
        public ICommand DoUpdateManifest { get { return new DelegateCommand(o => UpdateManifest()); } }
        public ICommand DoReadManifest { get { return new DelegateCommand(o => ReadManifest()); } }
        #endregion

        public void ReadManifest()
        {
            var file = (BoLib.getCountryCode() == 9) ? Properties.Resources.model_manifest_esp : Properties.Resources.model_manifest;
            
            string[] gen;
            IniFileUtility.GetIniProfileSection(out gen, "General", file);
            this.Update = Convert.ToInt32(gen[0].Substring(7));
            this.NoActive = Convert.ToInt32(gen[1].Substring(9));
                       
            string[] mod;
            IniFileUtility.GetIniProfileSection(out mod, "Models", file);
            this.NumberOfModels = Convert.ToInt32(mod[0].Substring(15));
        }
        
        public void UpdateManifest()
        {
            
        }
    }
}
