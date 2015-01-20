using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using System.Threading;

namespace PDTUtils.MVVM.ViewModels
{
    class MetersViewModel : ObservableObject
    {
        NumberFormatInfo nfi;// = Thread.CurrentThread.CurrentCulture.NumberFormat; /*new CultureInfo("en-GB", false).NumberFormat;*/
        LongTermMeters _longTerm = new LongTermMeters();
        ShortTermMeters _shortTerm = new ShortTermMeters();
        TitoMeters _titoMeters = new TitoMeters();
        ObservableCollection<HelloImJohnnyCashMeters> _cashRecon = new ObservableCollection<HelloImJohnnyCashMeters>();
        ObservableCollection<HelloImJohnnyCashMeters> _performance = new ObservableCollection<HelloImJohnnyCashMeters>();
        ObservableCollection<HelloImJohnnyCashMeters> _refill = new ObservableCollection<HelloImJohnnyCashMeters>();
        ObservableCollection<GameStatMeter> _gameStats = new ObservableCollection<GameStatMeter>();

        public int NumberOfGamesLt { get; set; }
        public int NumberOfGamesSt { get; set; }

#region Properties
        public LongTermMeters LongTerm { get { return _longTerm; } }
        public ShortTermMeters ShortTerm { get { return _shortTerm; } }
        public TitoMeters TitoMeter { get { return _titoMeters; } }
        public ObservableCollection<HelloImJohnnyCashMeters> CashRecon { get { return _cashRecon; } }
        public ObservableCollection<HelloImJohnnyCashMeters> Performance { get { return _performance; } }
        public ObservableCollection<HelloImJohnnyCashMeters> Refill { get { return _refill; } }
        public ObservableCollection<GameStatMeter> GameStats { get { return _gameStats; } }
#endregion
        
        public MetersViewModel()
        {
            if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                nfi = new CultureInfo("es-ES").NumberFormat;
            else
                nfi = new CultureInfo("en-GB").NumberFormat;
            
            _longTerm.ReadMeter();
            _shortTerm.ReadMeter();
            _titoMeters.ReadMeter();
            
            NumberOfGamesLt = 0;
           
            ReadPerformance();
            ReadCashRecon();
            
            RaisePropertyChangedEvent("LongTerm");
            RaisePropertyChangedEvent("ShortTerm");
            RaisePropertyChangedEvent("TitoMeters");
            RaisePropertyChangedEvent("NumberOfGames");
        }
        
        public ICommand ClearShortTerms
        {
            get { return new DelegateCommand(o => ClearShortTermMeters()); }
        }

        void ClearShortTermMeters()
        {
            Performance.Clear();
            CashRecon.Clear();
            GameStats.Clear();

            BoLib.clearShortTermMeters();
            NativeWinApi.WritePrivateProfileString("TicketsIn", "TicketCount", "0", @Properties.Resources.tito_log);
            NativeWinApi.WritePrivateProfileString("TicketsOut", "TicketCount", "0", @Properties.Resources.tito_log);
            
            ReadPerformance();
            ReadCashRecon();
        }
        
        public void ReadCashRecon()
        {
            string[] note_headers = new string[] { "£50 NOTES IN", "£20 NOTES IN", "£10 NOTES IN", "£5 NOTES IN" };
            string[] cash_headers = new string[] { "TOTAL CASH IN", "TOTAL CASH OUT", "TOTAL" };
            string[] coin_headers = new string[] { "£2 COINS IN", "£1 COINS IN", "50p COINS IN", "20p COINS IN", "10p COINS IN", "5p COINS IN" };
            
            _cashRecon.Add(new HelloImJohnnyCashMeters("STAKE IN", 
                                                       ((int)BoLib.useStakeInMeter(0)).ToString(), 
                                                       ((int)BoLib.useStakeInMeter(1)).ToString()));

            byte[] perfLt = new byte[] {  2,  3,  4,  5 };
            byte[] perfSt = new byte[] { 30, 31, 32, 33 };

            int ctr = 0;

            for (int i = 2; i <= 5; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(note_headers[i - 2],
                                                           BoLib.getReconciliationMeter(perfLt[ctr]).ToString(),
                                                           BoLib.getReconciliationMeter(perfSt[ctr]).ToString()));
                ctr++;
            }
            
            // corresponding to recon meter #defines in the bo lib.
            byte[] perfCoinLt = new byte[] {  8,  9, 10, 11, 12 };
            byte[] perfCoinSt = new byte[] { 36, 37, 38, 39, 40 };
            ctr = 0;
            for (int i = 8; i <= 12; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(coin_headers[i - 8],
                                                           BoLib.getReconciliationMeter(perfCoinLt[ctr]).ToString(),
                                                           BoLib.getReconciliationMeter(perfCoinSt[ctr]).ToString()));
                ctr++;
            }
            
            _refill.Add(new HelloImJohnnyCashMeters("£1 Coins IN", 
                                                    BoLib.getReconciliationMeter(14).ToString(),
                                                    BoLib.getReconciliationMeter(42).ToString()));

            _refill.Add(new HelloImJohnnyCashMeters("10p Coins IN",
                                                    BoLib.getReconciliationMeter(15).ToString(),
                                                    BoLib.getReconciliationMeter(43).ToString()));

