#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibGetValues.h"
#include <sstream>
#include <string>

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

extern int CoinConv[COIN_CNT][CC_CNT];
extern int NoteValues[NOTE_CNT][CC_CNT];

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

unsigned int getSwitchStatus(unsigned char offset, unsigned char mask)
{
	return GetSwitchStatus(offset, mask);
}

unsigned int getLastNote(int index)
{
	return nvr_main->lastFiveNotes[index];
}

unsigned int *getLastNotes()
{
	return nvr_main->lastFiveNotes;
}

unsigned int getTerminalType()
{
	return GetTerminalType();
}

unsigned int getHopperFloatLevel(unsigned char hopper)
{
	return GetHopperFloatLevel(hopper);
}

unsigned int getHopperDivertLevel(unsigned char hopper)
{
	return GetHopperDivertLevel(hopper);
}

unsigned char getHopperDumpSwitchActive()
{
	return GetHopperDumpSwitchActive();
}

unsigned char getHopperDumpSwitch()
{
	return GetSwitchStatus(HOPPER_DUMP_SW);
}

unsigned int getRequestEmptyLeftHopper()
{
	return GetRequestEmptyLeftHopper();
}

unsigned int getRequestEmptyRightHopper()
{
	return GetRequestEmptyRightHopper();
}

unsigned char getBnvType()
{
	return GetBnvType();
}

unsigned int getRecyclerFloatValue()
{
	return GetRecyclerFloatValue();
}

signed int getRefillCtr(unsigned char hopper)
{
	return GetRefillCtr(hopper);
}

unsigned char getLeftHopper()
{
	return LEFTHOPPER;
}

unsigned char getMiddleHopper()
{
	return MIDDLEHOPPER;
}

unsigned char getRightHopper()
{
	return RIGHTHOPPER;
}

unsigned int getMinPayoutValue()
{
	return GetMinPayoutValue();
}

unsigned long getCoinsIn(int meter)
{
	auto coins = 0;
	if (meter == MONEY_IN_LT) 
	{
		coins += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_LT);
		coins += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_LT);
		coins += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_LT);
		coins += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_LT);
		coins += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_LT);
		coins += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_LT);
		coins += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_LT);

		coins += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_LT);
		coins += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_LT);
		coins += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_LT);
		coins += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_LT);
		coins += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_LT);
		coins += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_LT);
	}
	else
	{
		coins += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_ST);
		coins += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_ST);
		coins += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_ST);
		coins += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_ST);
		coins += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_ST);
		coins += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_ST);
		coins += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_ST);

		coins += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_ST);
		coins += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_ST);
		coins += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_ST);
		coins += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_ST);
		coins += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_ST);
		coins += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_ST);
	}
	return coins;
}

// MONEY_OUT_LT = 1, MONEY_OUT_ST = 8
unsigned long getCoinsOut(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned long getNotesIn(int meter)
{
	auto notes = 0;
	if (meter == MONEY_IN_LT)
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_LT);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_LT);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_LT);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_LT);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_LT);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_LT);
	}
	else
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_ST);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_ST);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_ST);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_ST);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_ST);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_ST);
	}
	return notes;
}

unsigned long getNotesOut(int meter)
{
	auto notes = 0;
	if (meter == MONEY_OUT_LT)
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_OUT_LT);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_OUT_LT);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_OUT_LT);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_OUT_LT);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_OUT_LT);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_OUT_LT);
	}
	else
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_OUT_ST);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_OUT_ST);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_OUT_ST);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_OUT_ST);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_OUT_ST);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_OUT_ST);
	}
	return notes;
}

unsigned long getRefillValue(int meter)
{
	if (meter == REFILL_L_LT)
		return (GetReconciliationMeter(REFILL_L_LT)*COINVALUELEFT + GetReconciliationMeter(REFILL_R_LT)*COINVALUERIGHT);
	else
		return (GetReconciliationMeter(REFILL_L_ST)*COINVALUELEFT + GetReconciliationMeter(REFILL_R_ST)*COINVALUERIGHT);
}

unsigned long getVtp(int meter)
{
	if (meter == WAGERED_LT)
		return GetPerformanceMeter(WAGERED_LT);
	else
		return GetPerformanceMeter(WAGERED_ST);
}

unsigned long getWon(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned int getHandPay(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned long getTicketsPay(int meter)
{
	return GetReconciliationMeter(meter);
}
