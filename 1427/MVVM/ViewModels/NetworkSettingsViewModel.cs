using System;
using System.Net.NetworkInformation;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;

namespace PDTUtils.MVVM.ViewModels
{
    class NetworkSettingsViewModel : ObservableObject
    {
        public bool IPAddressActive { get; set; }
        public bool SubnetActive { get; set; }
        public bool DefaultActive { get; set; }
        public bool PingTestRunning { get; set; }

        public string IPAddress { get; set; }
        public string SubnetAddress { get; set; }
        public string DefaultGateway { get; set; }
        public string ComputerName { get; set; }

        public string PingOne { get; set; }
        public string PingTwo { get; set; }

        public NetworkSettingsViewModel()
        {
            IPAddressActive = false;
            SubnetActive = false;
            DefaultActive = false;

            IPAddress = "";
            SubnetAddress = "";
            DefaultGateway = "";
            ComputerName = "";

            PingOne = "";
            PingTwo = "*** Sending PING (Google DNS) ***";
            PingTestRunning = false;

            PopulateInfo();
        }
        //
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
            
            //PingSites(0); //move this to a command. but the commands are firing at startup?

            RaisePropertyChangedEvent("IPAddressActive");
            RaisePropertyChangedEvent("SubnetActive");
            RaisePropertyChangedEvent("DefaultActive");
           
            RaisePropertyChangedEvent("IPAddress");
            RaisePropertyChangedEvent("ComputerName");
            RaisePropertyChangedEvent("SubnetAddress");
            RaisePropertyChangedEvent("DefaultGateway");
            RaisePropertyChangedEvent("PingTestRunning");
        }

        public delegate void SuckMyDelegate(string p, string text);
        public void SuckTheDelegate(string p, string text)
        {
            p = text;
            RaisePropertyChangedEvent("PingOne");
        }

        public ICommand PingSites { get { return new DelegateCommand(DoPingSites); } }
        public void DoPingSites(object o)
        {
            Thread t = new Thread(() => _DoPingSite(o));
            t.Start();
        }

        private void _DoPingSite(object o)
        {
            try
            {
                int? indexer = o as int?;
                int index = (indexer == null) ? 0 : indexer.Value;
                // ping google dns. Add more - non google sources?
                System.Net.IPAddress[] addies = new System.Net.IPAddress[2]
                {
                    System.Net.IPAddress.Parse("8.8.8.8"),
                    System.Net.IPAddress.Parse("8.8.4.4"),
                };

                if (index == 0 && PingOne.Length > 0)
                {
                    PingOne = "";
                    RaisePropertyChangedEvent("PingOne");
                }
                
                PingTestRunning = true;
                RaisePropertyChangedEvent("PingTestRunning");
                
                Ping pinger = new Ping();
                PingReply reply = pinger.Send(addies[index]);
                
                if (reply.Status == IPStatus.Success)
                {
                    PingOne += "Ping to " + addies[index].ToString() + " OK - " + reply.Status.ToString() + "\n\n";
                    RaisePropertyChangedEvent("PingOne");
                }
                else
                {
                    if (index == 1)
                        PingOne += "\n\n";

                    System.Diagnostics.Debug.WriteLine("Host Not Reached {0}", "[" + addies[index].ToString() + "]");
                    PingOne += "Ping to " + addies[index].ToString() + " FAILED - " + reply.Status.ToString();
                    RaisePropertyChangedEvent("PingOne");
                    
                    if (index == 0)
                        DoPingSites(index + 1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                PingOne = ex.Message;
                RaisePropertyChangedEvent("PingOne");
            }
            //PingTestRunning = false;
            RaisePropertyChangedEvent("PingTestRunning");
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
