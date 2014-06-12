using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using PDTUtils.Native;

namespace PDTUtils
{
	abstract public class BaseNotifyPropertyChanged : INotifyPropertyChanged
	{
		public string GameDate 
		{
			get { return logDate.ToString(); }
		}

		public DateTime LogDate { get; set; }
		public uint Stake { get; set; }
		public uint Credit { get; set; }
		public uint GameModel { get; set; }

		#region Private Variables
		public DateTime logDate;
		#endregion

		public BaseNotifyPropertyChanged()
		{
			CultureInfo ci = new CultureInfo("es-ES"); // en-GB
			/*string date = DateTime.Now.ToString();
			DateTime.TryParse(date, ci, DateTimeStyles.None, out logDate);*/
			Stake = 0;
			Credit = 0;
		}

		public abstract void ParseGame(int gameNo);

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}

	public class WinningGame : BaseNotifyPropertyChanged
	{
		public uint WinAmount { get; set; }
		public WinningGame(int gameNo)
		{
			ParseGame(gameNo);
		}

		public override void ParseGame(int gameNo)
		{
			CultureInfo ci = new CultureInfo("es-ES"); // en-GB
			string date = DateTime.Now.ToString();
			DateTime.TryParse(date, ci, DateTimeStyles.None, out logDate);

			int counter = 0;
			GameModel = BoLib.getGameModel(gameNo);
			var today = //DateTime.Today;
			counter++;
			var gameDate = BoLib.getGameDate(gameNo);
			LogDate = new DateTime(2014, (int)(gameDate & 0x0000FFFF), (int)(gameDate >> 16));
			WinAmount = BoLib.getWinningGame(gameNo);
			this.OnPropertyChanged("WinningGames");
		}
	}

	public class PlayedGame : BaseNotifyPropertyChanged
	{
		public uint WinAmount { get; set; }
		public uint Credits { get; set; }

		public PlayedGame()
		{

		}

		public PlayedGame(int gameNo)
		{
			ParseGame(gameNo);
		}

		public override void ParseGame(int gameNo)
		{
			int counter = 0;
			CultureInfo ci = new CultureInfo("es-ES"); // en-GB
			string date = DateTime.Now.ToString();
			DateTime.TryParse(date, ci, DateTimeStyles.None, out logDate);

			var today = DateTime.Today;
			counter++;
			var gameDate = BoLib.getGameDate(gameNo);
			var time = BoLib.getGameTime(gameNo);

			var hour = time & 0x0000FFFF;
			var minute = time >> 16;

			var month = gameDate & 0x0000FFFF;
			var day = gameDate >> 16;
			var year = DateTime.Now.Year;
			if (month > DateTime.Now.Month)
				--year;
			
			Credits = BoLib.getGameCreditLevel(gameNo);
			GameModel = BoLib.getGameModel(gameNo);
			LogDate = new DateTime(year, (int)month, (int)day, (int)hour, (int)minute, 0);
			
			WinAmount = BoLib.getWinningGame(gameNo);
			this.OnPropertyChanged("PlayedGames");
		}
	}

	public class MachineErrorLog : BaseNotifyPropertyChanged
	{
		public int ErrorCode { get; set; }
		public MachineErrorLog()
		{
			this.ErrorCode = 10;
			this.OnPropertyChanged("ErrorCode");
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
			ErrorLog.Add(new MachineErrorLog());
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
