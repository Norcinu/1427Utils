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
DLLEXPORT void  setHopperDivertLevel(unsigned char hopper, unsigned int value);
DLLEXPORT void	shellSendRecycleNote();
DLLEXPORT void  setTerminalType(unsigned char type);
DLLEXPORT void	setPrinterType(unsigned char type);
DLLEXPORT void	setRecyclerChannel(unsigned char value);
DLLEXPORT void	setBnvType(unsigned char value);
DLLEXPORT void	setRebootRequired();
DLLEXPORT void	setUtilsAdd2CreditValue(unsigned int value);
DLLEXPORT void	setRequestUtilsAdd2Credit();
DLLEXPORT void  setEspRegionalValue(unsigned int QueryIndex,unsigned long Value);
DLLEXPORT void	enableUtilsCoinBit();
DLLEXPORT void	disableUtilsCoinBit();

