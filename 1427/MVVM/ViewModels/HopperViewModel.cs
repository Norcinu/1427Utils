using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;
using PDTUtils.Properties;
using Timer = System.Timers.Timer;
using GlobalConfig = PDTUtils.Logic.GlobalConfig;

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
        //public List<string> HopperList { get; set; }
        public ObservableCollection<string> HopperList { get; set; }

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

        string _currentSelHopper = "";
        public string CurrentSelHopper
        {
            get { return _currentSelHopper; }
            set
            {
                _currentSelHopper = value;
                if (_currentSelHopper == "LEFT HOPPER" || string.IsNullOrEmpty(_currentSelHopper))
                    SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
                else
                    SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
                
                RaisePropertyChangedEvent("SelHopperValue");
                RaisePropertyChangedEvent("CurrentSelHopper");
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
            DumpSwitchMessage = "";
            AreWeRefilling = false;
            SelectedTabIndex = 0;
            DivertLeftMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Left).ToString();
            DivertRightMessage = BoLib.getHopperDivertLevel((byte)Hoppers.Right).ToString();
            
            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;
            
            SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
            
            InitRefloatLevels();
            
            RaisePropertyChangedEvent("DumpSwitchMessage");
            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
            RaisePropertyChangedEvent("NotRefilling");
            RaisePropertyChangedEvent("AreWeReflling");
            RaisePropertyChangedEvent("SelHopperValue");
            
            HopperList = new ObservableCollection<string>();
            
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
                            Thread.Sleep(2000);
                            SelHopperValue = BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString();
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
                    break;

                case 1:
                    if (BoLib.getHopperFloatLevel((byte)Hoppers.Right) == 0) return;
                    
                    if (EmptyRightTimer == null)
                    {
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
                                Thread.Sleep(2000);
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

                    EmptyRightTimer.Enabled = true;
                    break;
            }
        }

        void InitRefloatLevels()
        {
            char[] refloatValue = new char[10];
            NativeWinApi.GetPrivateProfileString("Config", "RefloatLH", "", refloatValue, 10, Resources.birth_cert);
            RefloatLeft = new string(refloatValue).Trim("\0".ToCharArray());
            CheckForSync(Convert.ToUInt32(RefloatLeft), "left");
            NativeWinApi.GetPrivateProfileString("Config", "RefloatRH", "", refloatValue, 10, Resources.birth_cert);
            RefloatRight = new string(refloatValue).Trim("\0".ToCharArray());
            CheckForSync(Convert.ToUInt32(RefloatRight), "right");
        }
        
        public ICommand PerformSync { get { return new DelegateCommand(o => DoPerformSync()); } }
        void DoPerformSync()
        {
            if (!_syncLeft && !_syncRight)
            {
                NeedToSync = false;
                return;
            }
            
            NSync(ref _syncLeft, ref _refloatLeft, 0);
            NSync(ref _syncRight, ref _refloatRight, 2);

            RaisePropertyChangedEvent("RefloatLeft");
            RaisePropertyChangedEvent("RefloatRight");
        }

        /// <summary>
        /// Bye Bye Bye
        /// </summary>
        /// <param name="shouldSync">Its gonna be me</param>
        /// <param name="refloatValue">Tearing up my heart</param>
        /// <param name="whichHopper">This I promise you</param>
        void NSync(ref bool shouldSync, ref string refloatValue, byte whichHopper)
        {
            if (shouldSync)
            {
                var divert = BoLib.getHopperDivertLevel(whichHopper);
                refloatValue = divert.ToString();
                shouldSync = false;
                var key = (whichHopper == (byte)Hoppers.Left) ? "RefloatLH" : "RefloatRH";
                NativeWinApi.WritePrivateProfileString("Config", key, refloatValue, Resources.birth_cert);
                shouldSync = false;
            }
        }
        
        void CheckForSync(UInt32 newRefloatValue, string hopper)
        {
            if (hopper == "left")
            {
                if (newRefloatValue > BoLib.getHopperDivertLevel((byte)Hoppers.Left))
                {
                    NeedToSync = true;
                    _syncLeft = true;
                }
                else
                    _syncLeft = false;
            }
            else
            {
                if (newRefloatValue > BoLib.getHopperDivertLevel((byte)Hoppers.Right))
                {
                    NeedToSync = true;
                    _syncRight = true;
                }
                else
                    _syncRight = false;
            }
            
            if (!_syncLeft && !_syncRight)
                NeedToSync = false;
        }
        
        public ICommand SetRefloatLevel { get { return new DelegateCommand(DoSetRefloatLevel); } }
        void DoSetRefloatLevel(object o)
        {
            var format = o as string;
            var tokens = format.Split("+".ToCharArray());
            const int denom = 50;
            string key = (tokens[0] == "left") ? "RefloatLH" : "RefloatRH";

            char[] refloatValue = new char[10];
            NativeWinApi.GetPrivateProfileString("Config", key, "", refloatValue, 10, Resources.birth_cert);

            var newRefloatValue = Convert.ToUInt32(new string(refloatValue).Trim("\0".ToCharArray()));
            if (tokens[1] == "increase") newRefloatValue += denom;
            else if (tokens[1] == "decrease" && newRefloatValue > denom) newRefloatValue -= denom;

            if (tokens[0] == "left")
                RefloatLeft = newRefloatValue.ToString();
            else
                RefloatRight = newRefloatValue.ToString();

            NativeWinApi.WritePrivateProfileString("Config", key, newRefloatValue.ToString(), Resources.birth_cert);

            CheckForSync(newRefloatValue, tokens[0]);

            RaisePropertyChangedEvent(key);
        }
        
        public ICommand ChangeLeftDivert { get { return new DelegateCommand(DoChangeDivert); } }
        void DoChangeDivert(object o)
        {
            const uint changeAmount = 50;
            var actionType = o as string;
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Config", "LH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));//BoLib.getHopperDivertLevel(0);
            var newValue = currentThreshold;
            
            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 200)
            {
                newValue -= changeAmount;
                if (newValue < 200)
                    newValue = 0;
            }
                
            //BoLib.setHopperDivertLevel(BoLib.getLeftHopper(), newValue);
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Config", "LH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);
            
            DivertLeftMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertLeftMessage");
        }
        
        public ICommand ChangeRightDivert { get { return new DelegateCommand(DoChangeDivertRight); } }
        void DoChangeDivertRight(object o)
        {
            var actionType = o as string;
            //var currentThreshold = BoLib.getHopperDivertLevel((byte)Hoppers.Right);
            char[] divert = new char[10];
            NativeWinApi.GetPrivateProfileString("Config", "RH Divert Threshold", "", divert, 10, Resources.birth_cert);
            var currentThreshold = Convert.ToUInt32(new string(divert));
            const uint changeAmount = 50;
            var newValue = currentThreshold;
            
            if (actionType == "increase")
            {
                newValue += changeAmount;
            }
            else if (actionType == "decrease" && currentThreshold > 50)
            {
                newValue -= changeAmount;
                if (newValue < 50)
                    newValue = 50;
            }
            
            //BoLib.setHopperDivertLevel(BoLib.getRightHopper(), newValue);
            GlobalConfig.ReparseSettings = true;
            NativeWinApi.WritePrivateProfileString("Config", "RH Divert Threshold", newValue.ToString(), Resources.birth_cert);
            PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);

            DivertRightMessage = (newValue).ToString();
            RaisePropertyChangedEvent("DivertRightMessage");
        }
        
        public ICommand LoadDefaults { get { return new DelegateCommand(o => DoLoadDefaults()); } }
        void DoLoadDefaults()
        {
            uint[] refloatDefaults = new uint[2] { 600, 50 };
            uint[] divertDefaults = new uint[2] { 800, 250 };

            BoLib.setHopperFloatLevel((byte)Hoppers.Left, refloatDefaults[0]);
            BoLib.setHopperFloatLevel((byte)Hoppers.Right, refloatDefaults[1]);

            RefloatLeft = refloatDefaults[0].ToString();
            RefloatRight = refloatDefaults[1].ToString();

            BoLib.setHopperDivertLevel((byte)Hoppers.Left, divertDefaults[0]);
            BoLib.setHopperDivertLevel((byte)Hoppers.Right, divertDefaults[1]);
            
            DivertLeftMessage = divertDefaults[0].ToString();
            DivertRightMessage = divertDefaults[1].ToString();

            NeedToSync = false;
            _syncLeft = false;
            _syncRight = false;
            
            NativeWinApi.WritePrivateProfileString("Config", "RefloatLH", RefloatLeft, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Config", "RefloatRH", RefloatRight, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Config", "LH Divert Threshold", DivertLeftMessage, Resources.birth_cert);
            NativeWinApi.WritePrivateProfileString("Config", "RH Divert Threshold", DivertRightMessage, Resources.birth_cert);
            
            RaisePropertyChangedEvent("DivertLeftMessage");
            RaisePropertyChangedEvent("DivertRightMessage");
        }
        
        public ICommand RefloatTheHoppers { get { return new DelegateCommand(o => DoRefloatTheHoppers()); } }
        void DoRefloatTheHoppers()
        {
            char[] refloatLeft = new char[10];
            char[] refloatRight = new char[10];

            NativeWinApi.GetPrivateProfileString("Config", "RefloatLH", "", refloatLeft, 10, Resources.birth_cert);
            NativeWinApi.GetPrivateProfileString("Config", "RefloatRH", "", refloatRight, 10, Resources.birth_cert);
            
            RefloatLeft = new string(refloatLeft).Trim("\0".ToCharArray());
            RefloatRight = new string(refloatRight).Trim("\0".ToCharArray());
            
            BoLib.setHopperFloatLevel((byte)Hoppers.Left, Convert.ToUInt32(RefloatLeft));
            BoLib.setHopperFloatLevel((byte)Hoppers.Right, Convert.ToUInt32(RefloatRight));

            SelHopperValue = (_currentSelHopper.Equals("LEFT HOPPER")) ? BoLib.getHopperFloatLevel((byte)Hoppers.Left).ToString()
                                                                       : BoLib.getHopperFloatLevel((byte)Hoppers.Right).ToString();
            
            RaisePropertyChangedEvent("CurrentSelHopper");
            RaisePropertyChangedEvent("SelHopperValue");
        }
        
        /// <summary>
        /// Called via tab selection. If we have refilled elsewhere we will still show old levels.
        /// </summary>
        public void RefreshLevels()
        {
            CurrentSelHopper = CurrentSelHopper;
        }
    }
}
