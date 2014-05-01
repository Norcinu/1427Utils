#define DllExport __declspec(dllexport)


extern "C" DllExport int Bo_GetMaxCredits();
extern "C" DllExport int Bo_GetMaxBank();
extern "C" DllExport int Bo_GetTargetPercentage();
extern "C" DllExport bool Bo_IsDualBank();
extern "C" DllExport int Bo_GetError();
extern "C" DllExport const char *Bo_GetErrorText();
extern "C" DllExport int Bo_GetDoorStatus();
extern "C" DllExport int Bo_RefillKeyStatus();
extern "C" DllExport int Bo_GetCredit();		 
extern "C" DllExport int Bo_GetBank();		    
extern "C" DllExport int Bo_AddCredit(int pennies);
extern "C" DllExport int Bo_GetCountryCode();
extern "C" DllExport const char *Bo_GetLastGame(int index);
extern "C" DllExport const char *Bo_GetWinningGame(int index);
extern "C" DllExport unsigned long Bo_GetPerformanceMeter(unsigned int Offset);
extern "C" DllExport unsigned long Bo_GetGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
extern "C" DllExport unsigned int BO_GetLocalMasterVolume();