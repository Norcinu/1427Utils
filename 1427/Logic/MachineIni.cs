using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace PDTUtils.Logic
{
	public class IniElement
	{
		private string _category;
		private string _field;
		private string _value;

		#region Properties
		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string Field
		{
			get { return _field; }
			set { _field = value; }
		}
		

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}
		#endregion
		
		public IniElement(string category, string field, string value)
		{
			_category = category;
			_field = field;
			_value = value;
		}
	}

	// need to check for duplicates, remove etc...
	public class UniqueIniCategory : ObservableCollection<IniElement>
	{ 
		private List<string> uniqueEntries = new List<string>();
		public UniqueIniCategory()
		{
		}

		public void Find(MachineIni ini)
		{
			foreach (IniElement i in ini.GetItems)
			{
				if (!uniqueEntries.Contains(i.Category))
				{
					uniqueEntries.Add(i.Category);
					Add(i);
				}
			}
		}
	}

	/// <summary>
	/// Represents the machine ini of the cabinet.
	/// </summary>
	public class MachineIni : ObservableCollection<IniElement>
	{
		static readonly string IniPath = "Y:\\machine\\machine.ini";
		static readonly string EndOfIni = "[END]";
		Dictionary<string, string> iniVariables = new Dictionary<string, string>();

		public MachineIni()
		{
		}


		#region Properties

		public string this[string key]
		{
			get { return iniVariables[key]; }
		}

		public string GetIniValue(string key)
		{
			return iniVariables[key];
		}

		public IList<IniElement> GetItems
		{
			get { return Items; }
		}
		#endregion

		public void Save()
		{

		}
		

		/// <summary>
		/// Read Machine and parse accordingly.
		/// </summary>
		/// <returns></returns>
		public bool ParseIni()
		{
			using (FileStream fs = File.Open(IniPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (BufferedStream bs = new BufferedStream(fs))
			using (StreamReader sr = new StreamReader(bs))
			{
				string line;
				string category = "";
				while ((line = sr.ReadLine()) != null)
				{
					if (line.Equals(EndOfIni))
						break;
					else if (line.StartsWith("#") /*|| line.StartsWith("[")*/ || line.Equals(""))
					{
					}
					else if (line.StartsWith("["))
					{
						category = line.Trim("[]".ToCharArray());
						category += ".";
					}
					else
					{
						if (line.Contains("="))
						{
							var options = line.Split("=".ToCharArray());
						//	iniVariables.Add(category + options[0], options[1]);
							Add(new IniElement(category, options[0], options[1]));
						}
						else if(line != null || line != "")
						{
							//iniVariables.Add(line, line);
							Add(new IniElement(category, line, line));
						}
					}
				}
			}

			return true;
		}
	}
}
