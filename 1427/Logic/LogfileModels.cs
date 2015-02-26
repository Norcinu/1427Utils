using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using PDTUtils.MVVM;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public abstract void ParseGame(int gameNo);
        
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
    
    public abstract class BaseGameLog : BaseNotifyPropertyChanged, IComparable
    {
        private readonly NumberFormatInfo nfi;
        private CultureInfo ci = new CultureInfo("en-GB");
        
        protected BaseGameLog()
        {
            //CultureInfo ci = new CultureInfo("en-GB");//"es-ES");
            nfi = (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();
            nfi.CurrencySymbol = "£";
            stake = 0;
            credit = 0;
        }
        
        public string GameDate
        {
            get { return logDate.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string LogDate
        {
            get { return logDate.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string Stake
        {
            get { return (stake/100m).ToString("c2"); }
        }

        public string Credit
        {
            get { return (credit/100m).ToString("c2"); }
        }

        protected uint GameModel { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is PlayedGame)
            {
            }

            return 0;
        }
        
        #region Private Variables

        protected DateTime logDate;
        protected decimal credit;
        protected decimal stake;

        #endregion
    }

    public class WinningGame : BaseGameLog
    {
        private Decimal winAmount;

        public WinningGame(int gameNo)
        {
            ParseGame(gameNo);
        }

        public string WinAmount
        {
            get { return (winAmount/100m).ToString("c2"); }
        }

        public override void ParseGame(int gameNo)
        {
            var ci = new CultureInfo("en-GB"); // en-GB
            var date = DateTime.Now.ToString();
            DateTime.TryParse(date, ci, DateTimeStyles.None, out logDate);
            
            GameModel = (uint) BoLib.getLastGameModel(gameNo); // .getGameModel(gameNo);
            stake = BoLib.getGameWager(gameNo);
            var tempGameDate = BoLib.getGameDate(gameNo);
            var today = (int) tempGameDate >> 16;
            var month = (int) tempGameDate & 0x0000FFFF;
            logDate = new DateTime(2014, month, today);
            winAmount = BoLib.getWinningGame(gameNo);
            credit = BoLib.getGameCreditLevel(gameNo);
            OnPropertyChanged("WinningGames");
        }
    }

    public class PlayedGame : BaseGameLog
    {
        private decimal winAmount;

        public PlayedGame()
        {
            winAmount = 0;
        }

        public PlayedGame(int gameNo)
        {
            ParseGame(gameNo);
        }

        public string WinAmount
        {
            get { return (winAmount/100).ToString("c2"); }
        }

        public override void ParseGame(int gameNo)
        {
            //CultureInfo ci = new CultureInfo("en-GB");
            var ci = Thread.CurrentThread.CurrentCulture;
            var cui = Thread.CurrentThread.CurrentUICulture;

            var today = DateTime.Today;
            var gameDate = BoLib.getGameDate(gameNo);
            var time = BoLib.getGameTime(gameNo);

            var hour = time >> 16;
            var minute = time & 0x0000FFFF;
            
            var month = gameDate & 0x0000FFFF;
            var day = gameDate >> 16;
            var year = DateTime.Now.Year;
            if (month > DateTime.Now.Month)
                --year;

            try
            {
                var ds = day + @"/" + month + @"/" + year + " " + hour + " " + ":" + minute;
                credit = BoLib.getGameCreditLevel(gameNo);
                stake = BoLib.getGameWager(gameNo);
                GameModel = (uint) BoLib.getLastGameModel(gameNo);

                logDate = DateTime.Parse(ds, ci);
                winAmount = BoLib.getWinningGame(gameNo);

                OnPropertyChanged("PlayedGames");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class MachineErrorLog : BaseGameLog
    {
        public MachineErrorLog()
        {
            OnPropertyChanged("ErrorLog");
        }

        public MachineErrorLog(string code, string desciption, string date)
        {
            ErrorCode = code;
            Description = desciption;
            ErrorDate = date;
            OnPropertyChanged("ErrorLog");
        }

        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public string ErrorDate { get; set; }

        public override void ParseGame(int gameNo)
        {
            throw new NotImplementedException();
        }
    }

    public class HandPayLog : BaseNotifyPropertyChanged
    {
        //public string Operator { get; set; }

        public HandPayLog()
        {
            Time = "";
            Amount = "";
            //    Operator = "";
        }

        public HandPayLog(string time, string amount)
        {
            Time = time;
            Amount = amount;
            OnPropertyChanged("HandPayLogs");
        }

        public string Time { get; set; }
        public string Amount { get; set; }

        public override void ParseGame(int gameNo)
        {
        }
    }

    public class CashlessLibLog : BaseNotifyPropertyChanged
    {
        public CashlessLibLog()
        {
            Time = "";
            Message = "";
        }

        public string Time { get; set; }
        public string Message { get; set; }

        public override void ParseGame(int gameNo)
        {
            throw new NotImplementedException();
        }
    }


    public class MachineLogsController
    {
        private readonly ObservableCollection<CashlessLibLog> m_cashLess = new ObservableCollection<CashlessLibLog>();
        private readonly ObservableCollection<MachineErrorLog> m_errorLog = new ObservableCollection<MachineErrorLog>();
        private readonly ObservableCollection<HandPayLog> m_handPayLog = new ObservableCollection<HandPayLog>();
        private readonly ObservableCollection<PlayedGame> m_playedGames = new ObservableCollection<PlayedGame>();

        private readonly ObservableCollection<MachineErrorLog> m_warningLog =
            new ObservableCollection<MachineErrorLog>();

        private readonly ObservableCollection<WinningGame> m_winningGames = new ObservableCollection<WinningGame>();

        public MachineLogsController()
        {
            IsLoaded = false;
        }

        public void setErrorLog()
        {
            var errLogLocation = @"D:\machine\GAME_DATA\TerminalErrLog.log";
            try
            {
                var lines = File.ReadAllLines(errLogLocation);
                var reveresed = new string[lines.Length - 1];

                var ctr = 0;
                for (var i = lines.Length - 1; i > 0; i--)
                {
                    reveresed[ctr] = lines[i];
                    ctr++;
                }

                foreach (var s in reveresed)
                {
                    try
                    {
                        var subStr = s.Split("\t".ToCharArray());
                        bool? b = s.Contains("TimeStamp");
                        if (b == false && s != "")
                        {
                            foreach (var ss in subStr)
                            {
                                if (ss != "")
                                {
                                    var timeAndDate = ss.Substring(0, 19).TrimStart(" \t".ToCharArray());
                                    var errorCode = ss.Substring(21, 4).TrimStart(" \t".ToCharArray());
                                    var desc = ss.Substring(26).TrimStart(" \t".ToCharArray());
                                    ErrorLog.Add(new MachineErrorLog(errorCode, desc, timeAndDate));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void setWarningLog()
        {
            var errLogLocation = @"D:\machine\GAME_DATA\TerminalWarningLog.log";
            try
            {
                var lines = File.ReadAllLines(errLogLocation);
                var reveresed = new string[lines.Length - 1];

                var ctr = 0;
                for (var i = lines.Length - 1; i > 0; i--)
                {
                    reveresed[ctr] = lines[i];
                    ctr++;
                }

                foreach (var s in reveresed)
                {
                    try
                    {
                        var subStr = s.Split("\t".ToCharArray());
                        bool? b = s.Contains("TimeStamp");
                        if (b == false && s != "")
                        {
                            foreach (var ss in subStr)
                            {
                                if (ss != "")
                                {
                                    var timeAndDate = ss.Substring(0, 19).TrimStart(" \t".ToCharArray());
                                    var errorCode = ss.Substring(21, 4).TrimStart(" \t".ToCharArray());
                                    var desc = ss.Substring(26).TrimStart(" \t".ToCharArray());
                                    WarningLog.Add(new MachineErrorLog(errorCode, desc, timeAndDate));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void setPlayedLog()
        {
            for (var i = 0; i < 10; i++)
            {
                PlayedGames.Add(new PlayedGame(i));
            }
            PlayedGames.BubbleSort();
        }

        public void setWinningLog()
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    if (BoLib.getWinningGame(i) > 0)
                        WinningGames.Add(new WinningGame(i));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void setHandPayLog()
        {
            try
            {
                var filename = Resources.hand_pay_log;
                if (!File.Exists(filename))
                    File.Create(filename);

                using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var tokens = line.Split("-\t".ToCharArray());
                        HandPayLogs.Add(new HandPayLog(tokens[0] + " " + tokens[1], tokens[2]));
                    }
                }
            }
            catch (Exception ex) // come back to this and pass message back to the user.
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void setCashlessLibLog()
        {
            try
            {
                var filename = Resources.cashless_log + "nf";
                using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                }
            }
            catch (Exception ex)
            {
                var box = new WPFMessageBoxService();
                box.ShowMessage(ex.Message, "Exception Caught");
            }
        }

        #region Properties

        public bool IsLoaded { get; set; }

        public ObservableCollection<MachineErrorLog> ErrorLog
        {
            get { return m_errorLog; }
        }

        public ObservableCollection<WinningGame> WinningGames
        {
            get { return m_winningGames; }
        }

        public ObservableCollection<PlayedGame> PlayedGames
        {
            get { return m_playedGames; }
        }

        public ObservableCollection<MachineErrorLog> WarningLog
        {
            get { return m_warningLog; }
        }

        public ObservableCollection<HandPayLog> HandPayLogs
        {
            get { return m_handPayLog; }
        }

        public ObservableCollection<CashlessLibLog> CashLess
        {
            get { return m_cashLess; }
        }

        #endregion
    }
}