            this.RaisePropertyChangedEvent("CashRecon");
            this.RaisePropertyChangedEvent("Refill");
        }
        
        void ReadPerformance()
        {
            decimal longTermCashIn  = (int)BoLib.getPerformanceMeter(0) / 100.0M;
            decimal longTermCashOut = (int)BoLib.getPerformanceMeter(1) / 100.0M;
            decimal longTermTotal   = longTermCashIn - longTermCashOut;
            
            decimal shortTermCashIn     = (int)BoLib.getPerformanceMeter(7) / 100.0M;
            decimal shortTermCashOut    = (int)BoLib.getPerformanceMeter(8) / 100.0M;
            decimal shortTermTotal      = shortTermCashIn - shortTermCashOut;//t

            decimal handPayLt = (int)BoLib.getPerformanceMeter(2) / 100.0M;
            decimal handPaySt = (int)BoLib.getPerformanceMeter(9) / 100.0M;
            
            Performance.Add(new HelloImJohnnyCashMeters("Total Money in:", longTermCashIn.ToString("C", nfi), shortTermCashIn.ToString("C", nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Money Out:", longTermCashOut.ToString("C", nfi), shortTermCashOut.ToString("C", nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Gross Income:", longTermTotal.ToString("C", nfi), shortTermTotal.ToString("C", nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Hand Pay:", handPayLt.ToString("C", nfi), handPaySt.ToString("C", nfi)));
            
            decimal incomeLt = longTermCashIn - handPayLt;
            decimal incomeSt = shortTermCashIn - handPaySt;
            Performance.Add(new HelloImJohnnyCashMeters("Net Income:", incomeLt.ToString("C", nfi), incomeSt.ToString("C", nfi)));
            
            decimal refillSt = BoLib.getReconciliationMeter(42) + BoLib.getReconciliationMeter(43);
            decimal refillLt = BoLib.getReconciliationMeter(14) + BoLib.getReconciliationMeter(15);
            Performance.Add(new HelloImJohnnyCashMeters("Refill:", refillLt.ToString("C", nfi), refillSt.ToString("C", nfi)));
            
            NumberOfGamesLt = (int)BoLib.getPerformanceMeter(4);
            NumberOfGamesSt = (int)BoLib.getPerformanceMeter(11);
            Performance.Add(new HelloImJohnnyCashMeters("Number of Games: ", NumberOfGamesLt.ToString(), NumberOfGamesSt.ToString()));
            
            decimal totalBetsLt = (int)BoLib.getPerformanceMeter(5)  / 100.0M;
            decimal totalBetsSt = (int)BoLib.getPerformanceMeter(12) / 100.0M;
            decimal totalWonLt  = (int)BoLib.getPerformanceMeter(6)  / 100.0M;
            decimal totalWonSt  = (int)BoLib.getPerformanceMeter(13) / 100.0M;

            decimal percentageLt = (totalWonLt > 0 && totalBetsLt > 0) ? (totalWonLt / totalBetsLt) : 0;
            decimal percentageSt = (totalWonSt > 0 && totalBetsSt > 0) ? (totalWonSt / totalBetsSt) : 0;
            
            decimal retainedPercLt = (int)BoLib.getPerformanceMeter(0) / 100;
            decimal retainedPercSt = (int)BoLib.getPerformanceMeter(7) / 100;
            
            if (retainedPercLt > 0)
                retainedPercLt = ((retainedPercLt - (longTermCashOut + handPayLt)) / longTermTotal);
                        
            if (retainedPercSt > 0)
                retainedPercSt = ((retainedPercSt - (shortTermCashOut + handPaySt)) / shortTermTotal);
            
            Performance.Add(new HelloImJohnnyCashMeters("(VTP) Total Bets:", totalBetsLt.ToString("C", nfi), totalBetsSt.ToString("C", nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Wins:", totalWonLt.ToString("C", nfi), totalWonSt.ToString("C", nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Payout Percentage:", percentageLt.ToString("P"), percentageSt.ToString("P")));
            Performance.Add(new HelloImJohnnyCashMeters("Retained Percentage:", retainedPercLt.ToString("P"), retainedPercSt.ToString("P")));
            
            decimal tpCreditsLt = (int)BoLib.getTPlayMeter(2);
            decimal tpCreditsSt = (int)BoLib.getTPlayMeter(5);

            Performance.Add(new HelloImJohnnyCashMeters("TPlay Total Credits:", tpCreditsLt.ToString("C", nfi), tpCreditsSt.ToString("C", nfi)));

            int tpGamesLt = (int)BoLib.getTPlayMeter(1);
            int tpGamesSt = (int)BoLib.getTPlayMeter(4);
            Performance.Add(new HelloImJohnnyCashMeters("TP Games Played:", tpGamesLt.ToString(), tpGamesSt.ToString()));

            decimal tpMoneyOutLt = (int)BoLib.getTPlayMeter(0);
            decimal tpMoneyOutSt = (int)BoLib.getTPlayMeter(3);
            Performance.Add(new HelloImJohnnyCashMeters("TPlay Cash Out:", tpMoneyOutLt.ToString("C", nfi), tpMoneyOutSt.ToString("C", nfi)));
            
            //Read Game Stats meters
            for (uint i = 0; i <= BoLib.getNumberOfGames(); i++) //shell as well as games
            {
                uint model = BoLib.getGameModel((int)i);
                decimal bets = (int)BoLib.getGamePerformanceMeter(i, 0) / 100.0M;
                decimal won = (int)BoLib.getGamePerformanceMeter(i, 1) / 100.0M;
                decimal percentage = (won > 0 || bets > 0) ? (won / bets) : 0;
                GameStats.Add(new GameStatMeter(model.ToString(), bets.ToString("C", nfi), won.ToString("C", nfi), percentage.ToString("P", nfi)));
            }

            RaisePropertyChangedEvent("CashRecon");
            RaisePropertyChangedEvent("NumberOfGamesLt");
            RaisePropertyChangedEvent("NumberOfGamesSt");
            RaisePropertyChangedEvent("GameStats");
        }
    }
}
