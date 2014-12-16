#define DllExport __declspec(dllexport)
#define DLLEXPORT extern "C" __declspec(dllexport)

struct GamesInfo;
struct SpanishRegional;

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
DLLEXPORT int			getBank();		    
DLLEXPORT int			addCredit(int pennies);
DLLEXPORT int			getCountryCode();
DLLEXPORT char			*getCountryCodeStr();
DLLEXPORT const char	*getLastGame(int index);
DLLEXPORT unsigned long	getWinningGame(int index);
DLLEXPORT unsigned long	getPerformanceMeter(unsigned char Offset);
DLLEXPORT unsigned long	getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
DLLEXPORT unsigned int	getLocalMasterVolume();
DLLEXPORT unsigned long	getGameModel(int index);
DLLEXPORT unsigned int  getGameTime(int index);
DLLEXPORT unsigned int	getGameDate(int index);
DLLEXPORT unsigned int  getGameWager(int index);
DLLEXPORT unsigned int  getGameCreditLevel(int index);
DLLEXPORT unsigned int	getSwitchStatus(unsigned char offset, unsigned char mask);
DLLEXPORT unsigned int	getLastNote(int index);
DLLEXPORT unsigned int	*getLastNotes();
DLLEXPORT unsigned int	getHopperFloatLevel(unsigned char hopper);
DLLEXPORT unsigned int	getHopperDivertLevel(unsigned char hopper);
DLLEXPORT unsigned char	getHopperDumpSwitchActive();
DLLEXPORT unsigned char	getHopperDumpSwitch();
DLLEXPORT unsigned int	getRequestEmptyLeftHopper();
DLLEXPORT unsigned int	getRequestEmptyRightHopper();
DLLEXPORT unsigned char	getBnvType();
DLLEXPORT unsigned int	getRecyclerFloatValue();
DLLEXPORT signed int	getRefillCtr(unsigned char hopper);
DLLEXPORT unsigned char getLeftHopper();
DLLEXPORT unsigned char	getMiddleHopper();
DLLEXPORT unsigned char	getRightHopper();
DLLEXPORT unsigned int	getMinPayoutValue();
DLLEXPORT unsigned long	getCashIn(int meter);
DLLEXPORT unsigned long	getCashOut(int meter);
DLLEXPORT unsigned long	getNotesIn(int meter);
DLLEXPORT unsigned long	getNotesOut(int meter);
DLLEXPORT unsigned long	getRefillValue(int meter);
DLLEXPORT unsigned long	getVtp(int meter);
DLLEXPORT unsigned long	getWon(int meter);
DLLEXPORT unsigned int	getHandPay(int meter);
DLLEXPORT unsigned long	getTicketsPay(int meter);
DLLEXPORT char			*getSerialNumber();
DLLEXPORT char			*getEDCTypeStr();
DLLEXPORT unsigned long	getReconciliationMeter(unsigned char offset);
DLLEXPORT void			getMemoryStatus(MEMORYSTATUS *memory);
DLLEXPORT unsigned int	getNumberOfGames();
DLLEXPORT unsigned int	getBoLibVersion();
DLLEXPORT void			getRegionalValues(int index, SpanishRegional *region);
DLLEXPORT void			getActiveRegionValues(int index, SpanishRegional *region);
DLLEXPORT char 			*getErrorMessage(char *str, int code);
DLLEXPORT int			getUtilsRelease();
DLLEXPORT unsigned long getTPlayMeter(unsigned char offset);
DLLEXPORT int			getUkCountryCodeB3();
DLLEXPORT int			getUkCountryCodeC();
DLLEXPORT int			getSpainCountryCode();
DLLEXPORT unsigned char getRecyclerChannel();
DLLEXPORT unsigned long getMaxHandPayThreshold();
DLLEXPORT unsigned int	getCabinetType();


