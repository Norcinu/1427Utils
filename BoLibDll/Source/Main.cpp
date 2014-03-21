#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "Main.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

int SetEnvironment()
{
	return SetEnvironment(RELEASEx);
}

void Shutdown()
{
	CloseSharedMemory();
}

int GetCredit()
{
	return GetCredits();
}

int GetBank()
{
	return GetBankDeposit();
}

int AddCredit(int pennies)
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

void ClearBankAndCredit()
{
	ZeroBankDeposit();
	zero_cdeposit();
}

int GetCountryCode()
{
	return GetCountry();
}

int SetCountryCode(int countryCode)
{
	nvr_main->countryCode = countryCode;
	return GetCountry();
}

bool IsDualBank()
{
	return GetBankAndCreditMeter() ? true : false;
}

int GetError()
{
	return GetCurrentError();
}

int ClearError()
{
	ClearCriticalError(GetCurrentError());
	return GetCurrentError();
}

const char* GetErrorText()
{
	return GetErrorText(GetCurrentError());
}

void TransferBankToCredit()
{
	TransferFromBankToCredits();
}

int GetMaxCredits()
{
	return (signed)GetVariableValue(MAX_CREDITS);
}

int GetMaxBank()
{
	return(signed)GetVariableValue(MAX_WIN_BANK);
}

int GetRTP()
{
	return GetTargetPercentage();
}

void SetRTP(int Percentage)
{
	SetTargetPercentage(Percentage);
}