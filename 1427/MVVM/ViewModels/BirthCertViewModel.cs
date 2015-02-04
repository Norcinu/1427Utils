using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using PDTUtils.Logic;
using System.Windows.Input;
using System.Windows.Controls;

namespace PDTUtils.MVVM.ViewModels
{
    class BirthCertViewModel : ObservableObject
    {
        readonly string _filename = Properties.Resources.birth_cert;
        ObservableCollection<BirthCertModel> _values = new ObservableCollection<BirthCertModel>();
        public BirthCertViewModel()
        {
            //ParseIni();
        }

        public ObservableCollection<BirthCertModel> Values { get { return _values; } }

        public ICommand Parse { get { return new DelegateCommand(o => ParseIni()); } }
        void ParseIni()
        {
            if (Values.Count == 0)
            {
                string[] config = null;
                IniFileUtility.GetIniProfileSection(out config, "Config", _filename);

                foreach (string str in config)
                {
                    string[] pair = new string[2];
                    pair = str.Split("=".ToCharArray());
                    Values.Add(new BirthCertModel(pair[0], pair[1]));
                }
            }
            RaisePropertyChangedEvent("Values");
        }
    }
}
