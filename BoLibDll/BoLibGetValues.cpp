#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibGetValues.h"
#include <sstream>
#include <string>

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

// Local Util functions not export to DLL
namespace utils 
{
	template <class T>
	inline std::string to_string (const T& t)
	{
		std::stringstream ss;
		ss << t;
		return ss.str();
	}

	unsigned int GetDigit(const unsigned int n, const unsigned int k) 
	{
		switch(k)
		{
		case 0:return n%10;
		case 1:return n/10%10;
		case 2:return n/100%10;
		case 3:return n/1000%10;
		case 4:return n/10000%10;
		}
		return 0;
	}

	// Combines digits extracted from above function.
	// t = 5 and z = 3, yields the result of 53.
	unsigned int CombineDigits(const unsigned int t, const unsigned int z) 
	{
		unsigned int int1 = t;
		unsigned int int2 = z;
	
		unsigned int temp = int2;
	
		do
		{
			temp /= 10;
			int1 *= 10;
		} while (temp > 0);

		return int1 + int2;
	}
}
///////////////////////////////////////

// Helper defines
#define TO_STR(str) utils::to_string(str)
#define LAST_GAME_FIELDS 0x09
#define GAME_MODEL 1
// 

// Functions for export 
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

std::string ReturnDenom(const int index, const int field, const std::string& pounds)
{
	std::string ret = pounds;
	if (LastGames[index][field] >= 100)
	{
		ret.insert(ret.size()-2, ".");
		ret.insert(0, "£"); // should just handle
	}
	else
	{
		ret.insert(0, "£0.0");
	}
	return std::move(ret);
}

const char *Bo_GetLastGame(int index)
{
	std::string fields[LAST_GAME_FIELDS] = {" : ", " : ", " : ", " : ", " : ", " : ", " : ", " : ", " : "};
	fields[0] = TO_STR(LastGames[index][0]); // This should be the .raw icon file
	fields[2] = TO_STR(LastGames[index][3]) + ":" + TO_STR(LastGames[index][4]) 
		+ " " + TO_STR(LastGames[index][1]) + "/" + TO_STR(LastGames[index][2]);
	fields[4] = ReturnDenom(index, 5, TO_STR(LastGames[index][5]));
	fields[6] = ReturnDenom(index, 6, TO_STR(LastGames[index][6]));
	fields[8] = ReturnDenom(index, 7, TO_STR(LastGames[index][7]));
	
	std::string game_info = "";
	for (int i = 0; i < LAST_GAME_FIELDS; i++)
		game_info += fields[i];
	
    int field_length = game_info.size()-1;
	char buffer[512] = {0}; // erm? it only uses 44 chars and the rest dont get set to 0?
	//strncpy_s(buffer, field_length, game_info.c_str(), field_length);
	strncpy_s(buffer, game_info.c_str(), 511);
	return buffer;
}

const char *Bo_GetWinningGame(int index)
{
	auto selected_game = LastGames[index][6];
	if (selected_game)
	{
		auto value = utils::to_string(selected_game);
		value.insert(value.size()-2, ".");
		char buffer[16] = {0};
		//strncpy_s(buffer, 8, value.c_str(), 7);
		strncpy_s(buffer, value.c_str(), 15);
		return buffer;
	}

	return "";
}


unsigned long Bo_GetPerformanceMeter(unsigned char Offset)
{
	return GetPerformanceMeter(Offset);
}

unsigned long Bo_GetGamePerformanceMeter(unsigned int Offset, unsigned int MeterType)
{
	return GetGamePerformanceMeter(Offset, MeterType);
}