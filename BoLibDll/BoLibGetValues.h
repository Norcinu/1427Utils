#define DllExport __declspec(dllexport)

struct GamesInfo;

typedef int BOOL;

/**
*	Gets the maximum credits level for the category.
*	The call wraps GetVariableValue(MAX_CREDITS).
*/
extern "C" DllExport int getMaxCredits();
/**
*	Gets the maximum allowable bank meter level.
*	The call wraps GetVariableValue(MAX_WIN_BANK).
*	Returns the value in pennies.
*/
extern "C" DllExport int getMaxBank();
/**
*	Gets the machines RTP.
*	Calls directly the BO lib equivalent.
*	Returns the value in pennies.
*/
extern "C" DllExport int getTargetPercentage();
/**
*	Determines whether or not the machine is set for dual bank meters.
*	The call wraps GetBankAndCreditMeter();
*/
extern "C" DllExport bool isDualBank();
/**
*	Gets the current error code.
*	The call wraps GetCurrentError();
*	Returns an integer based for each code.
*/
extern "C" DllExport int getError();
/**
*	Gets the current error text based on the error code. 
*	Returns an string.
*	This call wraps GetErrorText(GetCurrentError());
*/
extern "C" DllExport const char *getErrorText();
/**
*	Checks to see the status of the door. Is door open or closed.
*	Returns 0 for door-closed, 1 for door-open.
*	Call wraps GetDoorStatus();
*/
extern "C" DllExport int getDoorStatus();
/**
*	Check for refill key status.
*	0 for off, 1 for on.
*	This call wraps GetSwitchStatus(REFILL_KEY);
*/
extern "C" DllExport int refillKeyStatus();
/**
*	Get the terminals current commited credit level.
*	Value is returned in pennies.
*	Call wraps GetCredits();
*/
extern "C" DllExport int getCredit();
/**
*	Get terminals current *uncommited* credit level.
*	Value is returned in pennies.
*	Call wraps GetBankDeposit();
*/
extern "C" DllExport int			getBank();		    
extern "C" DllExport int			addCredit(int pennies);
extern "C" DllExport int			getCountryCode();
extern "C" DllExport char			*getCountryCodeStr();
extern "C" DllExport const char		*getLastGame(int index);
extern "C" DllExport unsigned long	getWinningGame(int index);
extern "C" DllExport unsigned long	getPerformanceMeter(unsigned int Offset);
extern "C" DllExport unsigned long	getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
extern "C" DllExport unsigned int	getLocalMasterVolume();
extern "C" DllExport unsigned long	getGameModel(int index);
extern "C" DllExport unsigned int   getGameTime(int index);
extern "C" DllExport unsigned int	getGameDate(int index);
extern "C" DllExport unsigned int   getGameWager(int index);
extern "C" DllExport unsigned int   getGameCreditLevel(int index);
extern "C" DllExport unsigned int	getSwitchStatus(unsigned char offset, unsigned char mask);
extern "C" DllExport unsigned int	getLastNote(int index);
extern "C" DllExport unsigned int	*getLastNotes();
extern "C" DllExport unsigned int	getHopperFloatLevel(unsigned char hopper);
extern "C" DllExport unsigned int	getHopperDivertLevel(unsigned char hopper);
extern "C" DllExport unsigned char	getHopperDumpSwitchActive();
extern "C" DllExport unsigned char	getHopperDumpSwitch();
extern "C" DllExport unsigned int	getRequestEmptyLeftHopper();
extern "C" DllExport unsigned int	getRequestEmptyRightHopper();
extern "C" DllExport unsigned char	getBnvType();
extern "C" DllExport unsigned int	getRecyclerFloatValue();
extern "C" DllExport signed int		getRefillCtr(unsigned char hopper);
extern "C" DllExport unsigned char	getLeftHopper();
extern "C" DllExport unsigned char	getMiddleHopper();
extern "C" DllExport unsigned char	getRightHopper();
extern "C" DllExport unsigned int	getMinPayoutValue();
extern "C" DllExport unsigned long	getCoinsIn(int meter);
extern "C" DllExport unsigned long	getCoinsOut(int meter);
extern "C" DllExport unsigned long	getNotesIn(int meter);
extern "C" DllExport unsigned long	getNotesOut(int meter);
extern "C" DllExport unsigned long	getRefillValue(int meter);
extern "C" DllExport unsigned long	getVtp(int meter);
extern "C" DllExport unsigned long	getWon(int meter);
extern "C" DllExport unsigned int	getHandPay(int meter);
extern "C" DllExport unsigned long	getTicketsPay(int meter);
extern "C" DllExport char			*getSerialNumber();
extern "C" DllExport char			*getEDCTypeStr();
extern "C" DllExport unsigned long	getReconciliationMeter(unsigned char offset);
extern "C" DllExport void			getMemoryStatus(MEMORYSTATUS *memory);
extern "C" DllExport unsigned int	getNumberOfGames();

