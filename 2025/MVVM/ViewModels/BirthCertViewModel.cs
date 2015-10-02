using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class BirthCertViewModel : ObservableObject
    {
        readonly string _filename = Properties.Resources.birth_cert;
        //bool _showHelp = false;
        //string _helpMessage = "";

        public BirthCertViewModel()
        {
            //Values = new ObservableCollection<BirthCertModel>();
            OperatorESP = new ObservableCollection<BirthCertModel>();
            ParseIni();
        }

        string[] _theHelpMessages = new string[13]
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

        /*string[] _theHelpMessages = new string[27]
        {
            @"Payout Type: 0 = Hopper. 1 = Printer. 2 = Combined.", 
            @"Number Of Hoppers: 0 = No Hopper. 1 = High Value Coin Only. 2 = High & Low Hopper",
            @"Hopper Type 1 (Large): 0 No Hopper. 1 = Universal. 2 = Snow. 3 = Compact", 
            @"Hopper Type 2 (Small): 0 No Hopper. 1 = Universal. 2 = Snow. 3 = Compact",
            @"Payout Coin 1(Large Hop): 0 = No coins. XXX = Value in Cents unit.", 
            @"Payout Coin 2(Small Hop): 0 = No coins. XXX = Value in Cents unit.",
            @"Printer Type: 0 = No Printer. 1 = TL60. 2 = TPT52. 3 = NV200_NT. 4 = Onyx\Gen2", 
            @"Printer Baud Rate: 115200 or 9600",
            @"Cpu Type: 0 = S410. 1 = S430", 
            @"Cabinet Type: 0 = V19. 1 = Slant Top. 2 = TS22. 3 = TS22_2015. 4 = BS100_2014",
            @"BNV Type: 0 = None. 1 = Auto Detect. 2 = NV9. 3 = MEI. 4 = JCM. 5 = NV11. 6 = NV200",
            @"Coin Validator: 0 = Eagle. 1 = Calibri",
            @"Note Validator Float Control: 0 = NV Never Off. XYZ = NV Off Float when < XYZ",
            @"Recycler Channel: 2 = 10 Euro. 3 = 20 Euro.",
            @"Card Reader: 0 = No Card Reader. 1 = Card Reader Active.",
            @"Screen Count: 1 = 1 Screen. 2 = 2 Screens.",
            @"Dump Switch Fitted for hopper dumps: 0 = Note Used. 1 = Require dump switch for emptying hoppers.",
            @"Hand Pay Threshold: Hand Pay above this level. Value is in Cent units. €100 = 10000",
            @"Large Hopper Divert Level: Divert to cashbox when reached this level. Value is in Cent units. €100 = 10000",
            @"Small Hopper Divert Level: Divert to cashbox when reached this level. Value is in Cent units. €100 = 10000",
            @"Volume Control: 0 = Volume set by remote server. 1 = Set via the terminal.",
            @"Hand Pay Only: = 0: Combined Payment. 1 = Hand Pay Only",
            @"OverrideRecycler: 0 = Include Note Payment. 1 = Disable Note Payment",
            @"TiToEnabled: 0: Disabled. 1 = Enabled.",
            @"CommunityMember: 0 = No Community Link. 1 = Community active.",
            @"CommunityMaster: 0 = Not Master. 1 Act as Master.",
            @"CommunityIP: IP Address. E.g. 192.168.1.1"
        };*/

        //public ObservableCollection<BirthCertModel> Values { get; private set; }
        public ObservableCollection<BirthCertModel> OperatorESP { get; private set; }
        public ObservableCollection<string> HelpValues { get; set; }
      /*  public bool ShowHelp
        {
            get { return _showHelp; }
            set
            {
                _showHelp = true;
                RaisePropertyChangedEvent("ShowHelp");
            }
        }
        
        public string HelpMessage
        {
            get { return _helpMessage; }
            set
            {
                _helpMessage = value;
                RaisePropertyChangedEvent("HelpMessage");
            }
        }*/

        public ICommand Parse { get { return new DelegateCommand(o => ParseIni()); } }
        void ParseIni()
        {
           /* if (Values.Count == 0)
            {
                string[] config;
                IniFileUtility.GetIniProfileSection(out config, "Config", _filename);
                
                foreach (var str in config)
                {
                    var pair = str.Split("=".ToCharArray());
                    Values.Add(new BirthCertModel(pair[0], pair[1]));
                }
            }

            if (OperatorESP.Count == 0)
            {
                string[] config;
                IniFileUtility.GetIniProfileSection(out config, "Config", _filename);

                foreach (var str in config)
                {
                    var pair = str.Split("=".ToCharArray());
                    Values.Add(new BirthCertModel(pair[0], pair[1]));
                }
            }*/
           // ParseSection("Config", Values);
            ParseSection("Operator", OperatorESP);

           // RaisePropertyChangedEvent("Values");
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
                    /*if (pair[0] != "Minimum Payout Value" &&
                        pair[0] != "Minimum Bet" &&
                        pair[0] != "StakeMatch" &&
                        pair[0] != "Terminal Closed TIME" &&
                        pair[0] != "Terminal Open TIME")*/
                    {
                        collection.Add(new BirthCertModel(pair[0], pair[1]));
                    }
                }
            }
        }
        
        /*public void SetHelpMessage(int index)
        {
            HelpMessage = _theHelpMessages[index];
        }*/

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
                /*HelpValues.Add()*/

                RaisePropertyChangedEvent("HelpValues");
            }
        }

    }
}
