using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDTUtils.MVVM;

namespace PDTUtils.MVVM.ViewModels
{
    class GameChartViewModel : ObservableObject
    {
        List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, int>> ValueList { get { return valueList; } }
        
        public GameChartViewModel()
        {    
            valueList.Add(new KeyValuePair<string, int>("Developer", 60));
            valueList.Add(new KeyValuePair<string, int>("Misc", 20));
            valueList.Add(new KeyValuePair<string, int>("Tester", 50));
            valueList.Add(new KeyValuePair<string, int>("QA", 30));
            valueList.Add(new KeyValuePair<string, int>("Project Manager", 40));
            RaisePropertyChangedEvent("ValueList");
        }
    }
}
