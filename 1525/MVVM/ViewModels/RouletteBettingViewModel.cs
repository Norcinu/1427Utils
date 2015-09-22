using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Xml;

namespace PDTUtils.MVVM.ViewModels
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    }

    class RouletteBettingViewModel : ObservableObject
    {
        int _selectedIndex = -1;
        
        readonly string _betValues = @"D:\2001\BetValues.xml";
        Dictionary<string, Pair<int, int>> _betInfo = new Dictionary<string, Pair<int, int>>();
        List<string> _names = new List<string>();

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChangedEvent("SelectedIndex");
                RaisePropertyChangedEvent("SelectedName");
                RaisePropertyChangedEvent("SelectedMin");
                RaisePropertyChangedEvent("SelectedMax");
            }
        }

        public string SelectedName
        {
            get
            {
                if (_selectedIndex >= 0)
                    return _names[_selectedIndex];
                else
                    return "";
            }
        }

        public int SelectedMin
        {
            get
            {
                if (_selectedIndex >= 0)
                    return _betInfo[_names[_selectedIndex].Split(":".ToCharArray())[0]].First;
                else
                    return -1;
            }
            set
            {
                _betInfo[_names[_selectedIndex]].First = value;
            }
        }

        public int SelectedMax
        {
            get 
            {
                if (_selectedIndex >= 0)
                    return _betInfo[_names[_selectedIndex].Split(":".ToCharArray())[0]].Second;
                else
                    return -1;
            }
            set
            {
                if (_selectedIndex >= 0)
                    _betInfo[_names[_selectedIndex].Split(":".ToCharArray())[0]].Second = value;
            }
        }

        public List<string> Names { get { return _names; } }

        public Dictionary<string, Pair<int, int>> BetInfo
        {
            get { return _betInfo; }
        }
        
        public RouletteBettingViewModel()
        {
            ParseFile();
        }
        
        void ParseFile()
        {
            try
            {
                using (var xml = XmlReader.Create(_betValues))
                {
                    string name = "";
                    string[] attribute = new string[2];
                    int count = 0;
                    int length = 2;
                    
                    while (xml.Read())
                    {
                        if (xml.HasAttributes)
                        {
                            string attr = "";
                            if (xml.Name == "betvalue") 
                                attr = "update";
                            else if (xml.Name == "betvalues") 
                                attr = "update";
                            else if (xml.Name == "bet")
                            {
                                attr = "name";
                                name = xml.GetAttribute(attr);
                            }
                            else if (xml.Name == "min")
                            {
                                attr = "value";
                                attribute[0] = xml.GetAttribute(attr);
                                count++;
                            }
                            else if (xml.Name == "max")
                            {
                                attr = "value";
                                attribute[1] = xml.GetAttribute(attr);
                                count++;
                            }
                        }
                        
                        if (count == length)
                        {
                            if (!name.Equals("rose"))
                            {
                                try
                                {
                                    if (Convert.ToInt32(name) > 0)
                                        _names.Add(name + ":1");
                                    else
                                        _names.Add(name);
                                }
                                catch (Exception e)
                                {
                                    _names.Add(name);
                                }

                                _betInfo.Add(name, new Pair<int, int>(Convert.ToInt32(attribute[0]), Convert.ToInt32(attribute[1])));
                            }

                            count = 0;
                            attribute[0] = "";
                            attribute[1] = "";
                            name = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not load Roulette bets");
            }

            RaisePropertyChangedEvent("BetInfo");
            RaisePropertyChangedEvent("Names");
        }
        
        void Write()
        {
        }

        public ICommand Increment
        {
            get { return new DelegateCommand(DoIncrement); }
        }
        void DoIncrement(object o)
        {

        }

        public ICommand Decrement
        {
            get { return new DelegateCommand(DoDecrement); }
        }
        void DoDecrement(object o)
        {

        }

    }
}
