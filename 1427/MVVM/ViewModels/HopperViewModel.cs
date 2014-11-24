using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using System;

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

        public System.Timers.Timer _emptyLeftTimer;
        public System.Timers.Timer _emptyRightTimer;
        public System.Timers.Timer _refillTimer;

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
        public string LeftRefillMsg 
        {
            get
            {
                return _leftRefillMsg;
            }
            set
            {
                _leftRefillMsg = "Left Hopper Coins Added: " + Convert.ToDecimal(value).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
                this.RaisePropertyChangedEvent("LeftRefillMsg");
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
                this.RaisePropertyChangedEvent("RightRefillMsg");
            }
        }

        #endregion

        public HopperViewModel()
        {
            EndRefill = false;
            _emptyingHoppers = false;
            NotRefilling = true;
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = "Empty Hopper";
            this.RaisePropertyChangedEvent("DumpSwitchMessage");
            SelectedTabIndex = 2;

            LargeHopper = "£1 Hopper (LEFT)";
            SmallHopper = "10p Hopper (RIGHT)";
            this.RaisePropertyChangedEvent("LargeHopper");
            this.RaisePropertyChangedEvent("SmallHopper");
            this.RaisePropertyChangedEvent("NotRefilling");
            LeftRefillMsg = "0.00";//Left Hopper Coins Added: " + (0.00).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            RightRefillMsg = "0.00";//"Right Hopper Coins Added: " + (0.00).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            //this.RaisePropertyChangedEvent("LeftRefillMsg");
        }
        
        public ICommand ToggleRefillStatus
        {
            get { return new DelegateCommand(o => this.NotRefilling = !this.NotRefilling); }
        }

        public void TabControlSelectionChanged(object sender)
        {
            
        }

        public ICommand OnSelectionChanged { get { return new DelegateCommand(ComboBox_OnSelectionChanged); } }
        public void ComboBox_OnSelectionChanged(object sender)
        {
            //e.Handled = true;
        }
        
        public ICommand DoEmptyHopper
        {
            get { return new DelegateCommand(EmptyHopper); }
        }
        
        void EmptyHopper(object hopper)
        {
            var cb = hopper as ComboBox;
            bool dumpSwitchPressed = false;
            _emptyingHoppers = true;

            if (_emptyLeftTimer == null)
                _emptyLeftTimer = new System.Timers.Timer(100.0);
            
            if (cb.SelectedIndex == 0)
            {
                System.Diagnostics.Debug.WriteLine("SELECTED LEFT HOPPER (£1)");
                System.Diagnostics.Debug.WriteLine(Convert.ToDecimal(BoLib.getHopperFloatLevel(0)));
                
                _emptyLeftTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
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
                                this.RaisePropertyChangedEvent("DumpSwitchMessage");
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
                            this.RaisePropertyChangedEvent("DumpSwitchMessage");
                            var t = sender as System.Timers.Timer;
                            t.Enabled = false;
                            dumpSwitchPressed = false;
                        }
                    }
                };
                _emptyLeftTimer.Enabled = true;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
#endif
            }
            else if (cb.SelectedIndex == 1)
            {
                if (_emptyRightTimer == null)
                {
                    _emptyRightTimer = new System.Timers.Timer(100.0);
                    _emptyRightTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                    {
                        
                    };
                }
                System.Diagnostics.Debug.WriteLine("SELECTED RIGHT HOPPER (10p)");
                _emptyRightTimer.Enabled = true;
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
            if (_refillTimer == null)
            {
                _refillTimer = new System.Timers.Timer(100);
                _refillTimer.Enabled = true;
            }
            
            NotRefilling = false;
            _refillTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (EndRefill)
                {
                    this.EndRefill = false;
                    _refillTimer.Enabled = false;
                    //dispatch timer to update labels every x milliseconds.
                }
            };
        }
    }
}
