using System;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;
using PDTUtils.Logic;
using System.Text;

namespace PDTUtils.MVVM.ViewModels
{
    class GeneralSettingsViewModel : ObservableObject
    {
        public bool IsCatC { get; set; }
        public bool TiToEnabled { get; set; }
        public bool HasRecycler { get; set; }
        public string RtpMessage { get; set; }
        public string HandPayLevel { get; set; }
        public string DivertMessage { get; set; }
        public string RecyclerMessage { get; set; }
        public string TerminalAssetMsg { get; set; }

        string _titoDisabledMsg = "Warning: TiTo DISABLED";

        public GeneralSettingsViewModel()
        {
            try
            {
                if (BoLib.getCountryCode() != BoLib.getSpainCountryCode())
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                else
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

                if (BoLib.getCountryCode() != BoLib.getUkCountryCodeC())
                {
                    this.RtpMessage = "CAT C NOT ENABLED";
                    this.IsCatC = false;
                }
                else
                {
                    this.RtpMessage = BoLib.getTargetPercentage().ToString() + "%";
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
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            this.RaisePropertyChangedEvent("HasRecycler");
            this.RaisePropertyChangedEvent("IsCatC");
            this.RaisePropertyChangedEvent("HandPayLevel");
            this.RaisePropertyChangedEvent("DivertMessage");
            this.RaisePropertyChangedEvent("RecyclerMessage");
            this.RaisePropertyChangedEvent("TerminalAssetMsg");
        }

        public ICommand SetRTP
        {
            get { return new DelegateCommand(ChangeRTP); }
        }
        void ChangeRTP(object newRtp)
        {
            if (BoLib.getCountryCode() == BoLib.getUkCountryCodeC())
            {
                int rtp = Convert.ToInt32(newRtp as string);
                NativeWinApi.WritePrivateProfileString("Datapack", "DPercentage", rtp.ToString(), @Properties.Resources.machine_ini);
                IniFileUtility.HashFile(@Properties.Resources.machine_ini);
                
                this.RtpMessage = rtp.ToString() + "%";
                this.RaisePropertyChangedEvent("RtpMessage");
            }
        }

        public ICommand ChangeHandPayThreshold
        {
            get { return new DelegateCommand(DoChangeHandayThreshold); }
        }
        public void DoChangeHandayThreshold(object o)
        {
            if (BoLib.getTerminalType() == BoLib.printer())
            {
                if (!BoLib.getHandPayActive())
                    return;
            }

            string type = o as string;
            int current = (int)BoLib.getHandPayThreshold();
            int newVal = current;
            
            int maxHandPay = (int)BoLib.getMaxHandPayThreshold();
            int denom = maxHandPay - current;
            int amount = (denom < 5000) ? denom : 5000;
            
            if (type == "increment")
            {
                BoLib.setHandPayThreshold((uint)current + (uint)amount);
                newVal += amount;
                NativeWinApi.WritePrivateProfileString("Config", "Handpay Threshold", (newVal).ToString(), @Properties.Resources.birth_cert);
            }
            else
            {
                BoLib.setHandPayThreshold((uint)current - (uint)amount);
                newVal -= amount;
                NativeWinApi.WritePrivateProfileString("Config", "Handpay Threshold", (newVal).ToString(), @Properties.Resources.birth_cert);
            }

            IniFileUtility.HashFile(@Properties.Resources.birth_cert);

            this.HandPayLevel = (newVal / 100).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            this.RaisePropertyChangedEvent("HandPayLevel");
        }

        public ICommand ChangeDivert { get { return new DelegateCommand(DoChangeDivert); } }
        public void DoChangeDivert(object o)
        {
            string actionType = o as string;
            var currentThreshold = BoLib.getHopperDivertLevel(0);
            uint changeAmount = 50;
            uint newValue = currentThreshold;

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
            NativeWinApi.WritePrivateProfileString("Config", "LH Divert Threshold", newValue.ToString(), @Properties.Resources.birth_cert);
            IniFileUtility.HashFile(@Properties.Resources.birth_cert);

            this.DivertMessage = (newValue).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            this.RaisePropertyChangedEvent("DivertMessage");
        }

        public ICommand Recycle { get { return new DelegateCommand(DoRecycleNote); } }
        public void DoRecycleNote(object o)
        {
            string noteType = o as string;
            string channel = "";

            if (BoLib.getBnvType() == 5)
            {
                channel = (noteType == "10") ? "2" : "3";
                BoLib.shellSendRecycleNote();
                NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", channel, @Properties.Resources.birth_cert);
                IniFileUtility.HashFile(@Properties.Resources.birth_cert);
                RecyclerMessage = (noteType == "10") ? "£10 Recycled" : "£20 Recycled";
                RaisePropertyChangedEvent("RecyclerMessage");
            }
        }

        public ICommand TiToState { get { return new DelegateCommand(ToggleTiToState); } }
        public void ToggleTiToState(object o)
        {
            string state = o as string;
            TiToEnabled = (state == "enabled") ? true : false;
            if (TiToEnabled) // enable
            {
                StringBuilder sb = new StringBuilder(20);
                var res = NativeWinApi.GetPrivateProfileString("Keys", "AssetNo", "", sb, sb.Capacity, @Properties.Resources.machine_ini);
                TerminalAssetMsg = sb.ToString();

                BoLib.setFileAction();

                TerminalAssetMsg = _titoDisabledMsg;
                NativeWinApi.WritePrivateProfileString("Config", "TiToEnabled", "1", @Properties.Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("Config", "PayoutType", "1", @Properties.Resources.birth_cert);
                BoLib.setTerminalType(1); //printer
                string printerType = "";
                string bnvType = "6";

                if (BoLib.getCabinetType() == 3) // ts22_2015
                    printerType = "3";
                else
                    printerType = "4";
                
                NativeWinApi.WritePrivateProfileString("Config", "PrinterType", printerType, @Properties.Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("Config", "BnvType", bnvType, @Properties.Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", "0", @Properties.Resources.birth_cert);
                BoLib.setRecyclerChannel(0);

                BoLib.clearFileAction();
            }
            else // disable
            {
                BoLib.setFileAction();

                TerminalAssetMsg = _titoDisabledMsg;
                NativeWinApi.WritePrivateProfileString("Config", "TiToEnabled", "1", @Properties.Resources.birth_cert);
                BoLib.setTitoState(1);
                NativeWinApi.WritePrivateProfileString("Config", "PayoutType", "1", @Properties.Resources.birth_cert);
                BoLib.setTerminalType(1); //printer
                string printerType = "";
                string bnvType = "6";

                if (BoLib.getCabinetType() == 3) // ts22_2015
                    printerType = "3";
                else
                    printerType = "4";

                NativeWinApi.WritePrivateProfileString("Config", "PrinterType", printerType, @Properties.Resources.birth_cert); // 3 = NV200_ST
                BoLib.setPrinterType(Convert.ToByte(printerType));
                NativeWinApi.WritePrivateProfileString("Config", "BnvType", bnvType, @Properties.Resources.birth_cert);
                BoLib.setBnvType(Convert.ToByte(bnvType));
                NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", "0", @Properties.Resources.birth_cert);
                BoLib.setRecyclerChannel(0);

                BoLib.clearFileAction();
            }

            RaisePropertyChangedEvent("TiToEnabled");
            RaisePropertyChangedEvent("TerminalAssetMsg");
            //write to ini file
        }

        public ICommand TitoUpdate { get { return new DelegateCommand(DoTitoUpdate); } }
        public void DoTitoUpdate(object o)
        {

        }
    }
}
