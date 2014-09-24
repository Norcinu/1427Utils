using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.MVVM.Models
{
    enum ESiteType { StreetMarket = 1, Arcade }

    /*class RegionalSettingsModel
    {
        public bool BillEscrow { get; set; }
        public bool ReturnChange { get; set; }
        public bool NoteAcceptTwenty { get; set; }
        public bool NoteAcceptFifty { get; set; }
        public bool BankToCredits { get; set; }
        
        public uint Stake { get; set; }
        public uint MaxPricePerStake { get; set; }
        public uint MaxCredits { get; set; }
        public uint MaxReserve { get; set; }
        public uint MaxBank { get; set; }
        public uint Cycle { get; set; }

        public string Community { get; set; }
        public ESiteType SiteType { get; set; }

        //public uint MultipleStake { get; set; } how to handle this.
        public RegionalSettingsModel()
        {

        }
    }*/

    class SpanishRegionalModel
    {
        public uint MaxStakeFromCredits { get; set;}
        public uint MaxStakeFromBank { get; set; }
        public uint StakeInc { get; set; }
        public uint MaxWinPerStake { get; set; }
        public uint MaxCredit { get; set; }
        public uint MaxReserve { get; set; }
        public uint MaxBank { get; set; }
        public uint NoteEscrow { get; set; }
        public uint Rtp { get; set; }
        public uint Gtime { get; set; }
        public uint ChangeValue { get; set; }
        public uint MaxNote { get; set; }
        public uint CreditAndBank { get; set; }
        public uint ChargeConvertPoints { get; set; }
    }
}
