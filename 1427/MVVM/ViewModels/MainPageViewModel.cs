using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class MainPageViewModel : ObservableObject
    {
        public int Credits { get; set; }
        public int Bank { get; set; }
        public int Pennies { get; set; }
        
        public string ErrorMessage { get; set; }
        string _caption = "Warning";
        string _message = "Please Open the terminal door and try again.";
        WPFMessageBoxService _msgBoxService = new WPFMessageBoxService();
        
        public MainPageViewModel()
        {
            ErrorMessage = "";
            Credits = 0;
            Bank = 0;
            Pennies = 2000;
            this.GetErrorMessage();
            this.GetCreditLevel();
            this.GetBankLevel();
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
            get { return new DelegateCommand(o => AddCreditsLevel()); }
        }
        
        void AddCreditsLevel()
        {
            BoLib.addCredit(Pennies);
            Credits = BoLib.getCredit();
            Bank = BoLib.getBank();
            this.RaisePropertyChangedEvent("Credits");
            this.RaisePropertyChangedEvent("Bank");
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
    }
}
