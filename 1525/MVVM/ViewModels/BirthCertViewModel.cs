using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{
    class BirthCertViewModel : ObservableObject
    {
        readonly string _filename = Properties.Resources.birth_cert;

        public BirthCertViewModel()
        {
            Values = new ObservableCollection<BirthCertModel>();
            ParseIni();
        }

        public ObservableCollection<BirthCertModel> Values { get; private set; }
        
        public ICommand Parse { get { return new DelegateCommand(o => ParseIni()); } }
        void ParseIni()
        {
            if (Values.Count == 0)
            {
                string[] config;
                IniFileUtility.GetIniProfileSection(out config, "Config", _filename);
                
                foreach (var str in config)
                {
                    var pair = str.Split("=".ToCharArray());
                    Values.Add(new BirthCertModel(pair[0], pair[1]));
                }
            }
            RaisePropertyChangedEvent("Values");
        }

        /* TODO!!! */
        void WriteChanges()
        {

        }
    }
}
