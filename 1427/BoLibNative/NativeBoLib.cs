﻿using System;
using System.Runtime.InteropServices;

namespace PDTUtils.Native
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GamesInfo
	{
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;
		[MarshalAs(UnmanagedType.LPStr)]
		public string hash_code;
		[MarshalAs(UnmanagedType.LPStr)]
		public string path;

		public string Name { get; set; }
		public string HashCode { get; set; }
		public string Path { get; set; }
	}
    
    [StructLayout(LayoutKind.Sequential)]
    struct SpanishRegional
    {
        public uint MaxStake;
        public uint MaxStakeFromBank;
        public uint StakeMask;
        public uint MaxWinPerStake;
        public uint MaxCredit;
        public uint MaxReserve;
        public uint MaxBank;
        public uint MaxPlayerPoints;
        public uint NoteEscrow;
        public uint Rtp;
        public uint Gtime;
        public uint ChangeValue;
        public uint MaxNote;
        public uint BankToCredits;
        public uint ChargeConvertPoints;
        public uint CycleSize;
        public uint FastTransfer;
        public uint GamesPerPeriod;
    }

    enum Hoppers
    {
        Left = 0x00,
        Middle = 0x01,
        Right = 0x02,
        HopperLeftMask = 0x10,
        NoHopper = 0x100
    }

    enum Performance
    {
        MoneyInLt = 0,
        MoneyOutLt,
        HandPayLt,
        CashboxLt,
        NoGamesLt,
        WageredLt,
        WonLt,
        MoneyInSt,
        MoneyOutSt,
        HandPaySt,
        CashboxSt,
        NoGamesSt,
        WageredSt,
        WonSt
    }
    
    enum GamePerformance
    {
        GameWageredLt = 0,
        GameWonLt,
        GamePlayLt,
        GameWageredSt,
        GameWonSt,
        GamePlaySt,
        MaxGameMeters,
        GameStOffSetP = (MaxGameMeters / 2)
    }
    
    enum ELongTermMeters
    {
        NoteSix = 0,
        NoteFive,
        NoteFour,
        NoteThree,
        NoteTwo,
        NoteOne,

        CoinEight,
        CoinSeven,
        CoinSix,
        CoinFive,
        CoinFour,
        CoinThree,
        CoinTwo,
        CoinOne,
        
        TitoIn,

        RefillL,
        RefillR,

        NoteOneOut,
        NoteTwoOut,
        NoteThreeOut,
        NoteFourOut,
        NoteFiveOut,
        NoteSixOut,

        CoinSixOut,
        CoinFiveOut,
        CoinFourOut,
        CoinThreeOut,
        CoinTwoOut,
        TicketOut
    }

    enum EShortTermMeters
    {
        NoteSix = 28,
        NoteFive,
        NoteFour,
        NoteThree,
        NoteTwo,
        NoteOne,

        CoinEight,
        CoinSeven,
        CoinSix,
        CoinFive,
        CoinFour,
        CoinThree,
        CoinTwo,
        CoinOne,
        
        TitoIn,

        RefillL,
        RefillR,

        NoteOneOut,
        NoteTwoOut,
        NoteThreeOut,
        NoteFourOut,
        NoteFiveOut,
        NoteSixOut,

        CoinSixOut,
        CoinFiveOut,
        CoinFourOut,
        CoinThreeOut,
        CoinTwoOut,
        TicketOut
    }
    
    enum EspRegionalBase
    {
        MaxStakeCredits = 0,
        MaxStakeBank,
        StakeMask,
        GambleWinFactor,
        MaxCredits,
        MaxReserveCredits,
        MaxBank,
        EscrowState,
        Rtp,
        GameTime,
        GiveChangeThreshold,
        MaxBankNote,
        AllowBankToCredit,
        ChargeConverPlayerPoints,
        FastTransfer,
        Cycle,
        MaxPlayerPoints,
        GphPeriod
    }
    
    
    enum EspRegionalExt
    {
        EspAlwaysFichas = 18,
        EspAutoTfxToStake = 19
    }

    enum UtilBits
    {
        Allow               = 0x00000001,
        EEPromUpdate        = 0x00000002,
        ChangeRnv           = 0x00000004,
        EmptyRecycler       = 0x00000008,
        RecyclerValue       = 0x00000010,
        TitoAudit           = 0x00000020,
        AddToCredit         = 0x00000040,
        RefillCoins         = 0x00000080,
        CoinTest            = 0x00000100,
        NoteTest            = 0x00000200,
        PrintTicket         = 0x00000400,
        RereadBirthCert     = 0x00000800,
        TopupLeftHopper	    = 0x00001000,
        TopupRightHopper    = 0x00002000,
        DumpLeftHopper      = 0x00004000,
        DumpRightHopper     = 0x00008000,
        TestLeftHopper      = 0x00010000,
        TestRightHopper     = 0x00020000,
        ReadCpuEventsBit    = 0x00040000
    }
    
    static class BoLib
    {
#if DEBUG
        const string DLL_NAME = "BoLibDllD.dll";
#else
        const string DLL_NAME = "BoLibDll.dll";
#endif
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int setEnvironment();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void closeSharedMemory();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDoorStatus();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCountryCode();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getCountryCodeStr();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int refillKeyStatus();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getError();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getCurrentError();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getErrorText();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getLastGame(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern UInt32 getWinningGame(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getPerformanceMeter(byte offset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getGamePerformanceMeter(uint offset, uint meterType);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getLocalMasterVolume();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern UInt32 getGameModel(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getGameTime(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getGameDate(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGameCreditLevel(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGameWager(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getSwitchStatus(byte offset, byte mask);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCredit();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getBank();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getLastNote(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint[] getLastNotes();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHopperFloatLevel(byte hopper);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHopperDivertLevel(byte hopper);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getHopperDumpSwitchActive();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getHopperDumpSwitch();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRequestEmptyLeftHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRequestEmptyRightHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getBnvType();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRecyclerFloatValue();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getRefillCtr(byte hopper);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getLeftHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getMiddleHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getRightHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getMinPayoutValue();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getCashIn(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getCashOut(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getNotesIn(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getNotesOut(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getRefillValue(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getVtp(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getWon(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHandPay(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getTicketsPay(int meter);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getSerialNumber();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getReconciliationMeter(byte offset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getEDCTypeStr();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumberOfGames();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getBoLibVersion();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getRegionalValues(int index, ref SpanishRegional region);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDefaultRegionValues(int index, ref SpanishRegional region);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getActiveRegionValues(int index, ref SpanishRegional region);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getErrorMessage(string str, int code);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUtilsRelease();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getTPlayMeter(byte offset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUkCountryCodeB3();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUkCountryCodeC();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getSpainCountryCode();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTargetPercentage();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getRecyclerChannel();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getMaxHandPayThreshold();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCabinetType();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTerminalType();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte combined();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte hopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte printer();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getLiveElement(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDefaultElement(int region, int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getTerminalFormat();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getUtilsAdd2CreditValue();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getLastGameModel(int index);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getReserveCredits();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isBackOfficeAvilable();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoStateValue();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool canPerformHandPay();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getWinningGameMeter(int offset, int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getHistoryLength();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern string getLicense();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern string getCountryCodeStrLiteral(string str, int code);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getEspRegionalVariableValue(int value);

        /**
         ** Gets the bank, credit and reserve POINTER value not nvr_main.
         **/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getBankCreditsReservePtr();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getIsHopperHopping();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getUtilRequestBitState(int bit);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getSmartCardGroup();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getUtilsAccessLevel();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getUtilDoorAccess();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getUtilRefillAccess();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getUtilSmartCardAccess(int whichBit);
        
        /************************************************************************/
        /*							Set methods                                 */
        /************************************************************************/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearBankAndCredit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLocalMasterVolume(uint val);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLampStatus(byte offset, byte mask, byte state);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHopperFloatLevel(byte hopper, uint value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestEmptyHopper(int hopper);
        /*public static extern void setRequestEmptyLeftHopper();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestEmptyRightHopper();*/

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addCredit(int pennies);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clearError();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCriticalError(int code);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void transferBankToCredit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearShortTermMeters();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHopperDivertLevel(byte hopper, uint value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shellSendRecycleNote();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPrinterType(byte type);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRecyclerChannel(byte value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBnvType(byte value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRebootRequired();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setUtilsAdd2CreditValue(uint value);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestUtilsAdd2Credit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setEspRegionalValue(uint query, ulong value);

        /*[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPayoutCoinValues(uint which, uint value);*/
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableUtilsCoinBit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void disableUtilsCoinBit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shellSendEmptyRecycler();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearBankCreditReserve();
        
        /*[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void oogaDeBooga();*/

        /*[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ESPHandPay();*/
        /************************************************************************/
        /* General methods                                                      */
        /************************************************************************/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableNoteValidator();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void disableNoteValidator();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void printTestTicket();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getPrinterTicketState();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getBnvStringType(byte bnv);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useMoneyInType(int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useMoneyOutType(int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useRefillType(int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useVtpMeter(int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useWonMeter(int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useHandPayMeter(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useTicketsMeter(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong useStakeInMeter(int meter);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetUniquePcbID(byte type);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setFileAction();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearFileAction();

        [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
        public static extern void setTerminalType(byte type);

        /*[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setUtilBit(int bit);*/

        //[DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
        //public static extern void clearUtilBit(int bit);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setUtilRequestBitState(int bit);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearUtilRequestBitState(int bit);

        /************************************************************************/
        /* Hand Pay methods                                                     */
        /************************************************************************/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHandPayThreshold(uint value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHandPayThreshold();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getHandPayActive();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sendHandPayToServer(uint paidOut, uint release);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addHandPayToEDC(uint value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool performHandPay();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void cancelHandPay();


        /************************************************************************/
        /* TITO methods                                                         */
        /************************************************************************/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getTitoEnabledState();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoHost();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoProcessInState();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoTicketPresented();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setTitoState(int state);
        

        /************************************************************************/
        /* DirectSound methods - hear me now, praise de Lord!                   */
        /************************************************************************/
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DirectSoundInit();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DirectSoundShutdown();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadWavFile(string filename, int control_flags);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DoPlaySound(); //always gonna be sound zero in utils.
    }
}
