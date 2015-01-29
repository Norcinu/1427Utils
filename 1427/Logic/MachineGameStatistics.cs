using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using PDTUtils.Native;

namespace PDTUtils
{
	public class GameStats : IComparable
	{
		int m_gameNumber;
		int m_modelNumber;
		int m_bets;
		int m_wins;
		string m_percentage;
		double m_averageStake;

		#region Properties
		public int GameNumber
		{
			get { return m_gameNumber; }
			set { m_gameNumber = value; }
		}
		
		public int ModelNumber
		{
			get { return m_modelNumber; }
			set { m_modelNumber = value; }
		}
		
		public int Bets
		{
			get { return m_bets; }
			set { m_bets = value; }
		}
		
		public int Wins
		{
			get { return m_wins; }
			set { m_wins = value; }
		}
		
		public string Percentage
		{
			get { return m_percentage; }
			set { m_percentage = value; }
		}
		
		public double AverageStake
		{
			get { return m_averageStake; }
			set { m_averageStake = value; }
		}
		
		public string ImageSource
		{
			get;
			set;
			/*get { return m_imageSource; }
			set { m_imageSource = value; }*/
		}
		#endregion
        
		public GameStats()
		{
			m_gameNumber = 0;
			m_modelNumber = 0;
			m_bets = 0;
			m_wins = 0;
			m_percentage = "";
			m_averageStake = 0.0;
			ImageSource = "";
		}

        public int CompareTo(object obj)
        {
            GameStats otherGame = obj as GameStats;
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
		ObservableCollection<GameStats> m_games = new ObservableCollection<GameStats>();
		string m_perfLog = PDTUtils.Properties.Resources.perf_log;
		int m_moneyIn = 0;
		int m_moneyOut = 0;
		int m_totalBet = 0;
		int m_totalWon = 0;
		int m_totalGames = 0;
		int m_numberOfGames = 0;
		bool m_fileLoaded = false;

		#region Properties
		public ObservableCollection<GameStats> Games
		{
			get { return m_games; }
		}

		public int LoadedGameCount
		{
			get { return m_games.Count; }
		}

		public int MoneyIn
		{
			get { return m_moneyIn; }
			set { m_moneyIn = value; }
		}

		public int MoneyOut
		{
			get { return m_moneyOut; }
			set { m_moneyOut = value; }
		}

		public int TotalBet
		{
			get { return m_totalBet; }
			set { m_totalBet = value; }
		}

		public int TotalWon
		{
			get { return m_totalWon; }
			set { m_totalWon = value; }
		}

		public int TotalGames
		{
			get { return m_totalGames; }
			set { m_totalGames = value; }
		}

		public int NumberOfGames
		{
			get { return m_numberOfGames; }
			set { m_numberOfGames = value; }
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
			m_moneyIn = 0;
			m_moneyOut = 0;
			m_totalBet = 0;
			m_totalWon = 0;
			m_totalGames = 0;
			m_numberOfGames = 0;
			m_fileLoaded = false;

			Extension.RemoveAll(m_games);
		}
		
		public void ParsePerfLog()
        {
            if (m_games.Count > 0)
                Extension.RemoveAll(m_games);

            var moneyInLt = BoLib.getPerformanceMeter((byte)Performance.MONEY_IN_LT);
            var moneyOutLt = BoLib.getPerformanceMeter((byte)Performance.MONEY_OUT_LT);
            var moneyWageredLt = BoLib.getPerformanceMeter((byte)Performance.WAGERED_LT);
            var wonLt = BoLib.getPerformanceMeter((byte)Performance.WON_LT);
            var noGames = BoLib.getPerformanceMeter((byte)Performance.NO_GAMES_LT);

            var gameCnt = BoLib.getTerminalFormat();

            m_moneyIn = (int)moneyInLt;
            m_moneyOut = (int)moneyOutLt;
            m_totalBet = (int)moneyWageredLt;
            m_totalWon = (int)wonLt;
            m_totalGames = (int)noGames;
            m_numberOfGames = (int)gameCnt + 1;

            for (int i = 0; i < gameCnt + 1; i++)
            {
                var modelNo = BoLib.getGameModel(i);
                var Bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                var Win = (int)BoLib.getGamePerformanceMeter((uint)i, 1);
                var Perc = ((double)Win / (double)Bet) * 100;// *10000;
                GameStats stats = new GameStats();
                stats.GameNumber = i;
                stats.ModelNumber = (int)modelNo;
                stats.Bets = (int)Bet;
                stats.Wins = Win;
                stats.Percentage = (Perc > 0) ? Perc.ToString() + "%" : "0.00%";
                stats.ImageSource = (modelNo == 1524) ? @"D:\1525\BMP\GameIconS.png" : @"D:\" + modelNo.ToString() + @"\BMP\GameIconS.png";
                m_games.Add(stats);
            }

            #region lockoff
            /*if (m_fileLoaded == false)
			{
				using (FileStream fs = File.Open(m_perfLog, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (BufferedStream bs = new BufferedStream(fs))
				using (StreamReader sr = new StreamReader(bs))
				{
					int gameCounter = 0;
					int fieldCounter = 0;
					bool isGeneral = true;
					int generalFields = 6;
					var generalValues = new string[generalFields];
					string line = "";
                    var sorted = new List<GameStats>();
					while ((line = sr.ReadLine()) != null)
					{
						if (isGeneral == true)
						{
							if (line.StartsWith("[") == false)
							{
								var splitLine = line.Split("=".ToCharArray());
								generalValues[fieldCounter] = splitLine[1];
								fieldCounter++;
                           //     if (splitLine[0] == "GameCnt")
                           //         gameCounter = Convert.ToInt32(splitLine[1]);

							}
						}
						else
						{
                            if (line.StartsWith("[") == false)
                            {
                                if (gameCounter < Convert.ToInt32(generalValues[5]))
                                {
                                    var outter = sorted[gameCounter];//m_games[gameCounter];
                                    line = line.Trim();
                                    FillGameStats(line.Split("=".ToCharArray()), ref outter);
                                }
							}
							else
							{
								if (fieldCounter != 0)
									gameCounter++;
								fieldCounter++;
							}
						}
                        
						if (fieldCounter >= generalFields && isGeneral == true)
						{
							isGeneral = false;
							fieldCounter = 0;
							for (int i = 0; i < Convert.ToInt32(generalValues[5]); i++)
							{
                                sorted.Add(new GameStats());
							}
						}
					}
                    
                    sorted.Sort();
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        m_games.Add(sorted[i]);
                    }

					m_fileLoaded = true;
					m_moneyIn = Convert.ToInt32(generalValues[0]);
					m_moneyOut = Convert.ToInt32(generalValues[1]);
					m_totalBet = Convert.ToInt32(generalValues[2]);
					m_totalWon = Convert.ToInt32(generalValues[3]);
					m_totalGames = Convert.ToInt32(generalValues[4]);
					m_numberOfGames = Convert.ToInt32(generalValues[5]);
				}
			}*/
            #endregion
        }
	}
}
