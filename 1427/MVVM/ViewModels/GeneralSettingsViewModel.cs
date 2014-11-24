using System;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;

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

        public GeneralSettingsViewModel()
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
            TerminalAssetMsg = (TiToEnabled) ? "Change Asset" : "Warning: TiTo DISABLED";

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
            {
                HasRecycler = false;
                RecyclerMessage = "NO RECYCLER";
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
            int retries = 10;
            if (BoLib.getCountryCode() == BoLib.getUkCountryCodeC())
            {
                int rtp = Convert.ToInt32(newRtp as string);
                NativeWinApi.WritePrivateProfileString("Datapack", "DPercentage", rtp.ToString(), @Properties.Resources.machine_ini);

                if (NativeMD5.CheckFileType(@Properties.Resources.machine_ini))
                {
                    if (!NativeMD5.CheckHash(@Properties.Resources.machine_ini))
                    {
                        //make sure file in not read-only
                        if (NativeWinApi.SetFileAttributes(@Properties.Resources.machine_ini, NativeWinApi.FILE_ATTRIBUTE_NORMAL))
                        {
                            //delete [End] section
                            NativeWinApi.WritePrivateProfileSection("End", "", @Properties.Resources.machine_ini);
                            NativeWinApi.WritePrivateProfileSection("End", "", @Properties.Resources.machine_ini);

                            do
                            {
                                NativeMD5.AddHashToFile(@Properties.Resources.machine_ini);
                            }
                            while (!NativeMD5.CheckHash(@Properties.Resources.machine_ini) && retries-- > 0);
                        }
                    }
                }

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
            string type = o as string;
            var current = BoLib.getHandPayThreshold();
            var newVal = current;

            int denom = current-1000; //getvariablevalue(max_handpay_threshold);
            
            if (type == "increment")
                BoLib.setHandPayThreshold(current + 5000);
            else
                BoLib.setHandPayThreshold(current - 5000);

            
            this.HandPayLevel = (newVal / 100).ToString("C", Thread.CurrentThread.CurrentCulture.NumberFormat);
            this.RaisePropertyChangedEvent("HandPayLevel");
        }
    }
}
