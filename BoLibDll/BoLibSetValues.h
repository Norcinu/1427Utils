#define DllExport __declspec(dllexport)
#define DLLEXPORT extern "C" DllExport


DLLEXPORT int	setEnvironment();
DLLEXPORT void	closeSharedMemory();
DLLEXPORT void	clearBankAndCredit();
DLLEXPORT int	setCountryCode(int countryCode);
DLLEXPORT int	clearError();
DLLEXPORT void	transferBankToCredit();
DLLEXPORT void	setTargetPercentage(int Percentage);
DLLEXPORT void	setLocalMasterVolume(unsigned int val);
DLLEXPORT void	setLampStatus(unsigned char offset, unsigned char mask, unsigned char state);
DLLEXPORT void	setHopperFloatLevel(unsigned char hopper, unsigned int value);
DLLEXPORT void	setRequestEmptyLeftHopper();
DLLEXPORT void	setRequestEmptyRightHopper();
DLLEXPORT void  setCriticalError(int code);
DLLEXPORT void  clearShortTermMeters();