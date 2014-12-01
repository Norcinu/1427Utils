using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using PDTUtils.Native;
using System.Windows.Input;

namespace PDTUtils.MVVM.ViewModels
{
    class NetworkSettingsViewModel : ObservableObject
    {
        public bool IPAddressActive { get; set; }
        public bool SubnetActive { get; set; }
        public bool DefaultActive { get; set; }

        public string IPAddress { get; set; }
        public string SubnetAddress { get; set; }
        public string DefaultGateway { get; set; }
        public string ComputerName { get; set; }

        public NetworkSettingsViewModel()
        {
            IPAddressActive = true;
            SubnetActive = false;
            DefaultActive = false;

            IPAddress = "";
            SubnetAddress = "";
            DefaultGateway = "";
            ComputerName = "";
            PopulateInfo();
        }

        void PopulateInfo()
        {
            //IP Address
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
					ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
					{
						if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							IPAddress += ip.Address.ToString();
                            SubnetAddress = ip.IPv4Mask.ToString();
                            DefaultGateway = ni.GetIPProperties().GatewayAddresses[0].Address.ToString();
						}
					}
				}
			}
        
            ComputerName = System.Environment.MachineName;

            RaisePropertyChangedEvent("IPAddressActive");
            RaisePropertyChangedEvent("SubnetActive");
            RaisePropertyChangedEvent("DefaultActive");
            
            RaisePropertyChangedEvent("IPAddress");
            RaisePropertyChangedEvent("ComputerName");
            RaisePropertyChangedEvent("SubnetAddress");
            RaisePropertyChangedEvent("DefaultGateway");
        }

        public ICommand ToggleIP
        {
            get
            {
                Action<object> clart = (object b) => this.IPAddressActive = !this.IPAddressActive;
                RaisePropertyChangedEvent("IPAddressActive");
                return new DelegateCommand(clart);
            }
        }
        public ICommand ToggleSubnet { get { return new DelegateCommand(o => this.SubnetActive = !this.SubnetActive); } }
        public ICommand ToggleDefault { get { return new DelegateCommand(o => this.DefaultActive = !this.DefaultActive); } }
    }
}
