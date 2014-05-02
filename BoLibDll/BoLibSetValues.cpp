#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibSetValues.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

int setEnvironment()
{
	return SetEnvironment(RELEASEx);
}

void closeSharedMemory()
{
	CloseSharedMemory();
}

void clearBankAndCredit()
{
	ZeroBankDeposit();
	zero_cdeposit();
}

int setCountryCode(int countryCode)
{
	nvr_main->countryCode = countryCode;
	return GetCountry();
}

int clearError()
{
	ClearCriticalError(GetCurrentError());
	return GetCurrentError();
}

void transferBankToCredit()
{
	TransferFromBankToCredits();
}

void setTargetPercentage(int Percentage)
{
	SetTargetPercentage(Percentage);
}

void setLocalMasterVolume(unsigned int val)
{
	SetLocalMasterVolume(val);
}