using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.Native;

using Timer = System.Timers.Timer;

namespace PDTUtils.MVVM.ViewModels
{
    class MainPageViewModel : ObservableObject
    {
        public bool IsEnabled { get; set; }
        public bool IsErrorSet
        {
            get
            {
                return BoLib.getError() > 0;
            }
        }

        public bool HandPayActive
        {
            get { return _handPayActive; }
            set
            {
                _handPayActive = value;
                if (_handPayActive && (_addCreditsActive || _canRefillHoppers))
                {
                    AddCreditsActive = false;
                    CanRefillHoppers = false;
                }
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
                if (_addCreditsActive && (_handPayActive || _canRefillHoppers))
                {
                    HandPayActive = false;
                    CanRefillHoppers = false;
                }
                RaisePropertyChangedEvent("AddCreditsActive");
#if DEBUG
                Debug.WriteLine("AddCreditsActive", _addCreditsActive.ToString());
#endif
            }
        }
        
        public bool CanRefillHoppers
        {
            get { return _canRefillHoppers; }
            set
            {
                _canRefillHoppers = value;
                if (_canRefillHoppers && (_handPayActive || _addCreditsActive))
                {
                    HandPayActive = false;
                    AddCreditsActive = false;
                }
                RaisePropertyChangedEvent("CanRefillHoppers");

#if DEBUG
                Debug.WriteLine("RefillHoppers", _canRefillHoppers.ToString());
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
                _totalCredits = Credits + Bank;// + Reserve;
                RaisePropertyChangedEvent("TotalCredits");
            } 
        }
        
        public string ErrorMessage { get; set; }
        
        bool _handPayActive;
        bool _addCreditsActive;
        bool _canPayFifty;
        bool _canRefillHoppers;
            
        string _caption = "Warning";
        string _message = "Please Open the terminal door and try again.";
        readonly  WpfMessageBoxService _msgBoxService = new WpfMessageBoxService();
        
        System.Timers.Timer _refillTimer;
        Decimal _totalCredits = 0;
        
        public MainPageViewModel()
        {
            DoorOpen = false;
            IsEnabled = true;
            HandPayActive = false;
            ErrorMessage = "";
            Credits = 0;
            Bank = 0;
            Reserve = 0;
            Pennies = 2000;
            NotRefilling = true;
            CanRefillHoppers = false;

            GetErrorMessage();
            GetCreditLevel();
            GetBankLevel();
            GetReserveLevel();
            GetMaxNoteValue();
            
            TotalCredits = 0;
        }

        public bool DoorOpen { get; set; }

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

        private ICommand ShowMessageBox
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
            var oldCaption = _caption;
            var oldMsg = _message;
            
            var total = Bank + Credits;
            
            if (total > 0)
            {
                if (BoLib.performHandPay())
                {
                    BoLib.clearBankAndCredit();
                    WriteToHandPayLog(total);
                    Credits = BoLib.getCredit();
                    Bank = BoLib.getBank();
                    Reserve = (int) BoLib.getReserveCredits();
                    TotalCredits = 0;
                    
                    RaisePropertyChangedEvent("Credits");
                    RaisePropertyChangedEvent("Bank");
                    RaisePropertyChangedEvent("Reserve");
                }
                else
                {
                    _caption = "WARNING";
                    _message = "SET HANDPAY THRESHOLD";
                    ShowMessageBox.Execute(null);
                    _caption = oldCaption;
                    _message = oldMsg;
                }
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
            var filename = Properties.Resources.hand_pay_log;

            if (!File.Exists(filename))
            {
                using (var sw = File.CreateText(filename))
                {
                    var now = DateTime.Now;
                    var amount = total;
                    sw.Write(now.ToShortDateString() + " ");
                    sw.Write(now.ToLongTimeString() + " ");
                    sw.Write((Convert.ToDecimal(amount) / 100).ToString("C") + "\r\n");
                }
            }
            else
            {
                using (var sw = File.AppendText(filename))
                {
                    var now = DateTime.Now;
                    var amount = total;
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
            var text = b.Content as Label;
            
            if (text != null)
            {
                var str = text.Content as string;

                if (str[0] != '£' && str[0] != '€')
                    Pennies = Convert.ToInt32(str.Substring(0, str.Length - 1));
                else
                {
                    Pennies = Convert.ToInt32(str.Substring(1));
                    Pennies *= 100;
                }
            }
            
            BoLib.setUtilsAdd2CreditValue((uint)Pennies);
            BoLib.setRequestUtilsAdd2Credit();
            System.Threading.Thread.Sleep(250);
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            Reserve = (int)BoLib.getReserveCredits();
            TotalCredits = Bank + Credits;
            
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
        
        //Should pass in a value here?
        //are the hoppers broken? coins not adding to the level.
        public ICommand RefillHopper { get { return new DelegateCommand(o => DoRefillHopper()); } }
        void DoRefillHopper()
        {
            CanRefillHoppers = true;
            if (_refillTimer == null)
            {
                _refillTimer = new Timer() { Enabled = true, Interval = 1000 };
                _refillTimer.Elapsed += (sender, e) =>
                {
                    var left = BoLib.getHopperFloatLevel((byte)Hoppers.LeftHopper);
                    Debug.WriteLine("Left Hopper Level", left.ToString());
                    var right = BoLib.getHopperFloatLevel((byte)Hoppers.RightHopper);
                    Debug.WriteLine("Right Hopper Level", right.ToString());
                };
            }
            else
                _refillTimer.Enabled = true;
        }
        
        public ICommand EndRefillCommand { get { return new DelegateCommand(o => DoEndRefill()); } }
        void DoEndRefill()
        {
            if (_refillTimer == null) return;                
            
            NotRefilling = false;
            _refillTimer.Enabled = false;
        }
    }
}
