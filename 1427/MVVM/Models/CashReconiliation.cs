﻿
namespace PDTUtils.MVVM.Models
{
    class HelloImJohnnyCashMeters
    {
        public string Name { get; set; }
        public string LongTermValue { get; set; }
        public string ShortTermValue { get; set; }

        public HelloImJohnnyCashMeters()
        {
            Name = "";
            LongTermValue = "";
        }
        
        public HelloImJohnnyCashMeters(string n, string l, string s)
        {
            Name = n;
            LongTermValue = l;
            ShortTermValue = s;
        }
    }

    class PerformanceMeter
    {
        public string Name { get; set; }
        public long LongTermValue { get; set; }
        public long ShortTermValue { get; set; }

        public PerformanceMeter()
        {
            Name = "";
            ShortTermValue = 0;
            LongTermValue = 0;
        }

        public PerformanceMeter(string n, long l, long s)
        {
            Name = n;
            LongTermValue = l;
            ShortTermValue = s;
        }
    }
}
