using System;
using System.Diagnostics;
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
                this.RaisePropertyChangedEvent("HandPayActive");
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
                this.RaisePropertyChangedEvent("AddCreditsActive");
#if DEBUG
                Debug.WriteLine("AddCreditsActive", _addCreditsActive.ToString());
#endif
            }
        }
        
        public int Credits { get; set; }
        public int Bank { get; set; }
        public int Pennies { get; set; }
        public int TotalCredits { get { return Credits + Bank; } }
        public string ErrorMessage { get; set; }
        
        bool _handPayActive;
        bool _addCreditsActive;

        string _caption = "Warning";
        string _message = "Please Open the terminal door and try again.";
        WPFMessageBoxService _msgBoxService = new WPFMessageBoxService();
        
        public MainPageViewModel()
        {
            IsEnabled = true;
            HandPayActive = false;
            ErrorMessage = "";
            Credits = 0;
            Bank = 0;
            Pennies = 2000;
            
            GetErrorMessage();
            GetCreditLevel();
            GetBankLevel();
        }
        
        public ICommand GetCredit
        {
            get { return new DelegateCommand(o => GetCreditLevel()); }
        }

        void GetCreditLevel()
        {
            Credits = BoLib.getCredit();
            this.RaisePropertyChangedEvent("Credits");
        }

        public ICommand GetBank
        {
            get { return new DelegateCommand(o => GetBankLevel()); }
        }
        
        void GetBankLevel()
        {
            Bank = BoLib.getBank();
            this.RaisePropertyChangedEvent("Bank");
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
            this.RaisePropertyChangedEvent("Credits");
            this.RaisePropertyChangedEvent("Bank");
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
            this.RaisePropertyChangedEvent("Credits");
            this.RaisePropertyChangedEvent("Bank");
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
                string last = BoLib.getErrorMessage("", BoLib.getError());
                ErrorMessage = "Current Error : " + "[" + errorCode + "] " + last + "\nOpen Door and Press Button To Clear Error";
            }
            else
                ErrorMessage = "No Current Error";

            this.RaisePropertyChangedEvent("ErrorMessage");
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
                if (BoLib.clearError() == 0)
                {
                    ErrorMessage = "Error Cleared. Please Continue.";
                    this.RaisePropertyChangedEvent("ErrorMessage");
                }
            }
            else
            {
                this.ShowMessageBox.Execute(null);
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
            
            if (Bank + Credits > 0)
            {
                if (!BoLib.performHandPay())
                {
                    _caption = "WARNING";
                    _message = "SET HANDPAY THRESHOLD";
                    this.ShowMessageBox.Execute(null);
                    _caption = oldCaption;
                    _message = oldMsg;
                    return;
                }

                Credits = BoLib.getCredit();
                Bank = BoLib.getBank();
                this.RaisePropertyChangedEvent("Credits");
                this.RaisePropertyChangedEvent("Bank");
            }
            else
            {
                _caption = "WARNING";
                _message = "NO CREDITS FOR HAND PAY";
                this.ShowMessageBox.Execute(null);
                _caption = oldCaption;
                _message = oldMsg;
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
            
            BoLib.addCredit(Pennies);
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            this.RaisePropertyChangedEvent("Credits");
            this.RaisePropertyChangedEvent("Bank");
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
    }
}
