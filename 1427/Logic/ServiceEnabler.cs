using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PDTUtils.Logic
{
	public class ServiceEnabler : INotifyPropertyChanged
	{
		Dictionary<string, bool> m_categories = new Dictionary<string, bool>();

		#region Properties
		public bool GameStatistics
		{
			get { return m_categories["GameStatistics"]; }
			set 
			{
				IterateCategory("GameStatistics");
				//m_categories["GameStatistics"] = value;
				//this.OnPropertyChanged("GameStatistics");
			}
		}

		public bool MachineIni
		{
			get { return m_categories["MachineIni"]; }
			set 
			{
				IterateCategory("MachineIni");
				//m_categories["MachineIni"] = value;
				//this.OnPropertyChanged("MachineIni");
			}
		}

		public bool Volume
		{
			get { return m_categories["Volume"]; }
			set 
			{
				IterateCategory("Volume");
				//m_categories["Volume"] = value;
				//this.OnPropertyChanged("Volume");
			}
		}

		#endregion

		public ServiceEnabler()
		{
			m_categories.Add("GameStatistics", false);
			this.OnPropertyChanged("GameStatistics");
			m_categories.Add("MachineIni", false);
			this.OnPropertyChanged("MachineIni");
			m_categories.Add("Volume", false);
			this.OnPropertyChanged("Volume");
		}

		#region Property Changed
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		void IterateCategory(string name)
		{
			var keys = new List<string>(m_categories.Keys);	
			foreach(string key in keys)
			{
				if (key != name)
					m_categories[key] = false;
				else
					m_categories[key] = true;
				this.OnPropertyChanged(key);
			
			}
		}
	}
}
