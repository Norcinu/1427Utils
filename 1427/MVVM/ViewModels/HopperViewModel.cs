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
using PDTUtils.Properties;

namespace PDTUtils.MVVM.ViewModels
{
    class HopperViewModel : ObservableObject
    {
        bool _enabled;
        bool _emptyingHoppers;
        bool _needToSync;
        bool _syncLeft;
        bool _syncRight;
        int _selectedTabIndex;
        string _leftRefillMsg;
        string _rightRefillMsg;
        string _leftCoinsAdded;
        string _rightCoinsAdded;
        string _refloatLeft;
        string _refloatRight;

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

        public string DivertLeftMessage { get; set; }
        public string DivertRightMessage { get; set; }

        public string SelHopperValue { get; set; }

        public bool EmptyingHoppers
        {
            get { return _emptyingHoppers; }
            set { _emptyingHoppers = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChangedEvent("Enabled");
                RaisePropertyChangedEvent("LeftRefillMsg");
                RaisePropertyChangedEvent("RightRefillMsg");
            }
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

        public string RefloatRight
        {
            get { return _refloatRight; }
            set
            {
                _refloatRight = value;
                RaisePropertyChangedEvent("RefloatRight");
            }
        }
        
        public string RefloatLeft
        {
            get { return _refloatLeft; }
            set
            {
                _refloatLeft = value;
                RaisePropertyChangedEvent("RefloatLeft");
            }
        }

        public bool NeedToSync
        {
            get { return _needToSync; }
            set
            {
                _needToSync = value;
                RaisePropertyChangedEvent("NeedToSync");
            }
        }

        #endregion

        public HopperViewModel()
        {
            CurrentCulture = BoLib.getCountryCode() == BoLib.getSpainCountryCode()
                ? new CultureInfo("es-ES")
                : new CultureInfo("en-GB");

            Nfi = CurrentCulture.NumberFormat;

            RefloatLeft = "";
            RefloatRight = "";
            _enabled = false;
            EndRefill = false;
            _emptyingHoppers = false;
            NotRefilling = true;
            LeftHopper = new HopperModel();
            RightHopper = new HopperModel();
            DumpSwitchMessage = "Empty Hopper";
            AreWeRefilling = false;
            SelectedTabIndex = 0;
            DivertLeftMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Left).ToString();
            DivertRightMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Right).ToString();
            
            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;

            SelHopperValue = "Hopper Level: " + Convert.ToDecimal(BoLib.getHopperFloatLevel(0)).ToString();

            InitRefloatLevels();

            RaisePropertyChangedEvent("DumpSwitchMessage");
            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
            RaisePropertyChangedEvent("NotRefilling");
            RaisePropertyChangedEvent("AreWeReflling");
            RaisePropertyChangedEvent("SelHopperValue");

            HopperList = new List<string>();

            var leftHopperText = "LEFT HOPPER";
            var rightHopperText = "RIGHT HOPPER";
            HopperList.Add(leftHopperText);
            HopperList.Add(rightHopperText);
            RaisePropertyChangedEvent("HopperList");

            const uint initial = 0;
            try
            {
                LeftRefillMsg = "Left Hopper Coins Added: " + initial.ToString();
                RightRefillMsg = "Right Hopper Coins Added: " + initial.ToString();
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
                            SelHopperValue = "Hopper Level: " + BoLib.getHopperFloatLevel((byte)Hoppers.Left);
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
                                SelHopperValue = "Hopper Level: " + BoLib.getHopperFloatLevel((byte)Hoppers.Right);
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

        void InitRefloatLevels()
        {
            char[] refloatValue = new char[5];
            NativeWinApi.GetPrivateProfileString("Config", "RefloatLH", "", refloatValue, 5, Resources.birth_cert);
            RefloatLeft = new string(refloatValue).Trim("\0".ToCharArray());
            NativeWinApi.GetPrivateProfileString("Config", "RefloatRH", "", refloatValue, 5, Resources.birth_cert);
            RefloatRight = new string(refloatValue).Trim("\0".ToCharArray());
        }

        void CheckRefloatDivert()
        {
            if (Convert.ToUInt32(RefloatLeft) > BoLib.getHopperDivertLevel((byte)Hoppers.Left))
            {
                _needToSync = true;
                _syncLeft = true;
            }

            if (Convert.ToUInt32(RefloatRight) > BoLib.getHopperDivertLevel((byte)Hoppers.Right))
            {
                _needToSync = true;
                _syncRight = true;
            }
        }

        public ICommand PerformSort { get { return new DelegateCommand(o => DoPerformSort()); } }
        void DoPerformSort()
        {

        }

        public ICommand SetRefloatLevel { get { return new DelegateCommand(DoSetRefloatLevel); } }
        void DoSetRefloatLevel(object o)
        {
            var format = o as string;
            var tokens = format.Split("+".ToCharArray());
            const int denom = 50;
            string key = (tokens[0] == "left") ? "RefloatLH" : "RefloatRH";

            char[] refloatValue = new char[5];
            NativeWinApi.GetPrivateProfileString("Config", key, "", refloatValue, 5, Resources.birth_cert);

            var newRefloatValue = Convert.ToUInt32(new string(refloatValue).Trim("\0".ToCharArray()));
            if (tokens[1] == "increase") newRefloatValue += denom;
            else if (tokens[1] == "decrease") newRefloatValue -= denom;

            if (tokens[0] == "left")
                RefloatLeft = newRefloatValue.ToString();
            else
                RefloatRight = refloatValue.ToString();
            
            NativeWinApi.WritePrivateProfileString("Config", key, newRefloatValue.ToString(), Resources.birth_cert);
            RaisePropertyChangedEvent(key);
        }


        public ICommand ChangeLeftDivert { get { return new DelegateCommand(DoChangeDivert); } }
        void DoChangeDivert(object o)
        {
            var actionType = o as string;
            var currentThreshold = BoLib.getHopperDivertLevel(0);
            const uint changeAmount = 50;
            var newValue = currentThreshold;

            if (actionType == "increment" && currentThreshold < 800)
            {
                newValue += changeAmount;
                if (newValue > 800)
                    newValue = 800;
            }
            else if (actionType == "decrement" && currentThreshold > 200)
            {
                newValue -= changeAmount;
                if (newValue < 200)
                    newValue = 0;
            }

            BoLib.setHopperDivertLevel(BoLib.getLeftHopper(), newValue);
            NativeWinApi.WritePrivateProfileString("Config", "LH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);

            DivertLeftMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertLeftMessage");
        }

        public ICommand ChangeRightDivert { get { return new DelegateCommand(DoChangeDivertRight); } }
        void DoChangeDivertRight(object o)
        {
            var actionType = o as string;
            var currentThreshold = BoLib.getHopperDivertLevel((byte)Hoppers.Right);
            const uint changeAmount = 50;
            var newValue = currentThreshold;

            if (actionType == "increment" && currentThreshold < 600)
            {
                newValue += changeAmount;
                if (newValue > 600)
                    newValue = 600;
            }
            else if (actionType == "decrement" && currentThreshold > 50)
            {
                newValue -= changeAmount;
                if (newValue < 50)
                    newValue = 50;
            }

            BoLib.setHopperDivertLevel(BoLib.getRightHopper(), newValue);
            NativeWinApi.WritePrivateProfileString("Config", "RH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);

            DivertRightMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertRightMessage");
        }
    }
}
