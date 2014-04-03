#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibGetValues.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

int Bo_GetCredit()
{
	return GetCredits();
}

int Bo_GetBank()
{
	return GetBankDeposit();
}

int Bo_AddCredit(int pennies)
{
	int toBank = 0;
	if(pennies > GetVariableValue(MAX_WBANK_TRANSFER))
	{
		toBank = pennies - GetVariableValue(MAX_WBANK_TRANSFER);
		pennies -= toBank;
		AddToBankDeposit(toBank);
	}

	return add_cdeposit(pennies);
}

int Bo_GetCountryCode()
{
	return GetCountry();
}

bool Bo_IsDualBank()
{
	return GetBankAndCreditMeter() ? true : false;
}

int Bo_GetError()
{
	return GetCurrentError();
}

const char* Bo_GetErrorText()
{
	return GetErrorText(GetCurrentError());
}

int Bo_GetMaxCredits()
{
	return (signed)GetVariableValue(MAX_CREDITS);
}

int Bo_GetMaxBank()
{
	return(signed)GetVariableValue(MAX_WIN_BANK);
}

int Bo_GetTargetPercentage()
{
	return GetTargetPercentage();
}

int Bo_GetDoorStatus()
{
	return GetDoorStatus();
}

int Bo_RefillKeyStatus()
{
	return GetSwitchStatus(REFILL_KEY);
}

const char *Bo_GetLastGame()
{
	static int GameCounter = 10;
	// count down and when at zero reset to 10;
	// 10 is the most recent game.
	return "";
}
//int Bo_Get
