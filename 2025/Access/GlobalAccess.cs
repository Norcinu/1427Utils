using System.Collections;

namespace PDTUtils.Access
{
    enum SmartCardGroups
    {
        Player = 0x0, Technician = 0x01, Cashier = 0x02, Admin = 0x03, 
        Operator = 0x04, Distributor = 0x05, Manufacturer = 0x06, None = 0x7
    }
    
    static class GlobalAccess  //: PDTUtils.MVVM.ObservableObject
    {
        static int _level = (int)SmartCardGroups.None;
        static object _spinLock = new object();
        static BitArray _accessBits = new BitArray(8);
        
        public static int Level
        {
            get { return _level; }
            set
            {
                lock (_spinLock)
                {
                    _level = value;
                    //RaisePropertyChangedEvent("Level");
                }
            }
        }
        
       /* public static BitArray AccessBits
        {
            get { return _accessBits; }
            set
            {
                _accessBits[value] = !_accessBits[value];
            }
        }*/
    }
}
