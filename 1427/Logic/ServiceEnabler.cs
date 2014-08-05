using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using PDTUtils.Properties;

namespace PDTUtils.Logic
{
	public class ServiceEnabler : INotifyPropertyChanged
	{
		Dictionary<string, bool> m_categories = new Dictionary<string, bool>();

		#region Properties
		public bool GameStatistics
		{
			get { return m_categories[Categories.GameStatistics]; }
		}

		public bool MachineIni
		{
			get { return m_categories[Categories.MachineIni]; }
		}

		public bool Volume
		{
			get { return m_categories[Categories.Volume]; }
		}

		public bool Meters
		{
			get { return m_categories[Categories.Meters]; }
		}

		public bool Performance
		{
			get { return m_categories[Categories.Performance]; }
		}

		public bool System
		{
			get { return m_categories[Categories.System]; }
		}

		public bool Setup
		{
			get { return m_categories[Categories.Setup]; }
		}

		public bool Logfile
		{
			get { return m_categories[Categories.Logfile]; }
		}

		#endregion

		public ServiceEnabler()
		{
			m_categories.Add(Categories.GameStatistics, false);
			this.OnPropertyChanged(Categories.GameStatistics);
			m_categories.Add(Categories.MachineIni, false);
			this.OnPropertyChanged(Categories.MachineIni);
			m_categories.Add(Categories.Volume, false);
			this.OnPropertyChanged(Categories.Volume);
			m_categories.Add(Categories.Meters, false);
			this.OnPropertyChanged(Categories.Meters);
			m_categories.Add(Categories.Performance, false);
			this.OnPropertyChanged(Categories.Performance);
			m_categories.Add(Categories.System, false);
			this.OnPropertyChanged(Categories.System);
			m_categories.Add(Categories.Setup, false);
			this.OnPropertyChanged(Categories.Setup);
			m_categories.Add(Categories.Logfile, false);
			this.OnPropertyChanged(Categories.Logfile);
		}

		#region Property Changed
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		/// <summary>
		/// Sets the named category to active, sets everything else to inactive
		/// </summary>
		/// <param name="name">Name of the key to enable</param>
		public void EnableCategory(string name)
		{
			if (m_categories.Keys.Count == 0 || name == "")
				return;
			ClearAll();
			m_categories[name] = true;
			this.OnPropertyChanged(name);
		}

		public void ClearAll()
		{
			var keys = new List<string>(m_categories.Keys);
			foreach (var key in keys)
			{
				m_categories[key] = false;
				this.OnPropertyChanged(key);
			}
		}
	}
}
