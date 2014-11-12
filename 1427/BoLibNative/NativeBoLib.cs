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

    struct SpanishRegional
    {
        public uint MaxStake;
        public uint MaxStakeFromBank;
        public uint StakeInc;
        public uint MaxWinPerStake;
        public uint MaxCredit;
        public uint MaxReserve;
        public uint MaxBank;
        public uint NoteEscrow;
        public uint Rtp;
        public uint Gtime;
        public uint ChangeValue;
        public uint MaxNote;
        public uint CreditAndBank;
        public uint ChargeConvertPoints;
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
		public unsafe static extern uint getPerformanceMeter(byte Offset);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint getGamePerformanceMeter(uint Offset, uint MeterType);

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
		public unsafe static extern UInt32 getCoinsIn(int meter);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern UInt32 getCoinsOut(int meter);

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
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe static extern string getErrorMessage(string str, int code);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int getUtilsRelease();

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

        [DllImport(dllName, CallingConvention=CallingConvention.Cdecl)]
        public unsafe static extern ulong useStackInMeter(int meter);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public unsafe static extern string GetUniquePcbID(byte TYPE);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void setFileAction();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void clearFileAction();

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
        public unsafe static extern void performHandPay();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void cancelHandPay();
	}
}
