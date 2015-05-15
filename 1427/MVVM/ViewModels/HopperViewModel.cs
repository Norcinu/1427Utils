using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using Timer = System.Timers.Timer;

namespace PDTUtils.MVVM.ViewModels
{
    class HopperViewModel : ObservableObject
    {
        bool _emptyingHoppers;
        int _selectedTabIndex;
        string _leftRefillMsg;
        string _rightRefillMsg;
        string _leftCoinsAdded;
        string _rightCoinsAdded;
    
        Timer EmptyLeftTimer;
        Timer EmptyRightTimer;
        Timer RefillTimer;
        
        NumberFormatInfo Nfi { get; set; }
        CultureInfo CurrentCulture { get; set; }

        #region Properties
        public bool EndRefill { get; set; }
        public bool NotRefilling { get; set; }          // Disabling tabs.
        public HopperModel LeftHopper { get; set; }     // £1 Hopper
        public HopperModel RightHopper { get; set; }    // 10p Hopper
        
        public string SelHopperValue { get; set; }

        public bool EmptyingHoppers
        {
            get { return _emptyingHoppers; }
            set { _emptyingHoppers = value; }
        }
        public int SelectedTabIndex 
        {
            get { return _selectedTabIndex; }
            set 
            { 
                if (!_emptyingHoppers) 
                    _selectedTabIndex = value;
            }
        }
        public string DumpSwitchMessage { get; set; }
        //public string LargeHopper { get; set; }
        //public string SmallHopper { get; set; }
        
        public List<string> HopperList { get; set; }
        
        public ObservableCollection<HopperModel> _hopperModels = new ObservableCollection<HopperModel>();
        public ObservableCollection<HopperModel> HopperModels
        {
            get { return _hopperModels; }
            set
            {
                _hopperModels = value;
                RaisePropertyChangedEvent("HopperModels");
            }
        }
        
        public string LeftRefillMsg
        {
            get
            {
                return _leftRefillMsg;
            }
            set
            {
                _leftRefillMsg = "Left Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C",
                    Thread.CurrentThread.CurrentCulture.NumberFormat);
                RaisePropertyChangedEvent("LeftRefillMsg");
            }
        }
        
