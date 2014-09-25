using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PDTUtils.Native;

namespace PDTUtils.MVVM.Models
{
    enum ESiteType { StreetMarket = 1, Arcade }

    class SpainRegionSelection
    {
        public string Community { get; set; }
        public string Index { get; set; }
    }

    class SpanishRegionalModel
    {
        public uint MaxStakeFromCredits { get; set; }
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
        public string Community { get; set; }

        public SpanishRegionalModel(string community, SpanishRegional region)
        {
            this.Community = community;
            this.ChangeValue = region.ChangeValue;
            this.ChargeConvertPoints = region.ChargeConvertPoints;
            this.CreditAndBank = region.CreditAndBank;
            this.Gtime = region.Gtime;
            this.MaxBank = region.MaxBank;
            this.MaxCredit = region.MaxCredit;
            this.MaxReserve = region.MaxReserve;
            this.MaxStakeFromBank = region.MaxStakeFromBank;
            this.MaxStakeFromCredits = region.MaxWinPerStake;
            this.MaxWinPerStake = region.MaxWinPerStake;
            this.NoteEscrow = region.NoteEscrow;
            this.Rtp = region.Rtp;
            this.StakeInc = region.StakeInc;
        }
    }
}
