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
    }

    enum Hoppers
    {
        LeftHopper = 0x00,
        MiddleHopper = 0x01,
        RightHopper = 0x02,
        HopperLeftMask = 0x10
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

    enum EspRegionalExt
    {
        EspAlwaysFichas = 17,
        EspAutoTfxToStake = 18
    }
    
    static class BoLib
    {
#if DEBUG
        const string DllName = "BoLibDllD.dll";
#else
        const string dllName = "BoLibDll.dll";
#endif
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int setEnvironment();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int closeSharedMemory();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDoorStatus();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCountryCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getCountryCodeStr();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int refillKeyStatus();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getCurrentError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getErrorText();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getLastGame(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern UInt32 getWinningGame(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getPerformanceMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getGamePerformanceMeter(uint offset, uint meterType);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getLocalMasterVolume();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern UInt32 getGameModel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getGameTime(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getGameDate(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGameCreditLevel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGameWager(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getSwitchStatus(byte offset, byte mask);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getBank();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getLastNote(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint[] getLastNotes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHopperFloatLevel(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHopperDivertLevel(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getHopperDumpSwitchActive();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getHopperDumpSwitch();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRequestEmptyLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRequestEmptyRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getBnvType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getRecyclerFloatValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getRefillCtr(byte hopper);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getMiddleHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getMinPayoutValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getCashIn(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getCashOut(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getNotesIn(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getNotesOut(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getRefillValue(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getVtp(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getWon(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHandPay(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getTicketsPay(int meter);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getSerialNumber();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getReconciliationMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getEDCTypeStr();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumberOfGames();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getBoLibVersion();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getRegionalValues(int index, ref SpanishRegional region);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDefaultRegionValues(int index, ref SpanishRegional region);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getActiveRegionValues(int index, ref SpanishRegional region);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getErrorMessage(string str, int code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUtilsRelease();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getTPlayMeter(byte offset);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUkCountryCodeB3();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getUkCountryCodeC();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getSpainCountryCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTargetPercentage();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getRecyclerChannel();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getMaxHandPayThreshold();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCabinetType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTerminalType();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte combined();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte hopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte printer();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getLiveElement(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDefaultElement(int region, int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte getTerminalFormat();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getUtilsAdd2CreditValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getLastGameModel(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getReserveCredits();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isBackOfficeAvilable();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoStateValue();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool canPerformHandPay();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getWinningGameMeter(int offset, int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getHistoryLength();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string getLicense();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string getCountryCodeStrLiteral(string str, int code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getEspRegionalVariableValue(int value);

        /*[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getPayoutCoinValues(uint which);*/

        /************************************************************************/
        /*							Set methods                                 */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearBankAndCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLocalMasterVolume(uint val);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLampStatus(byte offset, byte mask, byte state);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHopperFloatLevel(byte hopper, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestEmptyLeftHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestEmptyRightHopper();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addCredit(int pennies);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clearError();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setCriticalError(int code);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void transferBankToCredit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearShortTermMeters();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHopperDivertLevel(byte hopper, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shellSendRecycleNote();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPrinterType(byte type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRecyclerChannel(byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBnvType(byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRebootRequired();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setUtilsAdd2CreditValue(uint value);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setRequestUtilsAdd2Credit();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setEspRegionalValue(uint query, ulong value);

        /*[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPayoutCoinValues(uint which, uint value);*/
        
        
        /************************************************************************/
        /* General methods                                                      */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableNoteValidator();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void disableNoteValidator();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void printTestTicket();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 getPrinterTicketState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getBnvStringType(byte bnv);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useMoneyInType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useMoneyOutType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useRefillType(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useVtpMeter(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useWonMeter(int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useHandPayMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int useTicketsMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong useStakeInMeter(int meter);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetUniquePcbID(byte type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setFileAction();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearFileAction();

        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        public static extern void setTerminalType(byte type);

        /************************************************************************/
        /* Hand Pay methods                                                     */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setHandPayThreshold(uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getHandPayThreshold();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getHandPayActive();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sendHandPayToServer(uint paidOut, uint release);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addHandPayToEDC(uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool performHandPay();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void cancelHandPay();


        /************************************************************************/
        /* TITO methods                                                         */
        /************************************************************************/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getTitoEnabledState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoHost();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoProcessInState();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getTitoTicketPresented();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setTitoState(int state);
    }
}
