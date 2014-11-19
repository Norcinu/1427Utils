using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class HopperViewModel : ObservableObject
    {
        string _dumpSwitchMessage = "";

        public bool NotRefilling { get; set; }        // Disabling tabs.
        public HopperModel LeftHopper { get; set; }     // £1 Hopper
        public HopperModel RightHopper { get; set; }    // 10p Hopper
        public string DumpSwitchMessage
        {
            get;
            set;
           /* get
            {
                return _dumpSwitchMessage;
            }
            set 
            { 
                _dumpSwitchMessage = value;
                this.RaisePropertyChangedEvent("DumpSwitchMessage");
            }   */
        }

        public System.Timers.Timer _emptyTimer;

        public HopperViewModel()
        {
            NotRefilling = true;
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = "RASCLART";
            this.RaisePropertyChangedEvent("DumpSwitchMessage");
        }
        
        public ICommand ToggleRefillStatus
        {
            get { return new DelegateCommand(o => this.NotRefilling = !this.NotRefilling); }
        }

        public ICommand DoEmptyHopper
        {
            get { return new DelegateCommand(EmptyHopper); }
        }
        
        void EmptyHopper(object hopper)
        {
            var cb = hopper as ComboBox;
            bool dumpSwitchPressed = true;
            if (_emptyTimer == null)
                _emptyTimer = new System.Timers.Timer(100.0);
            
            if (cb.SelectedIndex == 0)
            {
                System.Diagnostics.Debug.WriteLine("SELECTED LEFT HOPPER (£1)");

                _emptyTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
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
                                //break;
                            }
                        /*    Thread.Sleep(2);*/
                        }
                    }

                    if (BoLib.refillKeyStatus() > 0 && BoLib.getDoorStatus() > 0 && dumpSwitchPressed)
                    {
                        BoLib.setRequestEmptyLeftHopper();
                        if /*while*/(BoLib.getRequestEmptyLeftHopper() > 0 && BoLib.getHopperFloatLevel(0) > 0)
                        {
                            /*Thread.Sleep(2);*/
                            System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
                            //if (BoLib.getHopperFloatLevel(0) == 0)
                            //    break;
                        }
                        else if (BoLib.getHopperFloatLevel(0) == 0)
                        {
                            var t = sender as System.Timers.Timer;
                            t.Enabled = false;
                        }
                    }
                };
                _emptyTimer.Enabled = true;
            //    DumpSwitchMessage = "";
           //     this.RaisePropertyChangedEvent("DumpSwitchMessage");
#if DEBUG
                System.Diagnostics.Debug.WriteLine(BoLib.getHopperFloatLevel(0));
#endif
            }
            else if (cb.SelectedIndex == 1)
            {
                System.Diagnostics.Debug.WriteLine("SELECTED RIGHT HOPPER (10p)");
            }
        }
    }
}
