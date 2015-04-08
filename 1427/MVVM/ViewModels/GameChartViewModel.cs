using System;
using System.Collections.Generic;
using System.Linq;
using PDTUtils.Native;


namespace PDTUtils.MVVM.ViewModels
{
    class GameChartViewModel : ObservableObject
    {
        public List<KeyValuePair<string, uint>> Incomings { get; set; }
        public List<KeyValuePair<string, uint>> Outgoings { get; set; }
        
        public GameChartViewModel()
        {
            Incomings = new List<KeyValuePair<string, uint>>();
            Outgoings = new List<KeyValuePair<string, uint>>();
            
            var buffer = new char[3]; 
            NativeWinApi.GetPrivateProfileString("Models", "NumberOfModels", "", buffer, buffer.Length, 
                Properties.Resources.model_manifest);
            var gameCount = Convert.ToUInt32(new string(buffer)) + 1;
            for (var i = 1; i < gameCount; i++)
            {
                var modelNo = BoLib.getGameModel(i);
                var bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                var won = (uint)BoLib.getGamePerformanceMeter((uint)i, 1);
                var titleBuffer = new char[64];
                var name = NativeWinApi.GetPrivateProfileString("Model" + i, "Title", "", titleBuffer, titleBuffer.Length,
                    Properties.Resources.model_manifest);
                
                var title = new string(titleBuffer).Trim("\0".ToCharArray());
                Incomings.Add(new KeyValuePair<string, uint>(title, bet));
                Outgoings.Add(new KeyValuePair<string, uint>(title, won));
            }
            
            Incomings.Sort(CompareValue);
            Outgoings.Sort(CompareValue);
            
            RaisePropertyChangedEvent("Incomings");
            RaisePropertyChangedEvent("Outgoings");
        }
        
        static int CompareTitle(KeyValuePair<string, uint> left, KeyValuePair<string, uint> right)
        {
            return left.Key.CompareTo(right.Key); // for ascending sort.
        }
        
        static int CompareValue(KeyValuePair<string, uint> left, KeyValuePair<string, uint> right)
        {
            return right.Value.CompareTo(left.Value); // for descending sort.
        }
    }   
}