        public string RightRefillMsg
        {
            get
            {
                return _rightRefillMsg;
            }
            set
            {
                _rightRefillMsg = "Right Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C", 
                    Thread.CurrentThread.CurrentCulture.NumberFormat);
                RaisePropertyChangedEvent("RightRefillMsg");
            }
        }

        public bool AreWeRefilling { get; set; }

        #endregion
        
        public HopperViewModel()
        {
            CurrentCulture = BoLib.getCountryCode() == BoLib.getSpainCountryCode()
                ? new CultureInfo("es-ES")
                : new CultureInfo("en-GB");

            Nfi = CurrentCulture.NumberFormat;

            EndRefill = false;
            _emptyingHoppers = false;
            NotRefilling = true;
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = "Empty Hopper";
            AreWeRefilling = false;    
            SelectedTabIndex = 0;

            SelHopperValue = "Hopper Level: " + Convert.ToDecimal(BoLib.getHopperFloatLevel(0)).ToString();//"C", Nfi);
            
            //LargeHopper = "£1 Hopper (LEFT)";
            //SmallHopper = "10p Hopper (RIGHT)";
            
            RaisePropertyChangedEvent("DumpSwitchMessage");
            //RaisePropertyChangedEvent("LargeHopper");
            //RaisePropertyChangedEvent("SmallHopper");
            RaisePropertyChangedEvent("NotRefilling");
            RaisePropertyChangedEvent("AreWeReflling");
            RaisePropertyChangedEvent("SelHopperValue");
            
            HopperList = new List<string>();
            
            var leftHopperText = "LEFT HOPPER";
                //(BoLib.getCountryCode() == BoLib.getSpainCountryCode()) ? "SMALL HOPPER" : "£1 Hopper (LEFT)";
            var rightHopperText = "RIGHT HOPPER";
                //(BoLib.getCountryCode() == BoLib.getSpainCountryCode()) ? "LARGE HOPPER" : "£2 Hopper (RIGHT)";
            HopperList.Add(leftHopperText);
            HopperList.Add(rightHopperText);
            RaisePropertyChangedEvent("HopperList");
            
            //const decimal initial = 0.00M;
            const uint initial = 0;
            try
            {
                LeftRefillMsg = "Left Hopper Coins Added: " + initial.ToString();/*"C", Nfi);*/
                RightRefillMsg = "Right Hopper Coins Added: " + initial.ToString();/*"C", Nfi);*/
            }
            catch (FormatException)
            {
                LeftRefillMsg = initial.ToString();
                RightRefillMsg = initial.ToString();
            }
        }
        
        public ICommand ToggleRefillStatus
        {
            get { return new DelegateCommand(o => NotRefilling = !NotRefilling); }
        }
        
        public void TabControlSelectionChanged(object sender)
        {
            
        }

        public ICommand DoEmptyHopper
        {
            get { return new DelegateCommand(EmptyHopper); }
        }

        void EmptyHopper(object hopper)
        {
            var cb = hopper as ComboBox;
            var dumpSwitchPressed = false;
            _emptyingHoppers = true;

            if (EmptyLeftTimer == null)
                EmptyLeftTimer = new Timer(100.0);


            switch (cb.SelectedIndex)
            {

                case 0:
                    if (BoLib.getHopperFloatLevel((byte)Hoppers.Left) == 0)
                        return;

                    Debug.WriteLine("SELECTED LEFT HOPPER");
                    Debug.WriteLine(Convert.ToDecimal(BoLib.getHopperFloatLevel((byte)Hoppers.Left)));

                    EmptyLeftTimer.Elapsed += (sender, e) =>
                    {
                        if (BoLib.getHopperDumpSwitchActive() > 0)
                        {
                            DumpSwitchMessage = "Press Coin Dump Button to Continue";
                            RaisePropertyChangedEvent("DumpSwitchMessage");

                            if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0)
                            {
                                if (BoLib.getSwitchStatus(2, 0x20) > 0)
                                {
                                    DumpSwitchMessage = "Dump Button Pressed OK";
                                    RaisePropertyChangedEvent("DumpSwitchMessage");
                                    dumpSwitchPressed = true;
                                }
                                Thread.Sleep(2);
                            }
                        }

                        if (BoLib.refillKeyStatus() <= 0 || BoLib.getDoorStatus() <= 0 || !dumpSwitchPressed) return;
                        BoLib.setRequestEmptyLeftHopper();

                        if (BoLib.getRequestEmptyLeftHopper() > 0 && BoLib.getHopperFloatLevel((byte)Hoppers.Left) > 0)
                        {
                            Thread.Sleep(500); //2000
                            System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
                            SelHopperValue = "Hopper Level: " + /*Convert.ToDecimal(*/BoLib.getHopperFloatLevel((byte)Hoppers.Left/*)*/);
                            RaisePropertyChangedEvent("SelHopperValue");
                        }
                        else if (BoLib.getHopperFloatLevel(0) == 0)
                        {
                            DumpSwitchMessage = "Hopper Empty";
                            RaisePropertyChangedEvent("DumpSwitchMessage");
                            var t = sender as Timer;
                            t.Enabled = false;
                            dumpSwitchPressed = false;
                        }
                    };
                    EmptyLeftTimer.Enabled = true;
#if DEBUG
                    Debug.WriteLine(BoLib.getHopperFloatLevel(0));
#endif
                    break;
                case 1:
                    if (BoLib.getHopperFloatLevel((byte)Hoppers.Right) == 0) return;

                    if (EmptyRightTimer == null)
                    {
                        Debug.WriteLine("SELECTED RIGHT HOPPER");
                        Debug.WriteLine(Convert.ToDecimal(BoLib.getHopperFloatLevel((byte)Hoppers.Right)));

                        EmptyRightTimer = new Timer(100.0);
                        EmptyRightTimer.Elapsed += (sender, e) =>
                        {
                            if (BoLib.getHopperDumpSwitchActive() > 0)
                            {
                                DumpSwitchMessage = "Press Coin Dump Button to Continue";
                                RaisePropertyChangedEvent("DumpSwitchMessage");

                                if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0)
                                {
                                    if (BoLib.getSwitchStatus(2, 0x20) > 0)
                                    {
                                        DumpSwitchMessage = "Dump Button Pressed OK";
                                        RaisePropertyChangedEvent("DumpSwitchMessage");
                                        dumpSwitchPressed = true;
                                    }
                                    Thread.Sleep(2);
                                }
                            }
                            
                            if (BoLib.refillKeyStatus() <= 0 || BoLib.getDoorStatus() <= 0 || !dumpSwitchPressed) return;
                            BoLib.setRequestEmptyRightHopper();

                            if (BoLib.getRequestEmptyRightHopper() > 0 && BoLib.getHopperFloatLevel((byte)Hoppers.Right) > 0)
                            {
                                Thread.Sleep(500);//2000
                                System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(2));
                                SelHopperValue = "Hopper Level: " + /*Convert.ToDecimal(*/BoLib.getHopperFloatLevel((byte)Hoppers.Right)/*)*/;
                                RaisePropertyChangedEvent("SelHopperValue");
                            }
                            else if (BoLib.getHopperFloatLevel((byte)Hoppers.Right) == 0)
                            {
                                DumpSwitchMessage = "Hopper Empty";
                                RaisePropertyChangedEvent("DumpSwitchMessage");
                                var t = sender as Timer;
                                t.Enabled = false;
                                dumpSwitchPressed = false;
                            }
                        };
                    }
                    Debug.WriteLine("SELECTED RIGHT HOPPER");
                    EmptyRightTimer.Enabled = true;
                    break;
            }
        }
        
       /* public ICommand RefillHopper { get { return new DelegateCommand(o => DoRefillHopper()); } }
        void DoRefillHopper()
        {
            if (RefillHopper == null)
            {
                RefillTimer = new Timer(100) { Enabled = true };
                NotRefilling = true;//rename this.
            }
            else if (!RefillTimer.Enabled)
            {
                RefillTimer.Enabled = true;
                NotRefilling = true;
            }
            
            RefillTimer.Elapsed += (sender, e) =>
            {
                if (!EndRefill) return;
                ulong coins = BoLib.getReconciliationMeter(14) + BoLib.getReconciliationMeter(15);
                Debug.WriteLine("Coin Level", coins.ToString());
                //dispatch timer to update labels every x milliseconds.
            };
        }
        //
        public ICommand EndRefillCommand { get { return new DelegateCommand(o => DoEndRefill()); } }
        void DoEndRefill()
        {            
            NotRefilling = false;
            RefillTimer.Enabled = false;
            //BoLib.disableNoteValidator();
        }*/
    }
}
