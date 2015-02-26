using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Converters;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    internal class NetworkSettingsViewModel : ObservableObject
    {
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

        private bool ChangesMade { get; set; }
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

        public ICommand PingSites
        {
            get { return new DelegateCommand(DoPingSites); }
        }

        public ICommand ToggleIP
        {
            get { return new DelegateCommand(o => DoToggleIP()); }
        }

        public ICommand ToggleSubnet
        {
            get { return new DelegateCommand(o => DoToggleSubnet()); }
        }

        public ICommand ToggleDefault
        {
            get { return new DelegateCommand(o => DoToggleDefault()); }
        }

        public ICommand SaveNetworkInfo
        {
            get { return new DelegateCommand(o => DoSaveNetworkInfo()); }
        }

        private void PopulateInfo()
        {
            //IP Address
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
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

            ComputerName = Environment.MachineName;

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

        public void DoPingSites(object o)
        {
            var t = new Thread(() => _DoPingSite(o));
            t.Start();
        }

        private void _DoPingSite(object o)
        {
            try
            {
                var indexer = o as int?;
                var index = indexer ?? 0;
                // ping google dns. Add more - non google sources?
                var addies = new IPAddress[3]
                {
                    System.Net.IPAddress.Parse("8.8.8.8"), // Google 1 
                    System.Net.IPAddress.Parse("8.8.4.4"), // Google 2
                    System.Net.IPAddress.Parse("169.254.1.1") // Internal Back Office
                };

                if (BoLib.isBackOfficeAvilable())
                {
                    PingTwo = "* Sending PING to  Back Office *";
                    PingOne = "";
                    RaisePropertyChangedEvent("PingOne");
                    RaisePropertyChangedEvent("PingTwo");
                    index = 2;
                }
                else
                {
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
                }

                PingTestRunning = true;
                RaisePropertyChangedEvent("PingTestRunning");

                var pinger = new Ping();
                var reply = pinger.Send(addies[index]);

                if (reply.Status == IPStatus.Success)
                {
                    PingOne += "Ping to " + addies[index] + " OK - " + reply.Status + "\n\n";
                    RaisePropertyChangedEvent("PingOne");
                }
                else
                {
                    if (index == 1) // index == 1
                    {
                        PingOne += "\n\n";
                        PingTwo = "*** Internet Ping Test Completed ***";
                        RaisePropertyChangedEvent("PingTwo");
                    }
                    else if (index == 2)
                    {
                        PingOne += "\n\n";
                        PingTwo = "*** Back Office Ping Test Completed ***";
                        RaisePropertyChangedEvent("PingTwo");
                    }
                    

                    Debug.WriteLine("Host Not Reached {0}", "[" + addies[index] + "]");
                    PingOne += "Ping to " + addies[index] + " FAILED - " + reply.Status;
                    RaisePropertyChangedEvent("PingOne");

                    if (index == 0)
                        DoPingSites(index + 1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                PingOne = ex.Message;
                RaisePropertyChangedEvent("PingOne");
            }
            //PingTestRunning = false;
            RaisePropertyChangedEvent("PingTestRunning");
        }

        private void DoToggleIP()
        {
            ChangesMade = true;
            IPAddressActive = !IPAddressActive;
            RaisePropertyChangedEvent("IPAddressActive");
        }

        private void DoToggleSubnet()
        {
            ChangesMade = true;
            SubnetActive = !SubnetActive;
            RaisePropertyChangedEvent("SubnetActive");
        }

        private void DoToggleDefault()
        {
            ChangesMade = true;
            DefaultActive = !DefaultActive;
            RaisePropertyChangedEvent("DefaultActive");
        }

        private void DoSaveNetworkInfo()
        {
            var objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var objMOC = objMC.GetInstances();

            if (ChangesMade)
            {
                foreach (ManagementObject objMO in objMOC)
                {
                    if ((bool) objMO["IPEnabled"])
                    {
                        try
                        {
                            ManagementBaseObject setIP;
                            var newIP = objMO.GetMethodParameters("EnableStatic");
                            var newGateway = objMO.GetMethodParameters("SetGateways");

                            newIP["IPAddress"] = new[] {IPAddress};
                            newIP["SubnetMask"] = new[] {SubnetAddress};
                            // if ((string[])objMO["DefaultIPGateway"] != null)
                            //     newIP["DefaultIPGateway"] = new string[] { DefaultGateway };

                            setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                ChangesMade = false;
                DiskCommit.Save();
            }
        }
    }
}