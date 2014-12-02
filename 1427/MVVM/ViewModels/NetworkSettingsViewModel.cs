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
            IPAddressActive = false;
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
                            IPAddress = ip.Address.ToString();
                            SubnetAddress = ip.IPv4Mask.ToString();
                            DefaultGateway = ni.GetIPProperties().GatewayAddresses[0].Address.ToString();
						}
					}
				}
			}
        
            ComputerName = System.Environment.MachineName;
            
            PingSites(0); //move this to a command. but the commands are firing at startup?

            RaisePropertyChangedEvent("IPAddressActive");
            RaisePropertyChangedEvent("SubnetActive");
            RaisePropertyChangedEvent("DefaultActive");
            
            RaisePropertyChangedEvent("IPAddress");
            RaisePropertyChangedEvent("ComputerName");
            RaisePropertyChangedEvent("SubnetAddress");
            RaisePropertyChangedEvent("DefaultGateway");
        }
        
        public void PingSites(int index)
        {
            try
            {
                // ping google dns. Add more - non google sources?
                System.Net.IPAddress[] addies = new System.Net.IPAddress[2]
                {
                    System.Net.IPAddress.Parse("8.8.8.8"),
                    System.Net.IPAddress.Parse("8.8.4.4"),
                };

                Ping pinger = new Ping();
                PingReply reply = pinger.Send(addies[index]);

                if (reply.Status == IPStatus.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Host Not Reached {0}", "[" + addies[index].ToString() + "]");
                    if (index == 0)
                        PingSites(index + 1); // Suck my recursion.
                }
            }
            catch (Exception ex)
            {
                // put this message on screen.
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        
        public ICommand ToggleIP { get { return new DelegateCommand(o => DoToggleIP()); } }
        void DoToggleIP()
        {
            IPAddressActive = !IPAddressActive;
            RaisePropertyChangedEvent("IPAddressActive");
        }
        public ICommand ToggleSubnet { get { return new DelegateCommand(o => DoToggleSubnet()); } }
        void DoToggleSubnet()
        {
            SubnetActive = !SubnetActive;
            RaisePropertyChangedEvent("SubnetActive");
        }
        public ICommand ToggleDefault { get { return new DelegateCommand(o => DoToggleDefault()); } }
        void DoToggleDefault()
        {
            DefaultActive = !DefaultActive;
            RaisePropertyChangedEvent("DefaultActive");
        }
    }
}
