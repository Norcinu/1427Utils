#define DllExport __declspec(dllexport)

extern "C" DllExport int setEnvironment();
extern "C" DllExport void closeSharedMemory();
extern "C" DllExport void clearBankAndCredit();
extern "C" DllExport int setCountryCode(int countryCode);
extern "C" DllExport int clearError();
extern "C" DllExport void transferBankToCredit();
extern "C" DllExport void setTargetPercentage(int Percentage);
extern "C" DllExport void setLocalMasterVolume(unsigned int val);
extern "C" DllExport void setLampStatus(unsigned char offset, unsigned char mask, unsigned char state);
