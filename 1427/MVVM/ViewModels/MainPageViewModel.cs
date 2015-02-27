using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.Native;


namespace PDTUtils.MVVM.ViewModels
{
    class MainPageViewModel : ObservableObject
    {
        public bool IsEnabled { get; set; }
        public bool IsErrorSet
        {
            get
            {
                if (BoLib.getError() > 0)
                    return true;
                else 
                    return false;
            }
        }
        
        public bool HandPayActive
        {
            get { return _handPayActive; }
            set
            {
                _handPayActive = value;
                if (_handPayActive && _addCreditsActive)
                    AddCreditsActive = false;
                RaisePropertyChangedEvent("HandPayActive");
#if DEBUG
                Debug.WriteLine("HandPayActive", _handPayActive.ToString());
#endif
            }
        }
        
        public bool AddCreditsActive
        {
            get { return _addCreditsActive; }
            set 
            { 
                _addCreditsActive = value;
                if (_addCreditsActive && _handPayActive)
                    HandPayActive = false;
                RaisePropertyChangedEvent("AddCreditsActive");
#if DEBUG
                Debug.WriteLine("AddCreditsActive", _addCreditsActive.ToString());
#endif
            }
        }
        
        public int Credits { get; set; }
        public int Bank { get; set; }
        public int Reserve { get; set; }
        public int Pennies { get; set; }

        public bool CanPayFifty
        {
            get { return _canPayFifty; }
            set
            {
                _canPayFifty = value;
                RaisePropertyChangedEvent("CanPayFifty");
            }
        }
        
        public Decimal TotalCredits 
        { 
            get 
            {
                return _totalCredits; 
            }
            set 
            { 
                _totalCredits = Credits + Bank + Reserve;
                RaisePropertyChangedEvent("TotalCredits");
            } 
        }
        public string ErrorMessage { get; set; }
        
        bool _handPayActive;
        bool _addCreditsActive;
        bool _canPayFifty;

        string _caption = "Warning";
        string _message = "Please Open the terminal door and try again.";
        readonly  PDTUtils.MVVM.WpfMessageBoxService _msgBoxService = new PDTUtils.MVVM.WpfMessageBoxService();
        
 
        System.Timers.Timer RefillTimer;
        Decimal _totalCredits = 0;

        public MainPageViewModel()
        {
            IsEnabled = true;
            HandPayActive = false;
            ErrorMessage = "";
            Credits = 0;
            Bank = 0;
            Reserve = 0;
            Pennies = 2000;
            NotRefilling = true;
            
            GetErrorMessage();
            GetCreditLevel();
            GetBankLevel();
            GetReserveLevel();
            GetMaxNoteValue();
        }

        public bool DoorOpen
        {
            get
            {
                return false;
                //return (bool)BoLib.getDoorStatus();
            }
        }
        
        public bool NotRefilling { get; set; }
        
        public ICommand GetCredit
        {
            get { return new DelegateCommand(o => GetCreditLevel()); }
        }
        
        void GetCreditLevel()
        {
            Credits = BoLib.getCredit();
            RaisePropertyChangedEvent("Credits");
        }

        public ICommand GetBank
        {
            get { return new DelegateCommand(o => GetBankLevel()); }
        }
        
        void GetBankLevel()
        {
            Bank = BoLib.getBank();
            RaisePropertyChangedEvent("Bank");
        }

        public ICommand GetReserve
        {
            get { return new DelegateCommand(o => GetReserveLevel()); }
        }

        void GetReserveLevel()
        {
            Reserve = (int)BoLib.getReserveCredits();
            RaisePropertyChangedEvent("Reserve");
        }

        public ICommand ClearCredits
        {
            get { return new DelegateCommand(o => ClearCreditLevel()); }
        }

        void ClearCreditLevel()
        {
            BoLib.clearBankAndCredit();
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            RaisePropertyChangedEvent("Credits");
            RaisePropertyChangedEvent("Bank");
        }
        
        public ICommand TransferBank
        {
            get { return new DelegateCommand(o => TransferBankCredits()); }
        }
        
        void TransferBankCredits()
        {
            BoLib.transferBankToCredit();
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            TotalCredits = Bank + Credits;
            RaisePropertyChangedEvent("Credits");
            RaisePropertyChangedEvent("Bank");
        }

        void GetMaxNoteValue()
        {
            var maxValue = BoLib.getLiveElement(11); //ESP_MAX_BANKNOTE_VALUE 
            CanPayFifty = maxValue > 2000;
        }

        public ICommand AddCredits
        {
            get { return new DelegateCommand(o => AddCreditsActive = !AddCreditsActive); }
        }
        
        public ICommand GetError
        {
            get { return new DelegateCommand(o => GetErrorMessage()); }
        }
        
