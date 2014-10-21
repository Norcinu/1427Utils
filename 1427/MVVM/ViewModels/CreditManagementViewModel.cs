using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class CreditManagementViewModel : ObservableObject
    {
        public int Credits { get; set; }

        public CreditManagementViewModel()
        {
            Credits = 0;
        }
            
        public ICommand GetCreditsLevel
        {
            get { return new DelegateCommand(o => GetCredits()); }
        }
        void GetCredits()
        {
            Credits = BoLib.getCredit();
        }
        
        public ICommand AddCreditsToBank
        {
            get { return new DelegateCommand(o => AddCredits()); }
        }
        void AddCredits()
        {
            BoLib.addCredit(1000);
        }

        public ICommand ClearCreditLevel
        {
            get { return new DelegateCommand(o => ClearCredits()); }
        }
        void ClearCredits()
        {
            BoLib.clearBankAndCredit();
        }
    }
}
