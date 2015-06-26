/*
#include <Windows.h>
#include <bo.h>
#include <NVR.H>*/
#include "General.h"
#include "BoLibSetValues.h"
#include "BoLibGetValues.h" // errr

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);
//extern unsigned int getNumberOfGames();

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

void setLampStatus(unsigned char offset, unsigned char mask, unsigned char state)
{
	SetLampStatus(offset, mask, state);
}

void setHopperFloatLevel(unsigned char hopper, unsigned int value)
{
	SetHopperFloatLevel(hopper, value);
}

void setRequestEmptyLeftHopper()
{
	SetRequestEmptyLeftHopper();
}

void setRequestEmptyRightHopper()
{
	SetRequestEmptyRightHopper();
}

void setCriticalError(int code)
{
	if (!GetCurrentError())
		SetCriticalError(code);
}

void clearShortTermMeters()
{
	ClearShortTermMeters();
	
	for (auto i = 0; i <= getNumberOfGames(); i++)
	{
		nvr_ptr->gamePerformanceMeters[i][GAMEWON_ST] = 0;
		nvr_ptr->gamePerformanceMeters[i][GAMEWAGERED_ST] = 0;
		nvr_ptr->gamePerformanceMeters[i][GAMEPLAY_ST] = 0;
	}
}

void setHopperDivertLevel(unsigned char hopper, unsigned int value)
{
	SetHopperDivertLevel(hopper, value);
}

void shellSendEmptyRecycler()
{
	Share2 |= UTIL_EMPTY_RECYCLER_BIT; 
}

void setTerminalType(unsigned char type)
{
	SetTerminalType(type);
}

void setPrinterType(unsigned char type)
{
	SetPrinterType(type);
}

void setRecyclerChannel(unsigned char value)
{
	SetRecyclerChannel(value);
}

void setBnvType(unsigned char value)
{
	SetBnvType(value);
}

void setRebootRequired()
{
	SetRebootRequired();
}

void setUtilsAdd2CreditValue(unsigned int value)
{
    SetUtilsAdd2CreditValue(value);
}

void setRequestUtilsAdd2Credit() 
{
    SetRequestUtilsAdd2Credit();
}

void setEspRegionalValue(unsigned int QueryIndex,unsigned long Value)
{
	SetEspRegionalValue(QueryIndex, Value);
}

void enableUtilsCoinBit()
{
	//Share2 |= 0x80;
	nvr_main->share_2 |= 0x80;
}

void disableUtilsCoinBit()
{
	//Share2 &= ~0x80;
	nvr_main->share_2 &= ~0x80;
}

void setUtilBit(int bit)
{
	nvr_main->share_2 |= bit;
}

void clearUtilBit(int bit)
{
	nvr_main->share_2 &= ~bit;
}

/*
!!!! DEBUG REINCLUSION FOR NEXT BUILD OF L29. !!!!
void setPayoutCoinValues(unsigned int WhichOne, unsigned int Value)
{
	SetPayoutCoinValues(WhichOne, Value);
}*/



void clearBankCreditReserve()
{
	//if (UTIL_REQUEST_NOTE_TEST)
	{
		//clearBankAndCredit();
		nvr_ptr->bank1 = 0;
		nvr_ptr->bank2 = 0;
		nvr_ptr->bank3 = 0;

		nvr_ptr->cd1 = 0;
		nvr_ptr->cd2 = 0;
		nvr_ptr->cd3 = 0;

		nvr_ptr->reserveCredits1 = 0;
		nvr_ptr->reserveCredits2 = 0;
		nvr_ptr->reserveCredits3 = 0;
	}
}

void oogaDeBooga()
{
	Share2 |= UTIL_RECYCLER_VALUE_BIT;
}