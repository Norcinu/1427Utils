﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
	public class GameStats : IComparable
	{
		int _gameNumber;
		int _modelNumber;
		int _bets;
		int _wins;
		string _percentage;
		double _averageStake;

		#region Properties
		public int GameNumber
		{
			get { return _gameNumber; }
			set { _gameNumber = value; }
		}
		
		public int ModelNumber
		{
			get { return _modelNumber; }
			set { _modelNumber = value; }
		}
		
		public int Bets
		{
			get { return _bets; }
			set { _bets = value; }
		}
		
		public int Wins
		{
			get { return _wins; }
			set { _wins = value; }
		}
		
		public string Percentage
		{
			get { return _percentage; }
			set { _percentage = value; }
		}
		
		public double AverageStake
		{
            get { return Math.Round(_averageStake, 2); }
			set { _averageStake = value; }
		}
            		
		public string ImageSource
		{
			get;
			set;
		}
		#endregion
        
		public GameStats()
		{
			_gameNumber = 0;
			_modelNumber = 0;
			_bets = 0;
			_wins = 0;
			_percentage = "";
			_averageStake = 0.0;
			ImageSource = "";
		}
        
        public int CompareTo(object obj)
        {
            var otherGame = obj as GameStats;
            if (otherGame == null)
            {
                throw new ArgumentException("Object is not GameStats");
            }
            return this.GameNumber.CompareTo(otherGame.GameNumber);
        }
	}
    
	/// <summary>
	/// Gets the information regarding the ranking of games for the 
	/// statistics screen.
	/// </summary>
	public class MachineGameStatistics : INotifyPropertyChanged
	{
		ObservableCollection<GameStats> _games = new ObservableCollection<GameStats>();
        bool _fileLoaded = false;
        bool _firstPass = true;
		int _moneyIn = 0;
		int _moneyOut = 0;
		int _totalBet = 0;
		int _totalWon = 0;
		int _totalGames = 0;
		int _numberOfGames = 0;
        decimal _machineRtp = 0.00M;
        string _perfLog = Resources.perf_log;
        string _avgStakeMsg = "Average Stake ";
		
        #region Properties
		public ObservableCollection<GameStats> Games
		{
			get { return _games; }
		}

		public int LoadedGameCount
		{
			get { return _games.Count; }
		}

		public int MoneyIn
		{
			get { return _moneyIn; }
			set { _moneyIn = value; }
		}

		public int MoneyOut
		{
			get { return _moneyOut; }
			set { _moneyOut = value; }
		}

		public int TotalBet
		{
			get { return _totalBet; }
			set { _totalBet = value; }
		}

		public int TotalWon
		{
			get { return _totalWon; }
			set { _totalWon = value; }
		}
        
		public int TotalGames
		{
			get { return _totalGames; }
			set { _totalGames = value; }
		}
        
		public int NumberOfGames
		{
			get { return _numberOfGames; }
			set { _numberOfGames = value; }
		}
        
        public string MachineRtp
        {
            get
            {
                return "Overall Game Machine Average: " + Math.Round(_machineRtp, 4).ToString("P");
            }
        }

        public string AvgStakeMsg
        {
            get { return _avgStakeMsg; }
            set
            {
                _avgStakeMsg = value;
                RaisePropertyChangedEvent("AvgStakeMsg");
            }
        }
		#endregion
        
		public MachineGameStatistics()
		{
		}
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

		public void Update()
		{
		}
        
		private void FillGameStats(string[] combo, ref GameStats gs)
		{
            if (combo[0] == "GameNo")
                gs.GameNumber = Convert.ToInt32(combo[1]);
            else if (combo[0] == "ModelNo")
            {
                gs.ModelNumber = Convert.ToInt32(combo[1]);
                gs.ImageSource = @"D:\" + @"stats\" + gs.ModelNumber.ToString() + ".png";
            }
            else if (combo[0] == "Bets")
                gs.Bets = Convert.ToInt32(combo[1]);
            else if (combo[0] == "Wins")
                gs.Wins = Convert.ToInt32(combo[1]);
            else if (combo[0] == "Percentage")
            {
                gs.Percentage = combo[1].Trim() + "%";
            }
		}
		
		public void ResetStats()
		{
			_moneyIn = 0;
			_moneyOut = 0;
			_totalBet = 0;
			_totalWon = 0;
			_totalGames = 0;
			_numberOfGames = 0;
			_fileLoaded = false;
            
			_games.RemoveAll();
		}
        
		public void ParsePerfLog()
        {
            if (_games.Count > 0)
                _games.RemoveAll();

            if (_firstPass)
            {
                AvgStakeMsg += ((BoLib.getCountryCode() == BoLib.getSpainCountryCode()) ? "(¢)" : "(P)").ToString();
                _firstPass = false;
            }
            var moneyInLt = BoLib.getPerformanceMeter((byte)Performance.MoneyInLt);
            var moneyOutLt = BoLib.getPerformanceMeter((byte)Performance.MoneyOutLt);
            var moneyWageredLt = BoLib.getPerformanceMeter((byte)Performance.WageredLt);
            var wonLt = BoLib.getPerformanceMeter((byte)Performance.WonLt);
            var noGames = BoLib.getPerformanceMeter((byte)Performance.NoGamesLt);
            var gameCount = BoLib.getTerminalFormat();
            
            _moneyIn = (int)moneyInLt;
            _moneyOut = (int)moneyOutLt;
            _totalBet = (int)moneyWageredLt;
            _totalWon = (int)wonLt;
            _totalGames = (int)noGames;
            _numberOfGames = gameCount + 1;
            
            uint tempTotalWon = 0;
            uint totalGameCount = 0;
            uint totalBet = 0;
            for (var i = 0; i <= gameCount; i++)
            {
                var modelNo = BoLib.getGameModel(i);
                var bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                var win = (uint)BoLib.getGamePerformanceMeter((uint)i, 1);
                var playCount = (uint)BoLib.getGamePerformanceMeter((uint)i, 2);
                var average = (double)bet / (double)playCount;
                var perc = 0.00M;

                if (win > 0 && bet > 0)
                    perc = ((decimal)win / (decimal)bet) * 100;

                if (i > 0)
                {
                    totalGameCount += playCount;
                    tempTotalWon += win;
                    totalBet += bet;
                }

                _games.Add(new GameStats()
                {
                    GameNumber = i + 1,
                    ModelNumber = (int)modelNo,
                    Bets = (int)bet,
                    Wins = (int)win,
                    Percentage = (perc > 0) ? Math.Round(perc, 2).ToString() + "%" : "0.00%",
                    ImageSource = (modelNo == 1524) ? Properties.Resources.PreGambleIcon
                                                    : @"D:\" + modelNo.ToString() + @"\BMP\GameIconS.png",
                    AverageStake = average,
                });
            }
            
            try
            {
                _totalWon = (int)tempTotalWon;
                _machineRtp = (decimal)_totalWon / (decimal)totalBet;
                RaisePropertyChangedEvent("MachineRtp");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _machineRtp = 0.00M;
                RaisePropertyChangedEvent("MachineRtp");
            }
        }
	}
}
