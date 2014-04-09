using System.Collections.Generic;
using System.IO;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Represents the machine ini of the cabinet.
	/// </summary>
	class MachineIni
	{
		static readonly string IniPath = "D:\\machine\\machine.ini";
		static readonly string EndOfIni = "[END]";
		Dictionary<string, string> iniVariables = new Dictionary<string, string>();

		public MachineIni()
		{
		}

		public string this[string key]
		{
			get { return iniVariables[key]; }
		}

		public string GetIniValue(string key)
		{
			return iniVariables[key];
		}
		
		public bool ParseIni()
		{
			using (FileStream fs = File.Open(IniPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (BufferedStream bs = new BufferedStream(fs))
			using (StreamReader sr = new StreamReader(bs))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					//System.Windows.Forms.MessageBox.Show(line);
					if (line.Equals(EndOfIni))
						break;
					else if (line.StartsWith("#") || line.StartsWith("[") || line.Equals(""))
					{
					}
					else
					{
						if (line.Contains("="))
						{
							var options = line.Split("=".ToCharArray());
							System.Windows.Forms.MessageBox.Show(options[0] + ":" + options[1]);
							iniVariables.Add(options[0], options[1]);
						}
						else
						{
							System.Windows.Forms.MessageBox.Show(line);
							iniVariables.Add(line, line);
						}
					}
				}
			}
			return true;
		}
	}
}
