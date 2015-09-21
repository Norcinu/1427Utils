using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Xml;

namespace PDTUtils.MVVM.ViewModels
{
    class RouletteBettingViewModel : ObservableObject
    {
        readonly string _betValues = @"D:\2001\BetValues.xml";
        /*readonly string _rootNode = "betvalues";
        readonly string _name = "name";
        readonly string _min = "min";
        readonly string _max = "max";*/
        
        
        Dictionary<string, KeyValuePair<int, int>> _betInfo = new Dictionary<string, KeyValuePair<int, int>>();
        
        
        public RouletteBettingViewModel()
        {
            ParseFile();
        }
        //literally I am an insult to the bass and bass players.
        //pretty cool guitar solo imo.
        void ParseFile()
        {
            try
            {
                using (var xml = XmlReader.Create(_betValues))
                {
                    string name = "";
                    string[] attribute = new string[2];
                    int count = 0;
                    int length = 3;
                    
                    while (xml.Read())
                    {
                        switch (xml.NodeType)
                        {
                            case XmlNodeType.Element:
                                name = xml.Name;
                                count++;
                                break;
                            
                            case XmlNodeType.Text:
                                if (name == "min")
                                    attribute[0] = xml.Value.Trim("\t\n\r".ToCharArray());
                                else if (name == "max")
                                    attribute[1] = xml.Value.Trim("\t\n\r".ToCharArray());
                                
                                count++;
                                break;

                            case XmlNodeType.EndElement:
                                name = "";
                                attribute[0] = "";
                                attribute[1] = "";
                                break;
                        }
                        
                        if (count == length)
                        {
                            System.Diagnostics.Debug.WriteLine("BULLA JE FROG");
                            //NEW DICTIONARY ITEM
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var defaults = new Dictionary<string, KeyValuePair<int, int>>();
                //defaults.Add(""
                //load default values
            }
        }
        
        void Write()
        {
            //half an hour so false.
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
