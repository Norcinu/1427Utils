using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace PDTUtils.Views
{
    public class SoftwareInfo
    {
        public string ModelNumber { get; set; }
        public string Authed { get; set; }
        public string HashCode { get; set; }

        public SoftwareInfo(string m, string a, string h)
        {
            this.ModelNumber = m;
            this.Authed = a;
            this.HashCode = h;
        }
    }

    public class HardwareInfo
    {
        public string SerialKey { get; set; }
        public string MachineName { get; set; }
        public string License { get; set; }
        public string CpuType { get; set; }
        public string CabinetType { get; set; }
        public string IPAddress { get; set; }
        public string Subnet { get; set; }
        public string DefGateway { get; set; }

        public HardwareInfo(string sk, string mn, string l, string cpu, string ct)
        {
            this.SerialKey = sk;
            this.MachineName = mn;
            this.License = l;
            this.CpuType = cpu;
            this.CabinetType = ct;

            this.IPAddress = "192.168.1.3";
            this.Subnet = "255.255.0.0";
            this.DefGateway = "169.254.1.1";
        }
    }
    /// <summary>
    /// Interaction logic for Diagnostics.xaml
    /// </summary>
    public partial class Diagnostics : UserControl
    {
        public Diagnostics()
        {
            InitializeComponent();
            this.DataContext = this;
            _software.Add(new SoftwareInfo("1424", "d4ec0b9ecb3a9cef0e34f8f0baaf5b13", "Authed OK"));
            _software.Add(new SoftwareInfo("1427", "d56fc00bbb954950f9259bba663f783a", "Authed OK"));
            _software.Add(new SoftwareInfo("1222", "cb6865575732a4f2b34d88296acbc139", "Authed FAILED"));
            _software.Add(new SoftwareInfo("1202", "5ad2f9afb460663caef2e40092a81cf4", "Authed OK"));
            _software.Add(new SoftwareInfo("1172", "197a5baaaa1d62d1ce5393036d72ea5e", "Authed OK"));
            _software.Add(new SoftwareInfo("1074", "d1dffea7f44516b35a1b082de43de56c", "Authed OK"));
            _software.Add(new SoftwareInfo("1077", "f5de9a3527b0bd6e5ca739732e5c3dc5", "Authed OK"));
            _software.Add(new SoftwareInfo("1199", "5f23e92850e30f5d9eee14d5cd407d1f", "Authed OK"));

            _hardware.Add(new HardwareInfo("76505", "TERMINAL_01", "Development", "S430", "TS22 - L28"));
        }

        ObservableCollection<SoftwareInfo> _software = new ObservableCollection<SoftwareInfo>();
        public ObservableCollection<SoftwareInfo> Software { get { return _software; } }

        ObservableCollection<HardwareInfo> _hardware = new ObservableCollection<HardwareInfo>();
        public ObservableCollection<HardwareInfo> Hardware { get { return _hardware; } }
        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var tab = sender as TabControl;
            if (tab.SelectedIndex == 0)
                this.DataContext = this;
            else if (tab.SelectedIndex == 1)
                this.DataContext = new MachineLogsController();*/
        }
    }
}
