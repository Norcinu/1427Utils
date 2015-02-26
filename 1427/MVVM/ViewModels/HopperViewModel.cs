using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

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

        public System.Timers.Timer EmptyLeftTimer;
        public System.Timers.Timer EmptyRightTimer;
        public System.Timers.Timer RefillTimer;
        
        NumberFormatInfo _nfi;
        CultureInfo _currentCulture;
        
        #region Properties
        public bool EndRefill { get; set; }
        public bool NotRefilling { get; set; }        // Disabling tabs.
        public HopperModel LeftHopper { get; set; }     // £1 Hopper
        public HopperModel RightHopper { get; set; }    // 10p Hopper
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
        public string LargeHopper { get; set; }
        public string SmallHopper { get; set; }
        //public string MiddleHopper { get; set; } not needed atm.
        public List<string> HopperList { get; set; }

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
                _rightRefillMsg = "Right Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
                RaisePropertyChangedEvent("RightRefillMsg");
            }
        }

        public bool AreWeRefilling { get; set; }

        #endregion
        
        public HopperViewModel()
        {
            EndRefill = false;
            _emptyingHoppers = false;
            NotRefilling = true;
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = "Empty Hopper";
            AreWeRefilling = false;    
            SelectedTabIndex = 0;

            if (BoLib.getCountryCode() == BoLib.getSpainCountryCode())
                _currentCulture = new CultureInfo("es-ES");
            else
                _currentCulture = new CultureInfo("en-GB");

            _nfi = _currentCulture.NumberFormat;
            
            LargeHopper = "£1 Hopper (LEFT)";
            SmallHopper = "10p Hopper (RIGHT)";
            
            RaisePropertyChangedEvent("DumpSwitchMessage");
            RaisePropertyChangedEvent("LargeHopper");
            RaisePropertyChangedEvent("SmallHopper");
            RaisePropertyChangedEvent("NotRefilling");
            RaisePropertyChangedEvent("AreWeReflling");

            HopperList = new List<string>();
            string leftHopperText = (BoLib.getCountryCode() == BoLib.getSpainCountryCode()) ? "LEFT SPAIN HOPPER" : "£1 Hopper (LEFT)";
            string rightHopperText = (BoLib.getCountryCode() == BoLib.getSpainCountryCode()) ? "RIGHT SPANISH HOPPER" : "£2 Hopper (RIGHT)";
            HopperList.Add(leftHopperText);
            HopperList.Add(rightHopperText);
            RaisePropertyChangedEvent("HopperList");
            
            decimal initial = 0.00M;
            try
            {
                LeftRefillMsg = "Left Hopper Coins Added: " + initial.ToString("C", _nfi);
                RightRefillMsg = "Right Hopper Coins Added: " + initial.ToString("C", _nfi);
            }
            catch (FormatException)
            {
                LeftRefillMsg = initial.ToString();
                RightRefillMsg = initial.ToString();
            }
        }
        
        public ICommand ToggleRefillStatus
        {
            get { return new DelegateCommand(o => this.NotRefilling = !this.NotRefilling); }
        }
        
        public void TabControlSelectionChanged(object sender)
        {
            
        }
        //shaking trees, bring smiling face, startled eyes through smokey haze. 
        //shake
        public ICommand DoEmptyHopper
        {
            get { return new DelegateCommand(EmptyHopper); }
        }
        
        void EmptyHopper(object hopper)    
        {
            var cb = hopper as ComboBox;
            string value = cb.SelectedValue as string;
            bool dumpSwitchPressed = false;
            _emptyingHoppers = true;

            if (EmptyLeftTimer == null)
                EmptyLeftTimer = new System.Timers.Timer(100.0);
            
            if (cb.SelectedIndex == 0)
            {
                System.Diagnostics.Debug.WriteLine("SELECTED LEFT HOPPER (£1)");
                System.Diagnostics.Debug.WriteLine(Convert.ToDecimal(BoLib.getHopperFloatLevel(0)));
                
                EmptyLeftTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
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
                    
                    if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0 && dumpSwitchPressed)
                    {
                        BoLib.setRequestEmptyLeftHopper();
                        if (BoLib.getRequestEmptyLeftHopper() > 0 && BoLib.getHopperFloatLevel(0) > 0)
                        {
                            Thread.Sleep(2);
                            System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
                        }
                        else if (BoLib.getHopperFloatLevel(0) == 0)
                        {
                            DumpSwitchMessage = "Hopper Empty";
                            RaisePropertyChangedEvent("DumpSwitchMessage");
                            var t = sender as System.Timers.Timer;
                            t.Enabled = false;
                            dumpSwitchPressed = false;
                        }
                    }
                };
                EmptyLeftTimer.Enabled = true;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
#endif
            }//sounds ok
            else if (cb.SelectedIndex == 1)
            {
                if (EmptyRightTimer == null)
                {
                    EmptyRightTimer = new System.Timers.Timer(100.0);
                    EmptyRightTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                    {
                        if (BoLib.getHopperDumpSwitchActive() > 0)
                        {
                            DumpSwitchMessage = "Press Coin Dump Button to Continue";
                            this.RaisePropertyChangedEvent("DumpSwitchMessage");

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

                        if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0 && dumpSwitchPressed)
                        {
                            BoLib.setRequestEmptyRightHopper();
                            if (BoLib.getRequestEmptyRightHopper() > 0 && BoLib.getHopperFloatLevel(2) > 0)
                            {
                                Thread.Sleep(2);
                                System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(2));
                            }
                            else if (BoLib.getHopperFloatLevel(2) == 0)
                            {
                                DumpSwitchMessage = "Hopper Empty";
                                RaisePropertyChangedEvent("DumpSwitchMessage");
                                var t = sender as System.Timers.Timer;
                                t.Enabled = false;
                                dumpSwitchPressed = false;
                            }
                        }
                    };
                }
                System.Diagnostics.Debug.WriteLine("SELECTED RIGHT HOPPER (10p)");
                EmptyRightTimer.Enabled = true;
            }
        }
        
        public ICommand RefillHopper { get { return new DelegateCommand(o => DoRefillHopper()); } }
        void DoRefillHopper()
        {
            ulong coins = BoLib.getReconciliationMeter(14) + BoLib.getReconciliationMeter(15);
        }
        
        public ICommand EndRefillCommand { get { return new DelegateCommand(o => DoEndRefill()); } }
        void DoEndRefill()
        {
            if (RefillTimer == null)
            {
                RefillTimer = new System.Timers.Timer(100);
                RefillTimer.Enabled = true;
            }
            
            NotRefilling = false;
            RefillTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (EndRefill)
                {
                    this.EndRefill = false;
                    RefillTimer.Enabled = false;
                    //dispatch timer to update labels every x milliseconds.
                }
            };
        }
    }
}
