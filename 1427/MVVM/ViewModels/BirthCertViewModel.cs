using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{
    class BirthCertViewModel : ObservableObject
    {
        readonly string _filename = Properties.Resources.birth_cert;
        ObservableCollection<BirthCertModel> _values = new ObservableCollection<BirthCertModel>();

        public BirthCertViewModel()
        {
            ParseIni();
        }

        public ObservableCollection<BirthCertModel> Values { get { return _values; } }
        //monday night is fucking rediculous.
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

        void WriteChanges()
        {

        }
    }
}
