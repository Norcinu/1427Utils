﻿using System;
using System.Collections.Generic;
using System.Linq;
using PDTUtils.Native;


namespace PDTUtils.MVVM.ViewModels
{
    class GameChartViewModel : ObservableObject
    {
        public class KeepOnGiving
        {
            public uint Money { get; set; }
            public uint GameCount { get; set; }
        }
        
        public List<KeyValuePair<string, uint>> IncomingsSimple { get; set; }
        public List<KeyValuePair<string, uint>> OutgoingsSimple { get; set; }

        public List<KeyValuePair<string, KeepOnGiving>> Incomings { get; set; }
        public List<KeyValuePair<string, KeepOnGiving>> Outgoings { get; set; }

        /*string _manifest = (BoLib.getCountryCode() == 9) ? Properties.Resources.model_manifest_esp
                                                         : Properties.Resources.model_manifest;*/

        string _manifest = (MachineDescription.CountryCode == BoLib.getSpainCountryCode()) ? Properties.Resources.model_manifest_esp :
                                                                                             Properties.Resources.model_manifest;
        
        public GameChartViewModel()
        {
            try
            {
                Incomings = new List<KeyValuePair<string, KeepOnGiving>>();
                Outgoings = new List<KeyValuePair<string, KeepOnGiving>>();

                IncomingsSimple = new List<KeyValuePair<string, uint>>();
                OutgoingsSimple = new List<KeyValuePair<string, uint>>();
                
                var buffer = new char[3];
                NativeWinApi.GetPrivateProfileString("Models", "NumberOfModels", "", buffer, buffer.Length, _manifest);
                var gameCount = Convert.ToUInt32(new string(buffer)) + 1;
                for (var i = 1; i < gameCount; i++)
                {
                    var modelNo = BoLib.getGameModel(i);
                    var bet = (uint)BoLib.getGamePerformanceMeter((uint)i, 0);
                    var won = (uint)BoLib.getGamePerformanceMeter((uint)i, 1);
                    
                    var titleBuffer = new char[64];
                    var name = NativeWinApi.GetPrivateProfileString("Model" + i, "Title", "", titleBuffer, titleBuffer.Length,
                        _manifest);
                    
                    var count = (uint)BoLib.getGamePerformanceMeter((uint)i, 2);
                    var title = new string(titleBuffer).Trim("\0".ToCharArray());

                    Incomings.Add(new KeyValuePair<string, KeepOnGiving>(title, new KeepOnGiving() { Money = bet, GameCount = count }));
                    Outgoings.Add(new KeyValuePair<string, KeepOnGiving>(title, new KeepOnGiving() { Money = won, GameCount = count }));

                    IncomingsSimple.Add(new KeyValuePair<string, uint>(title, bet));
                    OutgoingsSimple.Add(new KeyValuePair<string, uint>(title, won));
                }
                
                Incomings.Sort(CompareValue); 
                Outgoings.Sort(CompareValue);
            }
            catch (Exception e)
            {
                //new WpfMessageBoxService().ShowMessage(e.Message, "Loading Error");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            RaisePropertyChangedEvent("Incomings");
            RaisePropertyChangedEvent("Outgoings");

            RaisePropertyChangedEvent("IncomingsSimple");
            RaisePropertyChangedEvent("OutgoingsSimple");
        }
        
        static int CompareTitle(KeyValuePair<string, KeepOnGiving> left, KeyValuePair<string, KeepOnGiving> right)
        {
            return left.Key.CompareTo(right.Key); // for ascending sort.
        }
        
        static int CompareValue(KeyValuePair<string, KeepOnGiving> left, KeyValuePair<string, KeepOnGiving> right)
        {
            return right.Value.Money.CompareTo(left.Value.Money); // for descending sort.
        }
    }   
}
