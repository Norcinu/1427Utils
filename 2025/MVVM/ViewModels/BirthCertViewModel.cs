using System.Collections.Generic;
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
            OperatorESP = new ObservableCollection<BirthCertModel>();
            ParseIni();
        }
        
        
        List<string> _theHelpMessages = new List<string>()
        {
            @"RTP: Game Payout 88-94%",
            @"MinPlayerPointsBet: Minimum Player Points Bet Value. 5. 10. 20. 40. 60. 80. 100. 200. 300. 400. 500",
            @"NvFloatControl: 0 - Never Disable. Cent Value = Nv Off Float < Cent Value",
            @"RecyclerChannel: 2 - 10€. 3 - 20€",
            @"OverRideRecycler: 0 - Enable note payout. 1 - Disable note payout",
            @"DumpSwitchFitted: 0 = No Hopper Dumpswitch. 1 = Hopper Dumpswitch Active",
            @"Handpay Threshold: Handpay above this value (¢)",
            @"Handpay Only: 0 - No Handpay. 1 - Handpay active",
            @"LH Divert Threshold: Value in € for Left Hopper Cashbox Divert",
            @"RH Divert Threshold: Value in € for Right Hopper Cashbox Divert",
            @"RefloatLH: Value to set left hopper float in ¢.",
            @"RefloatRH: Value to set right hopper float in ¢.",
            @"LProgressiveSys: 0 - No Local Progressive. 1 - Local Progressive"
        };
        
        public ObservableCollection<BirthCertModel> OperatorESP { get; private set; }
        public ObservableCollection<string> HelpValues { get; set; }

        public ICommand Parse { get { return new DelegateCommand(o => ParseIni()); } }
        void ParseIni()
        {
            ParseSection("Operator", OperatorESP);
            RaisePropertyChangedEvent("OperatorESP");
        }
        
        void ParseSection(string section, ObservableCollection<BirthCertModel> collection)
        {
            if (collection.Count == 0)
            {
                string[] config;
                IniFileUtility.GetIniProfileSection(out config, section, _filename);

                foreach (var str in config)
                {
                    if (str.StartsWith("#"))
                        break;
                    
                    var pair = str.Split("=".ToCharArray());
                    collection.Add(new BirthCertModel(pair[0], pair[1]));
                }
            }
        }
        
        public void SetHelpMessage(int index)
        {
            if (index > OperatorESP.Count)
                return;
            else
            {
                if (HelpValues == null)
                    HelpValues = new ObservableCollection<string>();

                if (HelpValues.Count > 0)
                    HelpValues.RemoveAll();

                var temp = _theHelpMessages[index];
                var arr = temp.Split(":.".ToCharArray());

                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0 && !string.IsNullOrEmpty(arr[i]))
                        HelpValues.Add(arr[i]);
                }

                RaisePropertyChangedEvent("HelpValues");
            }
        }

    }
}
