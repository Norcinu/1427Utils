﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.MVVM.Models
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
}