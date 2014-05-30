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
			set { IterateCategory("GameStatistics"); }
		}

		public bool MachineIni
		{
			get { return m_categories["MachineIni"]; }
			set { IterateCategory("MachineIni"); }
		}

		public bool Volume
		{
			get { return m_categories["Volume"]; }
			set { IterateCategory("Volume"); }
		}

		public bool Meters
		{
			get { return m_categories["Meters"]; }
			set { IterateCategory("Meters"); }
		}

		public bool Performance
		{
			get { return m_categories["Perfomance"]; }
			set { IterateCategory("Performance"); }
		}

		public bool System
		{
			get { return m_categories["System"]; }
			set { IterateCategory("System"); }
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
			m_categories.Add("Meters", false);
			this.OnPropertyChanged("Meters");
			m_categories.Add("Performance", false);
			this.OnPropertyChanged("Performance");
			m_categories.Add("System", false);
			this.OnPropertyChanged("System");
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
