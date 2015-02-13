using System;
using System.Net.NetworkInformation;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Management;
using System.Collections.Generic;

namespace PDTUtils.MVVM.ViewModels
{
    class NetworkSettingsViewModel : ObservableObject
    {
        bool ChangesMade { get; set; }

        public bool IPAddressActive { get; set; }
        public bool SubnetActive { get; set; }
        public bool DefaultActive { get; set; }
        public bool PingTestRunning { get; set; }
        
        public string IPAddress { get; set; }
        public string SubnetAddress { get; set; }
        public string DefaultGateway { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }

        public string PingOne { get; set; }
        public string PingTwo { get; set; }

        public NetworkSettingsViewModel()
        {
            ChangesMade = false;

            IPAddressActive = false;
            SubnetActive = false;
            DefaultActive = false;

            IPAddress = "";
            SubnetAddress = "";
            DefaultGateway = "";
            ComputerName = "";

            PingOne = "";
            PingTwo = "* Sending PING (Google DNS) #1 *";
            PingTestRunning = false;

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

                if (ni.OperationalStatus == OperationalStatus.Up && MacAddress == null)
                {
                    MacAddress += ni.GetPhysicalAddress().ToString();
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
            RaisePropertyChangedEvent("MacAddress");
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
                    PingTwo = "* Sending PING (Google DNS) #1 *";
                    PingOne = "";
                    RaisePropertyChangedEvent("PingOne");
                    RaisePropertyChangedEvent("PingTwo");
                }
                else if (index == 1 && PingOne.Length > 0)
                {
                    PingTwo = "* Sending PING (Google DNS) #2 *";
                    RaisePropertyChangedEvent("PingTwo");
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
                    {
                        PingOne += "\n\n";
                        PingTwo = "*** Internet Ping Test Completed ***";
                        RaisePropertyChangedEvent("PingTwo");
                    }

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
            ChangesMade = true;
            IPAddressActive = !IPAddressActive;
            RaisePropertyChangedEvent("IPAddressActive");
        }
        public ICommand ToggleSubnet { get { return new DelegateCommand(o => DoToggleSubnet()); } }
        void DoToggleSubnet()
        {
            ChangesMade = true;
            SubnetActive = !SubnetActive;
            RaisePropertyChangedEvent("SubnetActive");
        }
        public ICommand ToggleDefault { get { return new DelegateCommand(o => DoToggleDefault()); } }
        void DoToggleDefault()
        {
            ChangesMade = true;
            DefaultActive = !DefaultActive;
            RaisePropertyChangedEvent("DefaultActive");
        }
        
        public ICommand SaveNetworkInfo { get { return new DelegateCommand(o => DoSaveNetworkInfo()); } }
        void DoSaveNetworkInfo()
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            if (ChangesMade)
            {
                foreach (ManagementObject objMO in objMOC)
                {
                    if ((bool)objMO["IPEnabled"])
                    {
                        try
                        {
                            ManagementBaseObject setIP;
                            ManagementBaseObject newIP = objMO.GetMethodParameters("EnableStatic");
                            ManagementBaseObject newGateway = objMO.GetMethodParameters("SetGateways");

                            newIP["IPAddress"] = new string[] { IPAddress };
                            newIP["SubnetMask"] = new string[] { SubnetAddress };
                           // if ((string[])objMO["DefaultIPGateway"] != null)
                           //     newIP["DefaultIPGateway"] = new string[] { DefaultGateway };

                            setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
                ChangesMade = false;
                DiskCommit.Save();
            }
        }
    }
}
