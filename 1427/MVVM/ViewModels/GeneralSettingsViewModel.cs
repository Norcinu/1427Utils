using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils.MVVM.ViewModels
{
    class GeneralSettingsViewModel : ObservableObject
    {
        public bool IsCatC { get; set; }
        public bool TiToEnabled { get; set; }
        public bool HasRecycler { get; set; }
        public bool UseReserveEnabled { get; set; }
        public string RtpMessage { get; set; }
        public string HandPayLevel { get; set; }
        public string DivertMessage { get; set; }
        public string RecyclerMessage { get; set; }
        public string TerminalAssetMsg { get; set; }
        public string UseReserveStakeMsg { get; set; }

        readonly string _titoDisabledMsg = "Warning: TiTo DISABLED";

        public GeneralSettingsViewModel()
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = BoLib.getCountryCode() != BoLib.getSpainCountryCode() 
                    ? new CultureInfo("en-GB") 
                    : new CultureInfo("es-ES");

                if (BoLib.getCountryCode() != BoLib.getUkCountryCodeC())
                {
                    RtpMessage = "CAT C NOT ENABLED";
                    IsCatC = false;
                }
                else
                {
                    RtpMessage = BoLib.getTargetPercentage().ToString() + "%";
                    IsCatC = true;
                }

                TiToEnabled = BoLib.getTitoEnabledState();
                TerminalAssetMsg = (TiToEnabled) ? "Change Asset" : _titoDisabledMsg;

                HandPayLevel = (BoLib.getHandPayThreshold() / 100).ToString("C", Thread.CurrentThread.CurrentUICulture.NumberFormat);
                DivertMessage = (BoLib.getHopperDivertLevel(0)).ToString("C", Thread.CurrentThread.CurrentUICulture.NumberFormat);
                
                if (BoLib.getBnvType() == 5)
                {
                    HasRecycler = true;
                    if (BoLib.getRecyclerChannel() == 3)
                        RecyclerMessage = "£20 NOTE RECYCLED";
                    else
                        RecyclerMessage = "£10 NOTE RECYCLED";
                }
                else
                {
                    HasRecycler = false;
                    RecyclerMessage = "NO RECYCLER";
                }

                ///!!! DEBUG - Use proper BoLib function for this !!! 
                UseReserveEnabled = true;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            RaisePropertyChangedEvent("HasRecycler");
            RaisePropertyChangedEvent("IsCatC");
            RaisePropertyChangedEvent("HandPayLevel");
            RaisePropertyChangedEvent("DivertMessage");
            RaisePropertyChangedEvent("RecyclerMessage");
            RaisePropertyChangedEvent("TerminalAssetMsg");
            RaisePropertyChangedEvent("UseReserveEnabled");
            RaisePropertyChangedEvent("UseReserveStake");
        }

        public ICommand SetRtp
        {
            get { return new DelegateCommand(ChangeRtp); }
        }
        void ChangeRtp(object newRtp)
        {
            if (BoLib.getCountryCode() == BoLib.getUkCountryCodeC())
            {
                var rtp = Convert.ToInt32(newRtp as string);
                NativeWinApi.WritePrivateProfileString("Datapack", "DPercentage", rtp.ToString(), Resources.machine_ini);
                IniFileUtility.HashFile(Resources.machine_ini);
                
                RtpMessage = rtp.ToString() + "%";
                RaisePropertyChangedEvent("RtpMessage");
            }
        }
        
        public ICommand ChangeHandPayThreshold
        {
            get { return new DelegateCommand(DoChangeHandPayThreshold); }
        }
        public void DoChangeHandPayThreshold(object o)
        {
            if (!BoLib.canPerformHandPay() || BoLib.getTerminalType() == 1)
            {
                var _msgBox = new WpfMessageBoxService();
                _msgBox.ShowMessage("UNABLE TO CHANGE HANDPAY THRESHOLD. CHECK PRINTER OR COUNTRY SETTINGS", "ERROR");
                return;
            }
            
            var type = o as string;
            var current = (int)BoLib.getHandPayThreshold();
            var newVal = current;
            
            var maxHandPay = (int)BoLib.getMaxHandPayThreshold();
            var denom = maxHandPay - current;
            var amount = (denom < 1000) ? denom : 1000;//5000
            
            if (type == "increment")
            {
                BoLib.setHandPayThreshold((uint)current + (uint)amount);
                newVal += amount;
                NativeWinApi.WritePrivateProfileString("Config", "Handpay Threshold", (newVal).ToString(), Resources.birth_cert);
            }
            else
            {
                if (BoLib.getHandPayThreshold() == 0)
                    return;
                
                if (amount == 0)
                    amount = 1000;//5000

                BoLib.setHandPayThreshold((uint)current - (uint)amount);
                newVal -= amount;
                NativeWinApi.WritePrivateProfileString("Config", "Handpay Threshold", (newVal).ToString(), Resources.birth_cert);
            }

            IniFileUtility.HashFile(Resources.birth_cert);
            
            HandPayLevel = (newVal / 100).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            RaisePropertyChangedEvent("HandPayLevel");
        }
        
        public ICommand ChangeDivert { get { return new DelegateCommand(DoChangeDivert); } }
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
            IniFileUtility.HashFile(Resources.birth_cert);

            DivertMessage = (newValue).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            RaisePropertyChangedEvent("DivertMessage");
        }

        public ICommand Recycle { get { return new DelegateCommand(DoRecycleNote); } }
        void DoRecycleNote(object o)
        {
            var noteType = o as string;

            if (BoLib.getBnvType() != 5) return;
            
            var channel = (noteType == "10") ? "2" : "3";
            BoLib.shellSendRecycleNote();
            NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", channel, Resources.birth_cert);
            IniFileUtility.HashFile(Resources.birth_cert);
            RecyclerMessage = (noteType == "10") ? "£10 Recycled" : "£20 Recycled";
            RaisePropertyChangedEvent("RecyclerMessage");
        }
        
        public ICommand TiToState { get { return new DelegateCommand(ToggleTiToState); } }
        void ToggleTiToState(object o)
        {
            var state = o as string;
            TiToEnabled = (state == "enabled");
            if (TiToEnabled) // enable
            {
                var sb = new StringBuilder(20);
                NativeWinApi.GetPrivateProfileString("Keys", "AssetNo", "", sb, sb.Capacity, Resources.machine_ini);
                TerminalAssetMsg = sb.ToString();

                BoLib.setFileAction();

                TerminalAssetMsg = _titoDisabledMsg;
                NativeWinApi.WritePrivateProfileString("Config", "TiToEnabled", "1", Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("Config", "PayoutType", "1", Resources.birth_cert);
                BoLib.setTerminalType(1); //printer
                
                const string bnvType = "6";
                
                var printerType = BoLib.getCabinetType() == 3 ? "3" : "4";
                
                NativeWinApi.WritePrivateProfileString("Config", "PrinterType", printerType, Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("Config", "BnvType", bnvType, Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", "0", Resources.birth_cert);
                BoLib.setRecyclerChannel(0);
                
                BoLib.clearFileAction();
            }
            else // disable
            {
                BoLib.setFileAction();
                
                TerminalAssetMsg = _titoDisabledMsg;
                NativeWinApi.WritePrivateProfileString("Config", "TiToEnabled", "1", Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("Config", "PayoutType", "1", Resources.birth_cert);
                BoLib.setTerminalType(1); //printer
                
                const string bnvType = "6";

                var printerType = BoLib.getCabinetType() == 3 ? "3" : "4";
                
                NativeWinApi.WritePrivateProfileString("Config", "PrinterType", printerType, Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("Config", "BnvType", bnvType, Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", "0", Resources.birth_cert);
                BoLib.setRecyclerChannel(0);

                BoLib.clearFileAction();
            }

            RaisePropertyChangedEvent("TiToEnabled");
            RaisePropertyChangedEvent("TerminalAssetMsg");
            //write to ini file
        }
        
        public ICommand TitoUpdate { get { return new DelegateCommand(DoTitoUpdate); } }
        private void DoTitoUpdate(object o)
        {
            var titoUpdateForm = new IniSettingsWindow();
            var showDialog = titoUpdateForm.ShowDialog();
            if (showDialog != null && (bool)!showDialog)
            {
                var assetNumber = titoUpdateForm.TxtNewValue.Text;
                //validate value here and update accordingly.
                //create a keyboard with just a numberpad.
                NativeWinApi.WritePrivateProfileString("Keys", "AssetNo", assetNumber, Properties.Resources.machine_ini);
                IniFileUtility.HashFile(Properties.Resources.machine_ini);
            }
        }


        //!! DEBUG DEBUG - USE PROPER LIBRARY CALL.
        public ICommand UseReserve { get { return new DelegateCommand(DoUseReserve); } }
        void DoUseReserve(object o)
        {
            UseReserveEnabled = !UseReserveEnabled;
            if (!UseReserveEnabled) UseReserveStakeMsg = "Not using reserve fill.";
            else UseReserveStakeMsg = "Using reserve fill.";
            RaisePropertyChangedEvent("UseReserveStake");
        }
    }
}
