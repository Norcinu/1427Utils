#define DllExport __declspec(dllexport)

extern "C" DllExport int Bo_SetEnvironment();
extern "C" DllExport void Bo_Shutdown();
extern "C" DllExport void Bo_ClearBankAndCredit();
extern "C" DllExport int Bo_SetCountryCode(int countryCode);
extern "C" DllExport int Bo_ClearError();
extern "C" DllExport void Bo_TransferBankToCredit();
extern "C" DllExport void Bo_SetTargetPercentage(int Percentage);
extern "C" DllExport void BO_SetLocalMasterVolume(unsigned int val);