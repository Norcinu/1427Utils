﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using PDTUtils.Native;

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
    
	//  need to check for duplicates, remove etc...
	public class UniqueIniCategory : ObservableCollection<IniElement>
	{ 
		private List<string> uniqueEntries = new List<string>();
		public UniqueIniCategory()
		{
		}
	}
    
    /// <summary>
	/// Represents the machine ini of the cabinet.
	/// </summary>
	public class MachineIni : ObservableCollection<IniElement>
	{
		static readonly string IniPath = "D:\\machine\\machine.ini";
		static readonly string EndOfIni = "[END]";
        public bool ChangesPending { get; set; }
        
        List<IniElement> _models = new List<IniElement>();
        string _firstLine = "";

        Dictionary<string, string> iniVariables = new Dictionary<string, string>();
        List<string> m_field = new List<string>();
        List<string> m_values = new List<string>();

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
                string category = null;
                try
                {
                    char[] first = new char[10];
                	sr.Read(first, 0, 7);
                    _firstLine = new string(first).Trim('\0');
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);	
                }
                
                
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Equals(EndOfIni))
                        break;
                    else if (line.StartsWith("[") && LineContainsCategories(line) && line.Equals("") != true)
                    {
                        category = line.Trim("[]".ToCharArray());
                        
                        string[] str;
                        IniFileUtility.GetIniProfileSection(out str, category, @"D:\machine\machine.ini");
                        if (str != null)
                        {
                            foreach (var val in str)
                            {
                                if (val.Contains("="))
                                {
                                    var options = val.Split("=".ToCharArray());
                                    Add(new IniElement(category, options[0], options[1]));
                                }
                            }
                        }
                    }
                }
            }

            string[] models;
            IniFileUtility.GetIniProfileSection(out models, "Models", IniPath);
            foreach (var m in models)
            {
                _models.Add(new IniElement("Models", m, ""));
            }

            return true; 
        }
        
        private static bool LineContainsCategories(string line)
        {
            return (line.Contains("Game")   != true && line.Contains("Select")  != true && 
                    line.Contains("Models") != true && line.Contains("Standby") != true);
        }
        
        public void WriteMachineIni()
        {
            //if (File.Exists(IniPath))
            //{
                //  File.Move(IniPath, IniPath + "_old");
                //  File.Create(IniPath);
                string divider = "Models";
                using (StreamWriter w = File.CreateText(IniPath))
                {
                    w.WriteLine(_firstLine);
                    w.WriteLine("[" + divider + "]");
                    foreach (var m in _models)
                        w.WriteLine(m.Field);
                    w.Flush();
                }
            //}

            foreach (var item in Items)
            {
                if (item.Category != divider)
                {
                    divider = item.Category;
                    using (StreamWriter w = File.AppendText(IniPath))
                    {
                        w.WriteLine("\r\n");
                        w.Flush();
                    }
                }
                NativeWinApi.WritePrivateProfileString(item.Category, item.Field, item.Value, IniPath);
            }
        
            HashMachineIni();
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
