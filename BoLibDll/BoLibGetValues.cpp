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
int getCredit()
{
	return GetCredits();
}

int getBank()
{
	return GetBankDeposit();
}

int addCredit(int pennies)
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

int getCountryCode()
{
	return GetCountry();
}

bool isDualBank()
{
	return GetBankAndCreditMeter() ? true : false;
}

int getError()
{
	return GetCurrentError();
}

const char* getErrorText()
{
	return GetErrorText(GetCurrentError());
}

int getMaxCredits()
{
	return (signed)GetVariableValue(MAX_CREDITS);
}

int getMaxBank()
{
	return(signed)GetVariableValue(MAX_WIN_BANK);
}

int getTargetPercentage()
{
	return GetTargetPercentage();
}

int getDoorStatus()
{
	return GetDoorStatus();
}

int refillKeyStatus()
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

const char *getLastGame(int index)
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

unsigned long getGameModel(int index)
{
	return LastGames[index][0];
}

unsigned int getGameDate(int index)
{
	int result = (LastGames[index][3] << 16) | LastGames[index][4];
	return result;
}

unsigned long getWinningGame(int index)
{
	return LastGames[index][6];
}

unsigned long getPerformanceMeter(unsigned char Offset)
{
	return GetPerformanceMeter(Offset);
}

unsigned long getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType)
{
	return GetGamePerformanceMeter(Offset, MeterType);
}

unsigned int getLocalMasterVolume()
{
	return GetLocalMasterVolume();
}