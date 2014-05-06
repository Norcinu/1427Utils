using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDTUtils
{
	class GameStats
	{
		//int GamesPlayed;
		int gameNumber;
		int modelNumber;
		int bets;
		int wins;
		int Percentage;
		double averageStake;

		public GameStats()
		{

		}
	}

	/// <summary>
	/// Gets the information regarding the ranking of games for the 
	/// statistics screen.
	/// </summary>
	class MachineGameStatistics
	{
		List<GameStats> m_games = new List<GameStats>();
		const string m_perfLog = Properties.Resources.perf_log;
		int m_moneyIn = 0;
		int m_moneyOut = 0;
		int m_totalBet = 0;
		int m_totalWon = 0;
		int m_totalGames = 0;
		int m_numberOfGames = 0;

		public MachineGameStatistics()
		{
		}

		public void Update()
		{
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
				int generalFields = 7;
				var generalValues = new string[generalFields];
				string line = "";
				while ((line = sr.ReadLine()) != null)
				{
					if (isGeneral == true)
					{
						if (line.StartsWith("[") == false)
						{
							var splitLine = line.Split("=".ToCharArray());
							generalValues[gameCounter] = splitLine[2];
						}
						else
							fieldCounter++;

						if (gameCounter >= generalFields)
							isGeneral = false;
					}
					else
					{
						if (line.StartsWith("[") == false)
						{
							gameCounter++;
						}
						//gameCounter++;
					}
				}
				m_moneyIn = Convert.ToInt32(generalValues[1]);
				m_moneyOut = Convert.ToInt32(generalValues[2]);
				m_totalBet = Convert.ToInt32(generalValues[3]);
				m_totalWon = Convert.ToInt32(generalValues[4]);
				m_totalGames = Convert.ToInt32(generalValues[5]);
				m_numberOfGames = Convert.ToInt32(generalValues[6]);
			}
		}
	}
}
