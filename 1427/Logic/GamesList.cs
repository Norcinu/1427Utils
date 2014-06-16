﻿using System;
using System.Collections.Generic;
using System.Text;
using PDTUtils.Native;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PDTUtils.Logic
{
	public class GamesList : INotifyPropertyChanged
	{
		public GamesList()
		{

		}

		ObservableCollection<GamesInfo> m_gamesInfo = new ObservableCollection<GamesInfo>();
		
		public ObservableCollection<GamesInfo> GamesInfo
		{
			get { return m_gamesInfo; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public void GetGamesList()
		{
			if (m_gamesInfo.Count > 0)
				m_gamesInfo.RemoveAll();

			var numGames = BoLib.getNumberOfGames();
			for (int i = 0; i < numGames; i++)
			{
				GamesInfo g = new GamesInfo();
			
				StringBuilder sb = new StringBuilder(500);
				uint res = NativeWinApi.GetPrivateProfileString("Game" + (i + 1).ToString(), "Exe", "", sb, (uint)sb.Capacity, @"D:\machine\machine.ini");
				g.path = sb.ToString();
				var modelNo = sb.ToString().Substring(0, 4);
				g.name = @"D:\" + modelNo + @"\" + modelNo + ".png";

				if (NativeMD5.CheckHash(@"d:\" + modelNo + @"\" + sb.ToString()) == true)
				{
					var hash = NativeMD5.CalcHashFromFile(@"d:\" + modelNo + @"\" + sb.ToString());
					var hex = NativeMD5.HashToHex(hash);
					g.hash_code = hex;
				}
				else
					g.hash_code = "ERROR: NOT AUTHORISED";
			
				g.Name = g.name;
				g.Path = g.path;
				g.Hash_code = g.hash_code; 

				m_gamesInfo.Add(g);

				this.OnPropertyChanged("Name");
				this.OnPropertyChanged("Path");
				this.OnPropertyChanged("Hash_code"); // Fire dem events.
			}
		}
	}
}