        void GetErrorMessage()
        {
            var errorCode = BoLib.getError();
            if (errorCode > 0)
            {
                var last = BoLib.getErrorMessage("", BoLib.getError());
                ErrorMessage = "Current Error : " + "[" + errorCode + "] " + last + "\nOpen Door and Press Button To Clear Error";
            }
            else
                ErrorMessage = "No Current Error";

            RaisePropertyChangedEvent("ErrorMessage");
        }
        
        public ICommand ShowMessageBox
        {
            get { return new DelegateCommand(o => _msgBoxService.ShowMessage(_message, _caption)); }
        }
        
        public ICommand ClearCurrentError
        {
            get { return new DelegateCommand(o => ClearError()); }
        }
        
        void ClearError()
        {
            if (BoLib.getDoorStatus() > 0)
            {
                if (BoLib.clearError() != 0) return;
                ErrorMessage = "Error Cleared. Please Continue.";
                RaisePropertyChangedEvent("ErrorMessage");
            }
            else
            {
                ShowMessageBox.Execute(null);
            }
        }
        
        public ICommand SetHandPay
        {
            get { return new DelegateCommand(o => HandPayActive = !HandPayActive); }
        }

        public ICommand HandPay
        {
            get { return new DelegateCommand(o => DoHandPay()); }
        }
        
        void DoHandPay()
        {
          //  RaisePropertyChangedEvent("TotalCredits"); // credit value not updating if add credit from 0 and then handpay
            string oldCaption = _caption;
            string oldMsg = _message;
            
            int total = Bank + Credits;
            
            if (total > 0)
            {
                if (!BoLib.performHandPay())
                {
                    _caption = "WARNING";
                    _message = "SET HANDPAY THRESHOLD";
                    ShowMessageBox.Execute(null);
                    _caption = oldCaption;
                    _message = oldMsg;
                    return;
                }
                
                BoLib.clearBankAndCredit();
                WriteToHandPayLog(total);
                Credits = BoLib.getCredit();
                Bank = BoLib.getBank();
                Reserve = (int)BoLib.getReserveCredits();
                TotalCredits = 0;
                
                RaisePropertyChangedEvent("Credits");
                RaisePropertyChangedEvent("Bank");
                RaisePropertyChangedEvent("Reserve");
            }
            else
            {
                _caption = "WARNING";
                _message = "NO CREDITS FOR HAND PAY";
                ShowMessageBox.Execute(null);
                _caption = oldCaption;
                _message = oldMsg;
            }
        }
        
        void WriteToHandPayLog(int total)
        {
            string filename = Properties.Resources.hand_pay_log;

            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    DateTime now = DateTime.Now;
                    int amount = total;
                    sw.Write(now.ToShortDateString() + " ");
                    sw.Write(now.ToLongTimeString() + " ");
                    sw.Write((Convert.ToDecimal(amount) / 100).ToString("C") + "\r\n");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    DateTime now = DateTime.Now;
                    int amount = total;
                    sw.Write(now.ToShortDateString() + " ");
                    sw.Write(now.ToLongTimeString() + "\t\t");
                    sw.Write((Convert.ToDecimal(amount) / 100).ToString("C") + "\r\n");
                }
            }
        }
        
        public ICommand AddCreditSpecific
        {
            get { return new DelegateCommand(AddDenomButton); }
        }
        
        void AddDenomButton(object button)
        {
            var b = button as Button;
            string str = b.Content as string;
            
            if (str[0] == '£' || str[0] == '€')
            {
                Pennies = Convert.ToInt32(str.Substring(1));
                Pennies *= 100;
            }
            else
                Pennies = Convert.ToInt32(str.Substring(0, str.Length - 1));

            BoLib.setUtilsAdd2CreditValue((uint)Pennies);
            BoLib.setRequestUtilsAdd2Credit();
            System.Threading.Thread.Sleep(250);
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            Reserve = (int)BoLib.getReserveCredits();
            TotalCredits = Bank + Credits + Reserve;
            RaisePropertyChangedEvent("Credits");
            RaisePropertyChangedEvent("Bank");
            RaisePropertyChangedEvent("Reserve");
        }
        
        public ICommand CancelHandPay
        {
            get { return new DelegateCommand(o => DoCancelHandPay()); }
        }
        
        void DoCancelHandPay()
        {
            HandPayActive = false;
            BoLib.cancelHandPay();
        }

        public ICommand ToggleIsEnabled
        {
            get { return new DelegateCommand(o => IsEnabled = !IsEnabled); }
        }

        public ICommand RefillHopper { get { return new DelegateCommand(o => DoRefillHopper()); } }
        void DoRefillHopper()
        {
        }

        public ICommand EndRefillCommand { get { return new DelegateCommand(o => DoEndRefill()); } }
        void DoEndRefill()
        {
            if (RefillTimer == null)
            {
                RefillTimer = new System.Timers.Timer(100);
                RefillTimer.Enabled = true;
            }
            
            NotRefilling = false;
            RefillTimer.Elapsed += (sender, e) =>
            {
                RefillTimer.Enabled = false;
            };
        }

    }
}
