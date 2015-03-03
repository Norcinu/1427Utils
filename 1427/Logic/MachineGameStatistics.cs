using System;
using System.Collections.ObjectModel;
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
			get { return _averageStake; }
			set { _averageStake = value; }
		}
		
		public string ImageSource
		{
			get;
			set;
			/*get { return _imageSource; }
			set { _imageSource = value; }*/
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
	public class MachineGameStatistics 
	{
		ObservableCollection<GameStats> _games = new ObservableCollection<GameStats>();
		string _perfLog = Resources.perf_log;
		int _moneyIn = 0;
		int _moneyOut = 0;
		int _totalBet = 0;
		int _totalWon = 0;
		int _totalGames = 0;
		int _numberOfGames = 0;
		bool _fileLoaded = false;

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
		#endregion

		public MachineGameStatistics()
		{
		}

		public void Update()
		{
		}

		private void FillGameStats(string[] combo, ref GameStats gs)
		{
			/*if (combo[0] == "GameNo")
				gs.GameNumber = Convert.ToInt32(combo[1]);
			else if (combo[0] == "ModelNo")
			{
				gs.ModelNumber = Convert.ToInt32(combo[1]);
				gs.ImageSource = @"D:\" + gs.ModelNumber.ToString() + @"\I" + gs.ModelNumber.ToString() + ".png";
			}
			else if (combo[0] == "Bets")
				gs.Bets = Convert.ToInt32(combo[1]);
			else if (combo[0] == "Wins")
				gs.Wins = Convert.ToInt32(combo[1]);
			else if (combo[0] == "Percentage")
				gs.Percentage = Convert.ToDouble(combo[1]);*/

            if (combo[0] == "GameNo")
                gs.GameNumber = Convert.ToInt32(combo[1]);
            else if (combo[0] == "ModelNo")
            {
                gs.ModelNumber = Convert.ToInt32(combo[1]);
                gs.ImageSource = @"D:\" + /*gs.ModelNumber.ToString()*/@"stats\" + /*@"\I" +*/ gs.ModelNumber.ToString() + ".png";
            }
            else if (combo[0] == "Bets")
                gs.Bets = Convert.ToInt32(combo[1]);
            else if (combo[0] == "Wins")
                gs.Wins = Convert.ToInt32(combo[1]);
            else if (combo[0] == "Percentage")
            {
                gs.Percentage = combo[1].Trim() + "%";
                //var l = combo[1].Trim();
                //gs.Percentage = decimal.Parse(l); //Convert.ToDecimal(combo[1]);
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

            var moneyInLt = BoLib.getPerformanceMeter((byte)Performance.MoneyInLt);
            var moneyOutLt = BoLib.getPerformanceMeter((byte)Performance.MoneyOutLt);
            var moneyWageredLt = BoLib.getPerformanceMeter((byte)Performance.WageredLt);
            var wonLt = BoLib.getPerformanceMeter((byte)Performance.WonLt);
            var noGames = BoLib.getPerformanceMeter((byte)Performance.NoGamesLt);

            var gameCnt = BoLib.getTerminalFormat();

            _moneyIn = (int)moneyInLt;
            _moneyOut = (int)moneyOutLt;
            _totalBet = (int)moneyWageredLt;
            _totalWon = (int)wonLt;
            _totalGames = (int)noGames;
            _numberOfGames = gameCnt + 1;

            for (var i = 0; i < gameCnt + 1; i++)
            {
                var modelNo = BoLib.getGameModel(i);
                var bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                var win = (int)BoLib.getGamePerformanceMeter((uint)i, 1);
                var perc = ((double)win / (double)bet) * 100;// *10000;
                var stats = new GameStats();
                stats.GameNumber = i;
                stats.ModelNumber = (int)modelNo;
                stats.Bets = (int)bet;
                stats.Wins = win;
                stats.Percentage = (perc > 0) ? perc.ToString() + "%" : "0.00%";
                stats.ImageSource = (modelNo == 1524) ? @"D:\1525\BMP\GameIconS.png" : @"D:\" + modelNo.ToString() + @"\BMP\GameIconS.png";
                _games.Add(stats);
            }
        }
	}
}
