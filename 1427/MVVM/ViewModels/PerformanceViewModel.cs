using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class MetersViewModel : ObservableObject
    {
        LongTermMeters _longTerm = new LongTermMeters();
        ShortTermMeters _shortTerm = new ShortTermMeters();
        TitoMeters _titoMeters = new TitoMeters(); 
        ObservableCollection<CashReconiliation> _cashRecon = new ObservableCollection<CashReconiliation>();
        ObservableCollection<PerformanceMeter> _performance = new ObservableCollection<PerformanceMeter>();

        public int NumberOfGames { get; set; }

#region Properties
        public LongTermMeters LongTerm { get { return _longTerm; } }
        public ShortTermMeters ShortTerm { get { return _shortTerm; } }
        public TitoMeters TitoMeter { get { return _titoMeters; } }
        public ObservableCollection<CashReconiliation> CashRecon { get { return _cashRecon; } }
        public ObservableCollection<PerformanceMeter> Performance { get { return _performance; } }
#endregion
        
        public MetersViewModel()
        {
            _longTerm.ReadMeter();
            _shortTerm.ReadMeter();
            _titoMeters.ReadMeter();

            NumberOfGames = 0;

            this.ReadLCD();
            this.ReadPerformance();
            
            this.RaisePropertyChangedEvent("LongTerm");
            this.RaisePropertyChangedEvent("ShortTerm");
            this.RaisePropertyChangedEvent("TitoMeters");
            this.RaisePropertyChangedEvent("NumberOfGames");
        }
        
        public void ReadLCD()
        {
            string[] note_headers = new string[] { "£50 NOTES IN", "£20 NOTES IN", "£10 NOTES IN", "£5 NOTES IN" };
            string[] cash_headers = new string[] { "TOTAL CASH IN", "TOTAL CASH OUT", "TOTAL" };
            string[] coin_headers = new string[] { "£2 COINS IN", "£1 COINS IN", "50p COINS IN", "20p COINS IN" };
            
            _cashRecon.Add(new CashReconiliation("STAKE IN", 
                                                 BoLib.useStakeInMeter(0).ToString(), 
                                                 BoLib.useStakeInMeter(1).ToString()));
            
            for (int i = 0; i <= 3; i++)
            {
                _cashRecon.Add(new CashReconiliation(note_headers[i], 
                                                     BoLib.getNotesIn(0).ToString(), 
                                                     BoLib.getNotesIn(1).ToString()));
            }

            for (int i = 0; i <= 1; i++)
            {
                _cashRecon.Add(new CashReconiliation(cash_headers[i],
                                                     BoLib.getCoinsIn(0).ToString(),
                                                     BoLib.getCoinsIn(1).ToString()));
            }
            
            this.RaisePropertyChangedEvent("CashRecon");
        }

        void ReadPerformance()
        {
            var longTermCashIn = BoLib.getPerformanceMeter(0) / 100;
            var longTermCashOut = BoLib.getPerformanceMeter(1) / 100;
            var longTermTotal = (longTermCashIn - longTermCashOut);
            
            var shortTermCashIn = BoLib.getPerformanceMeter(7) / 100;
            var shortTermCashOut = BoLib.getPerformanceMeter(8) / 100;
            var shortTermTotal = (shortTermCashIn - shortTermCashOut);

            var handPayLt = BoLib.getPerformanceMeter(2) / 100;
            var handPaySt = BoLib.getPerformanceMeter(9) / 100;
            
            Performance.Add(new PerformanceMeter("Total Money in:",
                                                   (long)longTermCashIn,
                                                   (long)shortTermCashIn));

            Performance.Add(new PerformanceMeter("Total Money Out:",
                                                   (long)longTermCashOut,
                                                   (long)shortTermCashOut));

            Performance.Add(new PerformanceMeter("Gross Income:",
                                                   (long)longTermTotal,
                                                   (long)shortTermTotal));

            Performance.Add(new PerformanceMeter("Hand Pay:", (long)handPayLt, (long)handPaySt));
            
            var refillSt = BoLib.getReconciliationMeter(42) * 100 + BoLib.getReconciliationMeter(43) * 10;
            var refillLt = BoLib.getReconciliationMeter(14) * 100 + BoLib.getReconciliationMeter(15) * 10;
            Performance.Add(new PerformanceMeter("Refill:", refillLt, refillSt));

            int incomeLt = (int)(longTermCashIn - handPayLt);
            int incomeSt = (int)(shortTermCashIn - handPaySt);
            Performance.Add(new PerformanceMeter("Net Income:", incomeLt, incomeSt));
            
            NumberOfGames = (int)BoLib.getPerformanceMeter(4);
            
            this.RaisePropertyChangedEvent("Performance");
            this.RaisePropertyChangedEvent("NumberOfGames");
        }
    }
}
