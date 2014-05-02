#define DllExport __declspec(dllexport)


extern "C" DllExport int getMaxCredits();
extern "C" DllExport int getMaxBank();
extern "C" DllExport int getTargetPercentage();
extern "C" DllExport bool isDualBank();
extern "C" DllExport int getError();
extern "C" DllExport const char *getErrorText();
extern "C" DllExport int getDoorStatus();
extern "C" DllExport int refillKeyStatus();
extern "C" DllExport int getCredit();		 
extern "C" DllExport int getBank();		    
extern "C" DllExport int addCredit(int pennies);
extern "C" DllExport int getCountryCode();
extern "C" DllExport const char *getLastGame(int index);
extern "C" DllExport unsigned long getWinningGame(int index);
//extern "C" DllExport const unsigned char *Bo_GetWinningGame(int index);
extern "C" DllExport unsigned long getPerformanceMeter(unsigned int Offset);
extern "C" DllExport unsigned long getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
extern "C" DllExport unsigned int getLocalMasterVolume();

extern "C" DllExport unsigned long getGameModel(int index);
extern "C" DllExport unsigned int getGameDate(int index);