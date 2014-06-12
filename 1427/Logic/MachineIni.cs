using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using PDTUtils.Native;
using System;

namespace PDTUtils.Logic
{
	public class IniElement
	{
		private string m_category;
		private string m_field;
		private string m_value;

		#region Properties
		public string Category
		{
			get { return m_category; }
			set { m_category = value; }
		}

		public string Field
		{
			get { return m_field; }
			set { m_field = value; }
		}
		
		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}
		#endregion
		
		public IniElement(string category, string field, string value)
		{
			m_category = category;
			m_field = field;
			m_value = value;
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
			/*foreach (IniElement i in ini.GetItems)
			{
				if (!uniqueEntries.Contains(i.Category))
				{
					uniqueEntries.Add(i.Category);
					Add(i);
				}
			}*/
		}
	}

	/// <summary>
	/// Represents the machine ini of the cabinet.
	/// </summary>
	public class MachineIni : ObservableCollection<IniElement>
	{
		static readonly string IniPath = "D:\\machine\\machine.ini";
		static readonly string EndOfIni = "[END]";
		Dictionary<string, string> iniVariables = new Dictionary<string, string>();
		List<string> m_field = new List<string>();
		List<string> m_values = new List<string>();
		//ObservableCollection<IniElement> m_iniElements = new ObservableCollection<IniElement>();
		
		public MachineIni()
		{
			ParseIni();
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

		/*public IList<IniElement> GetItems
		{
			get { return Items; }
		}*/
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
			using (FileStream fs = File.Open(IniPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

		public void HashMachineIni()
		{
			int retries = 10;

			if (NativeMD5.CheckFileType(IniPath) == true)
			{
				if (NativeMD5.CheckHash(IniPath) != true)
				{
					if (NativeWinApi.SetFileAttributes(IniPath, NativeWinApi.FILE_ATTRIBUTE_NORMAL))
					{
						NativeWinApi.WritePrivateProfileSection("End", null, IniPath);
						NativeWinApi.WritePrivateProfileSection("End", "", IniPath);

						do
						{
							NativeMD5.AddHashToFile(IniPath);
						} while (!NativeMD5.CheckHash(IniPath) && retries-- > 0);
					}
				}
			}
		}
	}
}
