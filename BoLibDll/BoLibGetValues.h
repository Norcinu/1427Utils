#define DllExport __declspec(dllexport)


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
extern "C" DllExport int getBank();		    
extern "C" DllExport int addCredit(int pennies);
extern "C" DllExport int getCountryCode();
extern "C" DllExport const char *getLastGame(int index);
extern "C" DllExport unsigned long getWinningGame(int index);
extern "C" DllExport unsigned long getPerformanceMeter(unsigned int Offset);
extern "C" DllExport unsigned long getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
extern "C" DllExport unsigned int getLocalMasterVolume();

extern "C" DllExport unsigned long getGameModel(int index);
extern "C" DllExport unsigned int getGameDate(int index);