
namespace PDTUtils.MVVM.Models
{
    class CashReconiliation
    {
        public string Name { get; set; }
        public string LongTermValue { get; set; }
        public string ShortTermValue { get; set; }

        public CashReconiliation()
        {
            Name = "";
            LongTermValue = "";
        }
        
        public CashReconiliation(string n, string l, string s)
        {
            Name = n;
            LongTermValue = l;
            ShortTermValue = s;
        }
    }
}
