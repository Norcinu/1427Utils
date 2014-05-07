using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;

namespace PDTUtils
{
	class GameStats
	{
		int m_gameNumber;
		int m_modelNumber;
		int m_bets;
		int m_wins;
		double m_percentage;
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
		
		public double Percentage
		{
			get { return m_percentage; }
			set { m_percentage = value; }
		}
		
		public double AverageStake
		{
			get { return m_averageStake; }
			set { m_averageStake = value; }
		}
		#endregion

		public GameStats()
		{
			m_gameNumber = 0;
			m_modelNumber = 0;
			m_bets = 0;
			m_wins = 0;
			m_percentage = 0;
			m_averageStake = 0.0;
		}
	}

	/// <summary>
	/// Gets the information regarding the ranking of games for the 
	/// statistics screen.
	/// </summary>
	class MachineGameStatistics
	{
		List<GameStats> m_games = new List<GameStats>();
		string m_perfLog = PDTUtils.Properties.Resources.perf_log;
		int m_moneyIn = 0;
		int m_moneyOut = 0;
		int m_totalBet = 0;
		int m_totalWon = 0;
		int m_totalGames = 0;
		int m_numberOfGames = 0;
		
		#region Properties
		public List<GameStats> Games
		{
			get { return m_games; }
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
			if (combo[0] == "GameNo")
				gs.GameNumber = Convert.ToInt32(combo[1]);
			else if (combo[0] == "ModelNo")
				gs.ModelNumber = Convert.ToInt32(combo[1]);
			else if (combo[0] == "Bets")
				gs.Bets = Convert.ToInt32(combo[1]);
			else if (combo[0] == "Wins")
				gs.Wins = Convert.ToInt32(combo[1]);
			else if (combo[0] == "Percentage")
				gs.Percentage = Convert.ToDouble(combo[1]);
		}

		public void ParsePerfLog()
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
				while ((line = sr.ReadLine()) != null)
				{
					if (isGeneral == true)
					{
						if (line.StartsWith("[") == false)
						{
							var splitLine = line.Split("=".ToCharArray());
							generalValues[fieldCounter] = splitLine[1];
							fieldCounter++;
						}
					}
					else
					{
						if (line.StartsWith("[") == false)
						{
							var outter = m_games[gameCounter];
							FillGameStats(line.Split("=".ToCharArray()), ref outter);
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
							m_games.Add(new GameStats());
						}
					}
				}
				m_moneyIn = Convert.ToInt32(generalValues[0]);
				m_moneyOut = Convert.ToInt32(generalValues[1]);
				m_totalBet = Convert.ToInt32(generalValues[2]);
				m_totalWon = Convert.ToInt32(generalValues[3]);
				m_totalGames = Convert.ToInt32(generalValues[4]);
				m_numberOfGames = Convert.ToInt32(generalValues[5]);
			}
		}
	}
}
