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
    
    [StructLayout(LayoutKind.Explicit)]
    struct SpanishRegional
    {
        [FieldOffset(0)]
        public uint MaxStake;
        [FieldOffset(4)]
        public uint MaxStakeFromBank;
        [FieldOffset(8)]
        public uint StakeMask;
        [FieldOffset(12)]
        public uint MaxWinPerStake;
        [FieldOffset(16)]
        public uint MaxCredit;
        [FieldOffset(20)]
        public uint MaxReserve;
        [FieldOffset(24)]
        public uint MaxBank;
        [FieldOffset(28)]
        public uint MaxPlayerPoints;
        [FieldOffset(32)]
        public uint NoteEscrow;
        [FieldOffset(36)]
        public uint Rtp;
        [FieldOffset(40)]
        public uint Gtime;
        [FieldOffset(44)]
        public uint ChangeValue;
        [FieldOffset(48)]
        public uint MaxNote;
        [FieldOffset(52)]
        public uint BankToCredits;
        [FieldOffset(56)]
        public uint ChargeConvertPoints;
        [FieldOffset(60)]
        public uint CycleSize;
        [FieldOffset(64)]
        public uint FastTransfer;
    }
    
    enum Performance
    {
        MONEY_IN_LT = 0,
        MONEY_OUT_LT,
        HAND_PAY_LT,
        CASHBOX_LT,
        NO_GAMES_LT,
        WAGERED_LT,
        WON_LT,
        MONEY_IN_ST,
        MONEY_OUT_ST,
        HAND_PAY_ST,
        CASHBOX_ST,
        NO_GAMES_ST,
        WAGERED_ST,
        WON_ST
    }

    static class BoLib
    {
#if DEBUG
        const string dllName = "BoLibDllD.dll";
#else
        const string dllName = "BoLibDll.dll";
#endif
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int setEnvironment();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int closeSharedMemory();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getDoorStatus();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getCountryCode();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getCountryCodeStr();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int refillKeyStatus();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getError();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getCurrentError();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getErrorText();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getLastGame(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public unsafe static extern UInt32 getWinningGame(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getPerformanceMeter(byte Offset);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getGamePerformanceMeter(uint Offset, uint MeterType);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getLocalMasterVolume();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        public unsafe static extern UInt32 getGameModel(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getGameTime(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getGameDate(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getGameCreditLevel(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getGameWager(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getSwitchStatus(byte offset, byte mask);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getCredit();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getBank();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getLastNote(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint[] getLastNotes();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHopperFloatLevel(byte hopper);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHopperDivertLevel(byte hopper);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getHopperDumpSwitchActive();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getHopperDumpSwitch();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRequestEmptyLeftHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRequestEmptyRightHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getBnvType();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getRecyclerFloatValue();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getRefillCtr(byte hopper);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getLeftHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getMiddleHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getRightHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getMinPayoutValue();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getCashIn(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getCashOut(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getNotesIn(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getNotesOut(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getRefillValue(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getVtp(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getWon(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHandPay(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getTicketsPay(int meter);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getSerialNumber();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getReconciliationMeter(byte offset);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getEDCTypeStr();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getNumberOfGames();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getBoLibVersion();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getRegionalValues(int index, ref SpanishRegional region);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getDefaultRegionValues(int index, ref SpanishRegional region);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getActiveRegionValues(int index, ref SpanishRegional region);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getErrorMessage(string str, int code);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUtilsRelease();
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getTPlayMeter(byte offset);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUkCountryCodeB3();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUkCountryCodeC();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getSpainCountryCode();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getTargetPercentage();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getRecyclerChannel();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getMaxHandPayThreshold();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getCabinetType();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTerminalType();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte combined();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte hopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte printer();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getLiveElement(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getDefaultElement(int region, int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte getTerminalFormat();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getUtilsAdd2CreditValue();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getLastGameModel(int index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong getReserveCredits();

        /************************************************************************/
        /*							Set methods                                 */
        /************************************************************************/
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearBankAndCredit();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setLocalMasterVolume(uint val);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setLampStatus(byte offset, byte mask, byte state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHopperFloatLevel(byte hopper, uint value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestEmptyLeftHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestEmptyRightHopper();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void addCredit(int pennies);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int clearError();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setCriticalError(int code);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void transferBankToCredit();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearShortTermMeters();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHopperDivertLevel(byte hopper, uint value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void shellSendRecycleNote();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setPrinterType(byte type);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRecyclerChannel(byte value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setBnvType(byte value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRebootRequired();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setUtilsAdd2CreditValue(uint value);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setRequestUtilsAdd2Credit();

        /************************************************************************/
        /* General methods                                                      */
        /************************************************************************/
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void enableNoteValidator();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void disableNoteValidator();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void printTestTicket();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 getPrinterTicketState();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getBnvStringType(byte bnv);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useMoneyInType(int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useMoneyOutType(int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useRefillType(int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useVtpMeter(int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useWonMeter(int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useHandPayMeter(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int useTicketsMeter(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ulong useStakeInMeter(int meter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string GetUniquePcbID(byte TYPE);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setFileAction();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void clearFileAction();

        [DllImport(dllName, CallingConvention=CallingConvention.Cdecl)]
        public unsafe static extern void setTerminalType(byte type);

        /************************************************************************/
        /* Hand Pay methods                                                     */
        /************************************************************************/
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setHandPayThreshold(uint value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getHandPayThreshold();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool getHandPayActive();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void sendHandPayToServer(uint paid_out, uint release);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void addHandPayToEDC(uint value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool performHandPay();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void cancelHandPay();


        /************************************************************************/
        /* TITO methods                                                         */
        /************************************************************************/
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool getTitoEnabledState();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoHost();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoProcessInState();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint getTitoTicketPresented();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void setTitoState(int state);
    }
}
