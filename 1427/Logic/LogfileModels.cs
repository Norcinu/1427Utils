using System;
using System.Collections.ObjectModel;
using System.Globalization;
using PDTUtils.Native;
using System.ComponentModel;

namespace PDTUtils
{
	abstract public class BaseNotifyPropertyChanged : INotifyPropertyChanged
	{
		public BaseNotifyPropertyChanged()
		{

		}

		public abstract void ParseGame(int gameNo);

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}

	public abstract class BaseGameLog : BaseNotifyPropertyChanged
	{
		public string GameDate
		{
			get { return logDate.ToString("dd/MM/yyyy HH:mm"); }
		}

		public string LogDate { get { return logDate.ToString("dd/MM/yyyy HH:mm"); } }
		public string Stake { get { return (stake / 100m).ToString("c2"); } }
		public string Credit { get { return (credit / 100m).ToString("c2"); } }
		public uint GameModel { get; set; }

		#region Private Variables
		public DateTime logDate;
		public decimal credit;
		public decimal stake;
		#endregion

		public BaseGameLog()
		{
			CultureInfo ci = new CultureInfo("es-ES");
			stake = 0;
			credit = 0;
		}
	}

	public class WinningGame : BaseGameLog
	{
		public string WinAmount { get { return (winAmount / 100m).ToString("c2"); } }

		Decimal winAmount = 0;
		public WinningGame(int gameNo)
		{
			ParseGame(gameNo);
		}

		public override void ParseGame(int gameNo)
		{
			CultureInfo ci = new CultureInfo("en-GB"); // en-GB
			string date = DateTime.Now.ToString();
			DateTime.TryParse(date, ci, DateTimeStyles.None, out logDate);

			GameModel = BoLib.getGameModel(gameNo);
			stake = BoLib.getGameWager(gameNo);
			var tempGameDate = BoLib.getGameDate(gameNo);
			var today = (int)tempGameDate >> 16;
			var month = (int)tempGameDate & 0x0000FFFF;
			logDate = new DateTime(2014, month, today);
			winAmount = BoLib.getWinningGame(gameNo);
			credit = BoLib.getGameCreditLevel(gameNo);
			this.OnPropertyChanged("WinningGames");
		}
	}

	public class PlayedGame : BaseGameLog
	{
		public string WinAmount { get { return (winAmount / 100).ToString("c2"); } }
		private decimal winAmount;

		public PlayedGame()
		{

		}

		public PlayedGame(int gameNo)
		{
			ParseGame(gameNo);
		}

		public override void ParseGame(int gameNo)
		{
			CultureInfo ci = new CultureInfo("en-GB"); // en-GB

			var today = DateTime.Today;
			var gameDate = BoLib.getGameDate(gameNo);
			var time = BoLib.getGameTime(gameNo);

			var hour = time & 0x0000FFFF;
			var minute = time >> 16;

			var month = gameDate & 0x0000FFFF;
			var day = gameDate >> 16;
			var year = DateTime.Now.Year;
			if (month > DateTime.Now.Month)
				--year;

			string ds = day + @"/" + month + @"/" + year + " " + hour + " " + ":" + minute;
			credit = BoLib.getGameCreditLevel(gameNo);
			stake = BoLib.getGameWager(gameNo);
			GameModel = BoLib.getGameModel(gameNo);
			logDate = DateTime.Parse(ds, ci);
			winAmount = BoLib.getWinningGame(gameNo);
			this.OnPropertyChanged("PlayedGames");
		}
	}

	public class MachineErrorLog : BaseGameLog
	{
		public string ErrorCode { get; set; }
		public string Description { get; set; }
		public String ErrorDate { get; set; }

		public MachineErrorLog()
		{
			this.OnPropertyChanged("ErrorLog");
		}

		public MachineErrorLog(string code, string desciption, string date)
		{
			this.ErrorCode = code;
			this.Description = desciption;
			this.ErrorDate = date;
			this.OnPropertyChanged("ErrorLog");
		}

		public override void ParseGame(int gameNo)
		{
			throw new NotImplementedException();
		}
	}

	public class MachineLogsController
	{
		ObservableCollection<MachineErrorLog> m_errorLog = new ObservableCollection<MachineErrorLog>();
		ObservableCollection<WinningGame> m_winningGames = new ObservableCollection<WinningGame>();
		ObservableCollection<PlayedGame> m_playedGames = new ObservableCollection<PlayedGame>();
		
		public MachineLogsController()
		{			
		}

		#region Properties
		public ObservableCollection<MachineErrorLog> ErrorLog { get { return m_errorLog; } }
		public ObservableCollection<WinningGame> WinningGames { get { return m_winningGames; } }
		public ObservableCollection<PlayedGame> PlayedGames { get { return m_playedGames; } }
		#endregion

		public void setEerrorLog()
		{
			string errLogLocation = @"D:\machine\GAME_DATA\TerminalErrLog.log";
			try
			{
				string[] lines = System.IO.File.ReadAllLines(errLogLocation);
				string[] reveresed = new string[lines.Length - 1];

				int ctr = 0;
				for (int i = lines.Length - 1; i > 0; i--)
				{
					reveresed[ctr] = lines[i];
					ctr++;
				}

				foreach (string s in reveresed)
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
									var errorCode = ss.Substring(21, 3).TrimStart(" \t".ToCharArray());
									var desc = ss.Substring(26).TrimStart(" \t".ToCharArray());
									ErrorLog.Add(new MachineErrorLog(errorCode, desc, timeAndDate));
								}
							}
						}
					}
					catch (System.Exception ex)
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
			for (int i = 0; i < 10; i++)
			{
				PlayedGames.Add(new PlayedGame(i));
			}
		}

		public void setWinningLog()
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					if (BoLib.getWinningGame(i) > 0)
						WinningGames.Add(new WinningGame(i));
				}
				catch (System.Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}
}
