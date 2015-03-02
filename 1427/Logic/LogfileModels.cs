﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using PDTUtils.MVVM;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected abstract void ParseGame(int gameNo);
        
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
    
    public abstract class BaseGameLog : BaseNotifyPropertyChanged, IComparable
    {
        private readonly NumberFormatInfo _nfi;
        private CultureInfo _ci = new CultureInfo("en-GB");
        
        protected BaseGameLog()
        {
            //CultureInfo ci = new CultureInfo("en-GB");//"es-ES");
            _nfi = (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();
            _nfi.CurrencySymbol = "£";
            _stake = 0;
            _credit = 0;
        }
        
        public string GameDate
        {
            get { return _logDate.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string LogDate
        {
            get { return _logDate.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string Stake
        {
            get { return (_stake/100m).ToString("c2"); }
        }

        public string Credit
        {
            get { return (_credit/100m).ToString("c2"); }
        }

        public uint GameModel { get; set; }
        
        public int CompareTo(object obj)
        {
            if (obj is PlayedGame)
            {
            }
            
            return 0;
        }

        #region Private Variables
        
        protected DateTime _logDate;
        protected decimal _credit;
        protected decimal _stake;
        
        #endregion
    }
    
    public class WinningGame : BaseGameLog
    {
        private Decimal _winAmount;

        public WinningGame(int gameNo)
        {
            ParseGame(gameNo);
        }

        public string WinAmount
        {
            get { return (_winAmount/100m).ToString("c2"); }
        }

        protected override void ParseGame(int gameNo)
        {
            var ci = new CultureInfo("en-GB"); // en-GB
            var date = DateTime.Now.ToString();
            DateTime.TryParse(date, ci, DateTimeStyles.None, out _logDate);
            
            GameModel = (uint) BoLib.getLastGameModel(gameNo); // .getGameModel(gameNo);
            _stake = BoLib.getGameWager(gameNo);
            var tempGameDate = BoLib.getGameDate(gameNo);
            var today = (int) tempGameDate >> 16;
            var month = (int) tempGameDate & 0x0000FFFF;
            _logDate = new DateTime(2014, month, today);
            _winAmount = BoLib.getWinningGame(gameNo);
            _credit = BoLib.getGameCreditLevel(gameNo);
            OnPropertyChanged("WinningGames");
        }
    }

    public class PlayedGame : BaseGameLog
    {
        private decimal _winAmount;

        public PlayedGame()
        {
            _winAmount = 0;
        }

        public PlayedGame(int gameNo)
        {
            ParseGame(gameNo);
        }

        public string WinAmount
        {
            get { return (_winAmount/100).ToString("c2"); }
        }

        protected override void ParseGame(int gameNo)
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
                _credit = BoLib.getGameCreditLevel(gameNo);
                _stake = BoLib.getGameWager(gameNo);
                GameModel = (uint) BoLib.getLastGameModel(gameNo);

                _logDate = DateTime.Parse(ds, ci);
                _winAmount = BoLib.getWinningGame(gameNo);

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

        protected override void ParseGame(int gameNo)
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

        protected override void ParseGame(int gameNo)
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

        public CashlessLibLog(string message)
        {
            Time = "";
            Message = message;
            OnPropertyChanged("CashLess");
        }

        public CashlessLibLog(string time, string msg)
        {
            Time = time;
            Message = msg;
            OnPropertyChanged("CashLess");
        }

        public string Time { get; set; }
        public string Message { get; set; }

        protected override void ParseGame(int gameNo)
        {
            throw new NotImplementedException();
        }
    }

    public class VizTechLog : BaseNotifyPropertyChanged
    {
        public string Date { get; set; }
        public string Message { get; set; }

        public VizTechLog(string date, string message)
        {
            Date = date;
            Message = message;
        }

        protected override void ParseGame(int gameNo)
        {
            throw new NotImplementedException();
        }
    }

    public class MachineLogsController
    {
        readonly ObservableCollection<CashlessLibLog> _cashLess = new ObservableCollection<CashlessLibLog>();
        readonly ObservableCollection<MachineErrorLog> _errorLog = new ObservableCollection<MachineErrorLog>();
        readonly ObservableCollection<HandPayLog> _handPayLog = new ObservableCollection<HandPayLog>();
        readonly ObservableCollection<PlayedGame> _playedGames = new ObservableCollection<PlayedGame>();
        readonly ObservableCollection<MachineErrorLog> _warningLog = new ObservableCollection<MachineErrorLog>();
        readonly ObservableCollection<WinningGame> _winningGames = new ObservableCollection<WinningGame>();
        readonly ObservableCollection<VizTechLog> _vizTechLog = new ObservableCollection<VizTechLog>();
          
        public bool AreLogsBeingViewed { get; set; }

        public MachineLogsController()
        {
            IsLoaded = false;
            AreLogsBeingViewed = false;
        }

        public void ClearAllLogs()
        {
            
        }

        public void SetErrorLog()
        {
            const string errLogLocation = @"D:\machine\GAME_DATA\TerminalErrLog.log";
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
        
        public void SetWarningLog()
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

        public void SetPlayedLog()
        {
            for (var i = 0; i < 10; i++)
            {
                PlayedGames.Add(new PlayedGame(i));
            }
            PlayedGames.BubbleSort();
        }

        public void SetWinningLog()
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
        
        public void SetHandPayLog()
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
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void SetCashlessLibLog()
        {
            try
            {
                var filename = Resources.cashless_log;
                using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        CashLess.Add(new CashlessLibLog(line));
                    }
                }
            }
            catch (Exception ex)
            {
                var box = new WpfMessageBoxService();
                box.ShowMessage(ex.Message, "Exception Caught");
            }
        }
        
        public void SetVizTechLog()
        {
            try
            {
                var filename = Resources.viz_tech_log;
                using (var sr = new StreamReader(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var split = Regex.Split(line, "ERROR");
                        VizTechLogs.Add(new VizTechLog(split[0], split[1]));
                    }
                }

            }
            catch (Exception ex)
            {
                var box = new WpfMessageBoxService();
                box.ShowMessage(ex.Message, "Exception Caught");
            }
        }

        #region Properties

        public bool IsLoaded { get; set; }

        public ObservableCollection<MachineErrorLog> ErrorLog
        {
            get { return _errorLog; }
        }

        public ObservableCollection<WinningGame> WinningGames
        {
            get { return _winningGames; }
        }

        public ObservableCollection<PlayedGame> PlayedGames
        {
            get { return _playedGames; }
        }

        public ObservableCollection<MachineErrorLog> WarningLog
        {
            get { return _warningLog; }
        }

        public ObservableCollection<HandPayLog> HandPayLogs
        {
            get { return _handPayLog; }
        }

        public ObservableCollection<CashlessLibLog> CashLess
        {
            get { return _cashLess; }
        }

        public ObservableCollection<VizTechLog> VizTechLogs
        {
            get { return _vizTechLog; }
        }

        #endregion
    }
}