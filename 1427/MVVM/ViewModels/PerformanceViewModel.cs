using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class MetersViewModel : ObservableObject
    {
        readonly NumberFormatInfo _nfi;
        readonly LongTermMeters _longTerm = new LongTermMeters();
        readonly ShortTermMeters _shortTerm = new ShortTermMeters();
        readonly TitoMeters _titoMeters = new TitoMeters();
        readonly ObservableCollection<HelloImJohnnyCashMeters> _cashRecon = new ObservableCollection<HelloImJohnnyCashMeters>();
        readonly ObservableCollection<HelloImJohnnyCashMeters> _performance = new ObservableCollection<HelloImJohnnyCashMeters>();
        readonly ObservableCollection<HelloImJohnnyCashMeters> _refill = new ObservableCollection<HelloImJohnnyCashMeters>();
        readonly ObservableCollection<GameStatMeter> _gameStats = new ObservableCollection<GameStatMeter>();
        
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
            _nfi = BoLib.getCountryCode() == BoLib.getSpainCountryCode() 
                ? new CultureInfo("es-ES").NumberFormat 
                : new CultureInfo("en-GB").NumberFormat;

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
            var noteHeaders = new[] { "£50 NOTES IN", "£20 NOTES IN", "£10 NOTES IN", "£5 NOTES IN" };
            // ReSharper disable once UnusedVariable
            //var cashHeaders = new[] { "TOTAL CASH IN", "TOTAL CASH OUT", "TOTAL" };
            var coinHeaders = new[] { "£2 COINS IN", "£1 COINS IN", "50p COINS IN", "20p COINS IN", "10p COINS IN", "5p COINS IN" };

            _cashRecon.Add(new HelloImJohnnyCashMeters("STAKE IN",
                                                       ((int)BoLib.useStakeInMeter(0)).ToString(),
                                                       ((int)BoLib.useStakeInMeter(1)).ToString()));

            var perfLt = new byte[] { 2, 3, 4, 5 };
            var perfSt = new byte[] { 30, 31, 32, 33 };

            var ctr = 0;

            for (var i = 2; i <= 5; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(noteHeaders[i - 2],
                                                           BoLib.getReconciliationMeter(perfLt[ctr]).ToString(),
                                                           BoLib.getReconciliationMeter(perfSt[ctr]).ToString()));
                ctr++;
            }
            
            // corresponding to recon meter #defines in the bo lib.
            var perfCoinLt = new byte[] {8, 9, 10, 11, 12};
            var perfCoinSt = new byte[] {36, 37, 38, 39, 40};
            ctr = 0;
            for (var i = 8; i <= 12; i++)
            {
                _cashRecon.Add(new HelloImJohnnyCashMeters(coinHeaders[i - 8],
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

            RaisePropertyChangedEvent("CashRecon");
            RaisePropertyChangedEvent("Refill");
        }
        
        void ReadPerformance()
        {
            var longTermCashIn = (int)BoLib.getPerformanceMeter(0) / 100.0M;
            var longTermCashOut = (int)BoLib.getPerformanceMeter(1) / 100.0M;
            var longTermTotal = longTermCashIn - longTermCashOut;
            
            var shortTermCashIn = (int)BoLib.getPerformanceMeter(7) / 100.0M;
            var shortTermCashOut = (int)BoLib.getPerformanceMeter(8) / 100.0M;
            var shortTermTotal = shortTermCashIn - shortTermCashOut;
            
            var handPayLt = (int)BoLib.getPerformanceMeter(2) / 100.0M;
            var handPaySt = (int)BoLib.getPerformanceMeter(9) / 100.0M;

            Performance.Add(new HelloImJohnnyCashMeters("Total Money in:", 
                                                        longTermCashIn.ToString("C", _nfi), 
                                                        shortTermCashIn.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Money Out:", 
                                                        longTermCashOut.ToString("C", _nfi), 
                                                        shortTermCashOut.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Gross Income:", 
                                                        longTermTotal.ToString("C", _nfi), 
                                                        shortTermTotal.ToString("C", _nfi)));
            Performance.Add(new HelloImJohnnyCashMeters("Hand Pay:", 
                                                        handPayLt.ToString("C", _nfi), 
                                                        handPaySt.ToString("C", _nfi)));

            var incomeLt = longTermCashIn - handPayLt;
            var incomeSt = shortTermCashIn - handPaySt;
            Performance.Add(new HelloImJohnnyCashMeters("Net Income:", 
                                                        incomeLt.ToString("C", _nfi), 
                                                        incomeSt.ToString("C", _nfi)));

            decimal refillSt = BoLib.getReconciliationMeter(42) + BoLib.getReconciliationMeter(43);
            decimal refillLt = BoLib.getReconciliationMeter(14) + BoLib.getReconciliationMeter(15);
            Performance.Add(new HelloImJohnnyCashMeters("Refill:", 
                                                        refillLt.ToString("C", _nfi), 
                                                        refillSt.ToString("C", _nfi)));

            NumberOfGamesLt = (int)BoLib.getPerformanceMeter(4);
            NumberOfGamesSt = (int)BoLib.getPerformanceMeter(11);
            Performance.Add(new HelloImJohnnyCashMeters("Number of Games: ", 
                                                        NumberOfGamesLt.ToString(), 
                                                        NumberOfGamesSt.ToString()));

            double totalBetsLt = 0; //(int)BoLib.getPerformanceMeter((byte)Native.Performance.WageredLt) / 100.0M; // TOTAL UP ACCUMULATE
            double totalBetsSt = 0; //(int)BoLib.getPerformanceMeter((byte)Native.Performance.WageredSt) / 100.0M; // TOTAL UP ACCUMULATE
            double totalWonLt = 0; //(int)BoLib.getPerformanceMeter((byte)Native.Performance.WonLt) / 100.0M; // TOTAL UP ACCUMULATE
            double totalWonSt = 0; //(int)BoLib.getPerformanceMeter((byte)Native.Performance.WonSt) / 100.0M; // TOTAL UP ACCUMULATE

            for (uint i = 1; i <= BoLib.getNumberOfGames(); i++)
            {
                totalBetsLt += (int)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWageredLt);
                totalBetsSt += (int)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWageredSt);
                totalWonLt += (int)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonLt);
                totalWonSt += (int)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonSt);
            }

            totalBetsLt /= 100;
            totalBetsSt /= 100;
            totalWonLt /= 100;
            totalWonSt /= 100;

            var percentageLt = (totalWonLt > 0 && totalBetsLt > 0) ? (totalWonLt / totalBetsLt) : 0;
            var percentageSt = (totalWonSt > 0 && totalBetsSt > 0) ? (totalWonSt / totalBetsSt) : 0;
            
            decimal retainedPercLt = (int)BoLib.getPerformanceMeter(0) / 100;
            decimal retainedPercSt = (int)BoLib.getPerformanceMeter(7) / 100;
            
            if (retainedPercLt > 0)
                retainedPercLt = ((retainedPercLt - (longTermCashOut + handPayLt)) / longTermTotal);

            if (retainedPercSt > 0)
                retainedPercSt = ((retainedPercSt - (shortTermCashOut + handPaySt)) / shortTermTotal);
            
            Performance.Add(new HelloImJohnnyCashMeters("Total Fischas Bet:", // fischas bet
                                                        totalBetsLt.ToString(/*"C", _nfi*/), 
                                                        totalBetsSt.ToString(/*"C", _nfi)*/)));
            Performance.Add(new HelloImJohnnyCashMeters("Total Fischas Wins:", 
                                                        totalWonLt.ToString(/*"C", _nfi*/), 
                                                        totalWonSt.ToString(/*"C", _nfi*/)));
            Performance.Add(new HelloImJohnnyCashMeters("Payout Percentage:", 
                                                        percentageLt.ToString("P"), 
                                                        percentageSt.ToString("P")));
            Performance.Add(new HelloImJohnnyCashMeters("Retained Percentage:", 
                                                        retainedPercLt.ToString("P"), 
                                                        retainedPercSt.ToString("P")));

            decimal tpCreditsLt = (int)BoLib.getTPlayMeter(2);
            decimal tpCreditsSt = (int)BoLib.getTPlayMeter(5);

            Performance.Add(new HelloImJohnnyCashMeters("TPlay Total Credits:", 
                                                        tpCreditsLt.ToString("C", _nfi), 
                                                        tpCreditsSt.ToString("C", _nfi)));
            
            var tpGamesLt = (int)BoLib.getTPlayMeter(1);
            var tpGamesSt = (int)BoLib.getTPlayMeter(4);
            Performance.Add(new HelloImJohnnyCashMeters("TP Games Played:", 
                                                        tpGamesLt.ToString(), 
                                                        tpGamesSt.ToString()));
            
            decimal tpMoneyOutLt = (int)BoLib.getTPlayMeter(0);
            decimal tpMoneyOutSt = (int)BoLib.getTPlayMeter(3);
            Performance.Add(new HelloImJohnnyCashMeters("TPlay Cash Out:", 
                                                        tpMoneyOutLt.ToString("C", _nfi), 
                                                        tpMoneyOutSt.ToString("C", _nfi)));

            //Read Game Stats meters
            for (uint i = 0; i <= BoLib.getNumberOfGames(); i++) //shell as well as games
            {
                var model = BoLib.getGameModel((int)i);
                var bets = (uint)BoLib.getGamePerformanceMeter(i, 0) / 100.0M;
                var won = (int)BoLib.getGamePerformanceMeter(i, 1) / 100.0M;
                var percentage = (won > 0 || bets > 0) ? (won / bets) : 0;
                GameStats.Add(new GameStatMeter(model.ToString(), 
                                                bets.ToString("C", _nfi), 
                                                won.ToString("C", _nfi),
                                                System.Math.Round(percentage, 2).ToString("P", _nfi)));
            }
            
            RaisePropertyChangedEvent("CashRecon");
            RaisePropertyChangedEvent("NumberOfGamesLt");
            RaisePropertyChangedEvent("NumberOfGamesSt");
            RaisePropertyChangedEvent("GameStats");
        }
    }
}
