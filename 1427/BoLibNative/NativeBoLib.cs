using System;
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
    
    //[StructLayout(LayoutKind.Explicit)]
    [StructLayout(LayoutKind.Sequential)]
    struct SpanishRegional
    {
        //[FieldOffset(0)]
        public uint MaxStake;
        //[FieldOffset(4)]
        public uint MaxStakeFromBank;
        //[FieldOffset(8)]
        public uint StakeMask;
        //[FieldOffset(12)]
        public uint MaxWinPerStake;
        //[FieldOffset(16)]
        public uint MaxCredit;
        //[FieldOffset(20)]
        public uint MaxReserve;
        //[FieldOffset(24)]
        public uint MaxBank;
        //[FieldOffset(28)]
        public uint MaxPlayerPoints;
        //[FieldOffset(32)]
        public uint NoteEscrow;
        //[FieldOffset(36)]
        public uint Rtp;
        //[FieldOffset(40)]
        public uint Gtime;
        //[FieldOffset(44)]
        public uint ChangeValue;
        //[FieldOffset(48)]
        public uint MaxNote;
        //[FieldOffset(52)]
        public uint BankToCredits;
        //[FieldOffset(56)]
        public uint ChargeConvertPoints;
        //[FieldOffset(60)]
        public uint CycleSize;
        //[FieldOffset(64)]
        public uint FastTransfer;
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

    static class BoLib
    {
#if DEBUG
        const string DllName = "BoLibDllD.dll";
#else
        const string dllName = "BoLibDll.dll";
#endif
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int setEnvironment();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int closeSharedMemory();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getDoorStatus();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getCountryCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getCountryCodeStr();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int refillKeyStatus();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getCurrentError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getErrorText();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getLastGame(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public unsafe static extern UInt32 getWinningGame(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getPerformanceMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getGamePerformanceMeter(uint offset, uint meterType);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getLocalMasterVolume();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public unsafe static extern UInt32 getGameModel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getGameTime(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getGameDate(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getGameCreditLevel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getGameWager(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getSwitchStatus(byte offset, byte mask);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getBank();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getLastNote(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint[] getLastNotes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHopperFloatLevel(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHopperDivertLevel(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getHopperDumpSwitchActive();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getHopperDumpSwitch();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRequestEmptyLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRequestEmptyRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getBnvType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRecyclerFloatValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getRefillCtr(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getMiddleHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getMinPayoutValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getCashIn(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getCashOut(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getNotesIn(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getNotesOut(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getRefillValue(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getVtp(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getWon(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHandPay(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getTicketsPay(int meter);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getSerialNumber();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getReconciliationMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getEDCTypeStr();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getNumberOfGames();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getBoLibVersion();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getRegionalValues(int index, ref SpanishRegional region);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getDefaultRegionValues(int index, ref SpanishRegional region);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getActiveRegionValues(int index, ref SpanishRegional region);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getErrorMessage(string str, int code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUtilsRelease();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getTPlayMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUkCountryCodeB3();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUkCountryCodeC();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getSpainCountryCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getTargetPercentage();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getRecyclerChannel();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getMaxHandPayThreshold();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getCabinetType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTerminalType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte combined();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte hopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte printer();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getLiveElement(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getDefaultElement(int region, int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getTerminalFormat();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getUtilsAdd2CreditValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getLastGameModel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getReserveCredits();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isBackOfficeAvilable();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoStateValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool canPerformHandPay();
        
        /************************************************************************/
        /*							Set methods                                 */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearBankAndCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setLocalMasterVolume(uint val);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setLampStatus(byte offset, byte mask, byte state);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHopperFloatLevel(byte hopper, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestEmptyLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestEmptyRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void addCredit(int pennies);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int clearError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setCriticalError(int code);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void transferBankToCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearShortTermMeters();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHopperDivertLevel(byte hopper, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void shellSendRecycleNote();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setPrinterType(byte type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRecyclerChannel(byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setBnvType(byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRebootRequired();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setUtilsAdd2CreditValue(uint value);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestUtilsAdd2Credit();

        /************************************************************************/
        /* General methods                                                      */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void enableNoteValidator();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void disableNoteValidator();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void printTestTicket();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getPrinterTicketState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getBnvStringType(byte bnv);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useMoneyInType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useMoneyOutType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useRefillType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useVtpMeter(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useWonMeter(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useHandPayMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useTicketsMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong useStakeInMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string GetUniquePcbID(byte type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setFileAction();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearFileAction();

        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        public unsafe static extern void setTerminalType(byte type);

        /************************************************************************/
        /* Hand Pay methods                                                     */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHandPayThreshold(uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHandPayThreshold();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool getHandPayActive();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void sendHandPayToServer(uint paidOut, uint release);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void addHandPayToEDC(uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool performHandPay();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void cancelHandPay();


        /************************************************************************/
        /* TITO methods                                                         */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool getTitoEnabledState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoHost();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoProcessInState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoTicketPresented();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setTitoState(int state);
    }
}
