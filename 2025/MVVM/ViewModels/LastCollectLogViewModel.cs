﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    enum HopperPayoutNames
    {
        LeftHandCoinValue       = 0,
        LeftHandCoinCount       = 1,
        RightHandCoinValue      = 2,
        RightHandCoinCount      = 3,
        NoteValue               = 4,
        NoteCount               = 5,
        InitBankValue           = 6,
        InitPartCollectValue    = 7,
        InitCreditValue         = 8,
        HandPayValue            = 9
    }

    enum TicketPayoutNames
    {
        RSTicketModelNo             = 0,
        RSTicketNumber              = 1,
        RSTicketBarcode             = 2, //an array of 32 ints
        RSticketDuplicateNumber     = 3,
        RSPrintProgress             = 4,
        RSPrinterStatus             = 5,
        StartPrinterBankValue       = 6,
        StartPrintPartCollectValue  = 7,
        StartPrintCreditValue       = 8
    }

    public class LastCollectLogViewModel : ObservableObject
    {
        bool _hopperPayout = false;
        bool _ticketPayout = false;
        bool _showListView = false;
        bool _errorMessageActive = false;

        int _notesPaid = 0;
        int _leftHopperCoinsPaid = 0;
        int _rightHopperCoinsPaid = 0;
        int _totalPaidOut = 0;
        int _handPaidOut = 0;

        string _errorMessage = "";
        string _payoutFile = ""; //Properties.Resources.payout;

        DateTime _payoutDate = DateTime.Now;

        Dictionary<string, Pair<int, int>> _entries = new Dictionary<string, Pair<int, int>>();

        #region PROPERTIES
        public bool ShowListView { get; set; }
        public ObservableCollection<string> LastCollect { get; set; }

        public bool ErrorMessageActive
        {
            get { return _errorMessageActive; }
            set
            {
                _errorMessageActive = value;
                RaisePropertyChangedEvent("ErrorMessageActive");
            }
        }
        
        public bool HopperPayout
        {
            get { return _hopperPayout; }
            set
            {
                _hopperPayout = value;
                RaisePropertyChangedEvent("HopperPayout");
            }
        }

        public bool TicketPayout
        {
            get { return _ticketPayout; }
            set
            {
                _ticketPayout = value;
                RaisePropertyChangedEvent("TicketPayout");
            }
        }

        public string PayoutDate
        {
            get { return _payoutDate.ToShortTimeString() + " " + _payoutDate.ToShortDateString(); }
            set
            {
                //_payoutDate = value;
                RaisePropertyChangedEvent("PayoutDate");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChangedEvent("ErrorMessage");
            }
        }
        
        public int LeftHandCoinsPaid
        {
            get { return _leftHopperCoinsPaid; }
            set
            {
                _leftHopperCoinsPaid = value;
                RaisePropertyChangedEvent("LeftHandCoinsPaid");
            }
        }

        public int RightHandCoinsPaid
        {
            get { return _rightHopperCoinsPaid; }
            set
            {
                _rightHopperCoinsPaid = value;
                RaisePropertyChangedEvent("RightHandCoinsPaid");
            }
        }

        public int TotalPaidOut
        {
            get { return _totalPaidOut; }
            set
            {
                _totalPaidOut = value;
                RaisePropertyChangedEvent("TotalPaidOut");
            }
        }

        public int NotesPaidOut
        {
            get { return _notesPaid; }
            set
            {
                _notesPaid = value;
                RaisePropertyChangedEvent("NotesPaid");
            }
        }

        public int HandPaidOut
        {
            get { return _handPaidOut; }
            set
            {
                _handPaidOut = value;
            }
        }
        
        public Dictionary<string, Pair<int, int>> Entries
        {
            get { return _entries; }
        }

        #endregion

        public LastCollectLogViewModel()
        {
            
            LastCollect = new ObservableCollection<string>();
            ShowListView = false;
            RaisePropertyChangedEvent("ShowListView");
            
            DoLoadLog();
        }

        bool TestCheckSums(int liveChecksum, int finalChecksum)
        {
            if (liveChecksum != finalChecksum)
            {
                if (LastCollect.Count > 0)
                    LastCollect.Clear();

                LastCollect.Add("ERROR: Checksums do not match.");
                RaisePropertyChangedEvent("LastCollect");
                return false;
            }
            return true;
        }
        
        void HopperCollectPayout(ref List<int> wagwan, ref int liveChecksum, ref int finalChecksum)
        {
            BoLib.setFileAction();
            try
            {
                using (var b = new BinaryReader(File.Open(@_payoutFile, FileMode.Open)))
                {
                    int position = 0;
                    int length = (int)b.BaseStream.Length;
                    while (position < length)
                    {
                        var value = b.ReadInt32();
                        wagwan.Add(value);

                        if (position != length - sizeof(int))
                            liveChecksum += value;

                        position += sizeof(int);
                    }
                }

                finalChecksum = wagwan[wagwan.Count - 1];
                if (TestCheckSums(liveChecksum, finalChecksum))
                {
                    var attr = File.GetAttributes(_payoutFile);
                    //PayoutDate = File.GetLastWriteTime(_payoutFile);
                    _payoutDate = File.GetLastWriteTime(_payoutFile);
                    LeftHandCoinsPaid = wagwan[(int)HopperPayoutNames.LeftHandCoinCount] * wagwan[(int)HopperPayoutNames.LeftHandCoinValue];
                    RightHandCoinsPaid = wagwan[(int)HopperPayoutNames.RightHandCoinCount] * wagwan[(int)HopperPayoutNames.RightHandCoinValue];
                    NotesPaidOut = wagwan[(int)HopperPayoutNames.NoteValue]; // *(int)HopperPayoutNames.NoteCount;
                    HandPaidOut = wagwan[(int)HopperPayoutNames.HandPayValue];
                    Entries.Add("Left Hand Coins", new Pair<int, int>(LeftHandCoinsPaid, 0));
                    Entries.Add("Right Hand Coins", new Pair<int, int>(RightHandCoinsPaid, 0));
                    Entries.Add("Notes Paid Out", new Pair<int, int>(NotesPaidOut, 0));
                    Entries.Add("Total Paid Out", new Pair<int, int>(LeftHandCoinsPaid + RightHandCoinsPaid + NotesPaidOut, 0));
                    RaisePropertyChangedEvent("PayoutDate");
                    RaisePropertyChangedEvent("Entries");
                }
                else
                {
                    ErrorMessage = "ERROR: CHECKSUM MISMATCH";
                    ErrorMessageActive = true;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                ErrorMessageActive = true;
            }

            BoLib.clearFileAction();
        }
        
        //!!! TODO COMPLETE THIS
        void TicketCollectPayout(ref List<int> wagwan, ref List<int>ticketNumber, ref int liveChecksum, ref int finalChecksum)
        {
            BoLib.setFileAction();
            //using (var b = new BinaryReader(File.Open(@_ticketPayout, FileMode.Open)))
            {
                /*int position = 0;
                int length = (int)b.BaseStream.Length;
                while (position < length)
                {
                    if (position != (sizeof(int) * 2)) //3?
                    {
                        var value = b.ReadInt32();
                        wagwan.Add(value);
                        if (position != length - sizeof(int))
                            liveChecksum += value;
                    }
                    else
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            var value = b.ReadInt32();
                            ticketNumber.Add(value);
                            liveChecksum += value;
                        }
                    }
                }*/
            }
            BoLib.clearFileAction();
        }
        
        void DoLoadLog()
        {
            var liveChecksum = 0;
            var finalChecksum = 0;
            var wagwan = new List<int>();

           // if (BoLib.getLastPayoutType() == (int)CollectType.Hopper) // need an enum of these payout types.
            {
                _payoutFile = Properties.Resources.payout;
                HopperCollectPayout(ref wagwan, ref liveChecksum, ref finalChecksum);
            }
           /* else
            {
                var ticketNumber = new List<int>();
                _payoutFile = Properties.Resources.payout_ticket;
                TicketCollectPayout(ref wagwan, ref ticketNumber, ref liveChecksum, ref finalChecksum);
            }*/
            
            ShowListView = true;
            RaisePropertyChangedEvent("ShowListView");
        }

        public System.Windows.Input.ICommand LoadLog { get { return new DelegateCommand(o => DoLoadLog()); } }
    }
}
