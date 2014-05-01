#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibSetValues.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

int Bo_SetEnvironment()
{
	return SetEnvironment(RELEASEx);
}

void Bo_Shutdown()
{
	CloseSharedMemory();
}

void Bo_ClearBankAndCredit()
{
	ZeroBankDeposit();
	zero_cdeposit();
}

int Bo_SetCountryCode(int countryCode)
{
	nvr_main->countryCode = countryCode;
	return GetCountry();
}

int Bo_ClearError()
{
	ClearCriticalError(GetCurrentError());
	return GetCurrentError();
}

void Bo_TransferBankToCredit()
{
	TransferFromBankToCredits();
}

void Bo_SetTargetPercentage(int Percentage)
{
	SetTargetPercentage(Percentage);
}

void BO_SetLocalMasterVolume(unsigned int val)
{
	SetLocalMasterVolume(val);
}