using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
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
        ObservableCollection<CashReconiliation> _performance = new ObservableCollection<CashReconiliation>();

        public int NumberOfGamesLt { get; set; }
        public int NumberOfGamesSt { get; set; }

#region Properties
        public LongTermMeters LongTerm { get { return _longTerm; } }
        public ShortTermMeters ShortTerm { get { return _shortTerm; } }
        public TitoMeters TitoMeter { get { return _titoMeters; } }
        public ObservableCollection<CashReconiliation> CashRecon { get { return _cashRecon; } }
        public ObservableCollection<CashReconiliation> Performance { get { return _performance; } }
#endregion
        
        public MetersViewModel()
        {
            _longTerm.ReadMeter();
            _shortTerm.ReadMeter();
            _titoMeters.ReadMeter();
            
            NumberOfGamesLt = 0;
            
            this.ReadLCD();
            this.ReadPerformance();
            
            this.RaisePropertyChangedEvent("LongTerm");
            this.RaisePropertyChangedEvent("ShortTerm");
            this.RaisePropertyChangedEvent("TitoMeters");
            this.RaisePropertyChangedEvent("NumberOfGames");
        }

        public ICommand ClearShortTerms
        {
            get { return new DelegateCommand(o => ClearShortTermMeters()); }
        }

        void ClearShortTermMeters()
        {
            Performance.Clear();
            
            ReadLCD();
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
            //Make this a property of viewmodel and set the culture when we read the shell.
            NumberFormatInfo nfi = new CultureInfo("en-GB", false).NumberFormat;

            decimal longTermCashIn = (int)BoLib.getPerformanceMeter(0) / 100.0M;
            decimal longTermCashOut = (int)BoLib.getPerformanceMeter(1) / 100.0M;
            decimal longTermTotal = longTermCashIn - longTermCashOut;
            
            decimal shortTermCashIn = (int)BoLib.getPerformanceMeter(7) / 100.0M;
            decimal shortTermCashOut = (int)BoLib.getPerformanceMeter(8) / 100.0M;
            decimal shortTermTotal = shortTermCashIn - shortTermCashOut;

            decimal handPayLt = (int)BoLib.getPerformanceMeter(2) / 100.0M;
            decimal handPaySt = (int)BoLib.getPerformanceMeter(9) / 100.0M;
            
            Performance.Add(new CashReconiliation("Total Money in:", longTermCashIn.ToString("C", nfi), shortTermCashIn.ToString("C", nfi)));
            Performance.Add(new CashReconiliation("Total Money Out:", longTermCashOut.ToString("C", nfi), shortTermCashOut.ToString("C", nfi)));
            Performance.Add(new CashReconiliation("Gross Income:", longTermTotal.ToString("C", nfi), shortTermTotal.ToString("C", nfi)));
            Performance.Add(new CashReconiliation("Hand Pay:", handPayLt.ToString("C", nfi), handPaySt.ToString("C", nfi)));

            decimal incomeLt = longTermCashIn - handPayLt;
            decimal incomeSt = shortTermCashIn - handPaySt;
            Performance.Add(new CashReconiliation("Net Income:", incomeLt.ToString("C", nfi), incomeSt.ToString("C", nfi)));
            
            decimal refillSt = BoLib.getReconciliationMeter(42) * 100 + BoLib.getReconciliationMeter(43) * 10;
            decimal refillLt = BoLib.getReconciliationMeter(14) * 100 + BoLib.getReconciliationMeter(15) * 10;
            Performance.Add(new CashReconiliation("Refill:", refillLt.ToString("C", nfi), refillSt.ToString("C", nfi)));
            
            NumberOfGamesLt = (int)BoLib.getPerformanceMeter(4);
            NumberOfGamesSt = (int)BoLib.getPerformanceMeter(11);
            Performance.Add(new CashReconiliation("Number of Games: ", NumberOfGamesLt.ToString(), NumberOfGamesSt.ToString()));

            decimal totalBetsLt = (int)BoLib.getPerformanceMeter(5) / 100.0M;
            decimal totalBetsSt = (int)BoLib.getPerformanceMeter(12) / 100.0M;
            decimal totalWonLt = (int)BoLib.getPerformanceMeter(6) / 100.0M;
            decimal totalWonSt = (int)BoLib.getPerformanceMeter(13) / 100.0M;

            decimal percentageLt = (totalWonLt / totalBetsLt);
            decimal percentageSt = (totalWonSt / totalBetsSt);   

            decimal retainedPercLt = (int)BoLib.getPerformanceMeter(0) / 100;
            decimal retainedPercSt = (int)BoLib.getPerformanceMeter(7) / 100;

            if (retainedPercLt > 0)
                retainedPercLt = ((retainedPercLt - (longTermCashOut + handPayLt)) / longTermTotal);

            if (retainedPercSt > 0)
                retainedPercSt = ((retainedPercSt - (shortTermCashOut + handPaySt)) / shortTermTotal);

            Performance.Add(new CashReconiliation("(VTP) Total Bets:", totalBetsLt.ToString("C", nfi), totalBetsSt.ToString("C", nfi)));
            Performance.Add(new CashReconiliation("Total Wins:", totalWonLt.ToString("C", nfi), totalWonSt.ToString("C", nfi)));
            Performance.Add(new CashReconiliation("Payout Percentage:", percentageLt.ToString("P"), percentageSt.ToString("P")));
            Performance.Add(new CashReconiliation("Retained Percentage:", retainedPercLt.ToString("P"), retainedPercSt.ToString("P")));

            decimal tpCreditsLt = (int)BoLib.getTPlayMeter(2);
            decimal tpCreditsSt = (int)BoLib.getTPlayMeter(5);

            Performance.Add(new CashReconiliation("TPlay Total Credits:", tpCreditsLt.ToString("C", nfi), tpCreditsSt.ToString("C", nfi)));

            int tpGamesLt = (int)BoLib.getTPlayMeter(1);
            int tpGamesSt = (int)BoLib.getTPlayMeter(4);
            Performance.Add(new CashReconiliation("TP Games Played:", tpGamesLt.ToString(), tpGamesSt.ToString()));

            decimal tpMoneyOutLt = (int)BoLib.getTPlayMeter(0);
            decimal tpMoneyOutSt = (int)BoLib.getTPlayMeter(3);
            Performance.Add(new CashReconiliation("TPlay Cash Out:", tpMoneyOutLt.ToString("C", nfi), tpMoneyOutSt.ToString("C", nfi)));

            this.RaisePropertyChangedEvent("Performance");
            this.RaisePropertyChangedEvent("NumberOfGamesLt");
            this.RaisePropertyChangedEvent("NumberOfGamesSt");
        }
    }
}
