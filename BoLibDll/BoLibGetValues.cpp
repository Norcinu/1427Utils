#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include <sstream>
#include <cstring>
#include <string>
#include <vector>
#include "BoLibGetValues.h"
#include "MD5.h"

#define RELEASE_NUMBER	1427

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

extern int CoinConv[COIN_CNT][CC_CNT];
extern int NoteValues[NOTE_CNT][CC_CNT];

const std::string MACHINE_INI = "D:\\machine\\machine.ini";
char global_buffer[256] = {0};
std::string country_code_buffer = "";
char item[2]= {0};
char path_buffer[64] = {0};

struct GamesInfo
{
	char *name;
	char *hash_code;
	char *path;
};

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

	template <class T>
	bool FromString(T& t, const std::string& s, std::ios_base& (*f)(std::ios_base&))
	{
		std::istringstream iss(s);
		return !(iss >> f >> t).fail();
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
unsigned int getBoLibVersion()
{
	return BOLIBRELEASE;
}

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

int getUkCountryCodeB3()
{
	return CC_UKB3;
}

int getUkCountryCodeC()
{
	return CC_UKC;
}

int getSpainCountryCode()
{
	return CC_ESP;
}

char *getCountryCodeStr()
{
	//SetFileAction();
	char buffer[32] = {0};
	GetPrivateProfileSection("CountryCode", buffer, 32, MACHINE_INI.c_str());
	//ClearFileAction();
	country_code_buffer = "Country Code: ";
	country_code_buffer += buffer[0];
	return (char *)country_code_buffer.c_str();
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
	
	strncpy_s(buffer, game_info.c_str(), 511);
	return buffer;
}

unsigned long getGameModel(int index)
{
	return LastGames[index][0];
}

unsigned int getGameTime(int index)
{
				  //       hour							minute
	int result = (LastGames[index][1] << 16) | LastGames[index][2];
	return result;
}

unsigned int getGameDate(int index)
{
				  //		day							 month
	int result = (LastGames[index][3] << 16) | LastGames[index][4];
	return result;
}

unsigned int getGameWager(int index)
{
	return LastGames[index][5];
}

unsigned long getWinningGame(int index)
{
	return LastGames[index][6];
}

unsigned int getGameCreditLevel(int index)
{
	return LastGames[index][7];
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

unsigned long getSpecificCoinIn(int meter, int denom)
{
	return 0;
//	return CoinConv[denom][GetCountry()] * GetReconciliationMeter()
}

unsigned long getCashIn(int meter)
{
	auto HelloImJohnnyCash = 0;
	if (meter == MONEY_IN_LT) 
	{
		HelloImJohnnyCash += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_LT);
		HelloImJohnnyCash += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_LT);
		HelloImJohnnyCash += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_LT);
		HelloImJohnnyCash += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_LT);
		HelloImJohnnyCash += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_LT);
		HelloImJohnnyCash += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_LT);
		HelloImJohnnyCash += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_LT);

		HelloImJohnnyCash += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_LT);
		HelloImJohnnyCash += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_LT);
		HelloImJohnnyCash += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_LT);
		HelloImJohnnyCash += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_LT);
		HelloImJohnnyCash += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_LT);
		HelloImJohnnyCash += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_LT);

		HelloImJohnnyCash += GetReconciliationMeter(TITO_IN_LT);
	}
	else
	{
		HelloImJohnnyCash += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_ST);
		HelloImJohnnyCash += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_ST);
		HelloImJohnnyCash += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_ST);
		HelloImJohnnyCash += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_ST);
		HelloImJohnnyCash += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_ST);
		HelloImJohnnyCash += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_ST);
		HelloImJohnnyCash += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_ST);

		HelloImJohnnyCash += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_ST);
		HelloImJohnnyCash += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_ST);
		HelloImJohnnyCash += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_ST);
		HelloImJohnnyCash += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_ST);
		HelloImJohnnyCash += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_ST);
		HelloImJohnnyCash += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_ST);

		HelloImJohnnyCash += GetReconciliationMeter(TITO_IN_ST);
	}
	return HelloImJohnnyCash;
}

// MONEY_OUT_LT = 1, MONEY_OUT_ST = 8
unsigned long getCashOut(int meter)
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
		return (GetReconciliationMeter(REFILL_L_LT)*COINVALUELEFT + 
			GetReconciliationMeter(REFILL_R_LT)*COINVALUERIGHT);
	else
		return (GetReconciliationMeter(REFILL_L_ST)*COINVALUELEFT + 
			GetReconciliationMeter(REFILL_R_ST)*COINVALUERIGHT);
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

char *getSerialNumber()
{
	GetPrivateProfileString("Keys", "Serial", "~", global_buffer, 256, MACHINE_INI.c_str());
	std::string pre = "Serial Number: ";
	char final[272] = {0};
	strncat_s(final, pre.c_str(), pre.length());
	strncat_s(final, global_buffer, 256);
	return final;
}

char *getEDCTypeStr()
{
	char buffer[64] = {0};
	GetPrivateProfileString("Datapack", "Protocol", "", buffer, 64, MACHINE_INI.c_str());
	auto mversion = atoi(buffer);
	
	if (mversion)
		return "Data/EDC: 1 - On.";
	else
		return "Data/EDC: 0 - Off.";
}

unsigned long getReconciliationMeter(unsigned char offset)
{
	return GetReconciliationMeter(offset);
}

void getMemoryStatus(MEMORYSTATUS *memory)
{
	MEMORYSTATUS mem;
	mem.dwTotalPageFile = sizeof(mem);
	
	GlobalMemoryStatus(&mem);
	memory->dwAvailPageFile = (mem.dwAvailPageFile	/ 1024) / 1024;
	memory->dwAvailPhys		= (mem.dwAvailPhys		/ 1024) / 1024;
	memory->dwAvailVirtual	= (mem.dwAvailVirtual	/ 1024) / 1024;
	memory->dwLength		= (mem.dwLength			/ 1024) / 1024;
	memory->dwMemoryLoad	= (mem.dwMemoryLoad		/ 1024) / 1024;
	memory->dwTotalPageFile = (mem.dwTotalPageFile	/ 1024) / 1024;
	memory->dwTotalPhys		= (mem.dwTotalPhys		/ 1024) / 1024;
	memory->dwTotalVirtual	= (mem.dwTotalVirtual	/ 1024) / 1024;
}

unsigned int getNumberOfGames()
{
	char buffer[64] = {0};
	GetPrivateProfileString("Terminal", "NumberGames", "", buffer, 64, MACHINE_INI.c_str());
	unsigned int value = 0;
	utils::FromString<unsigned int>(value, buffer, std::dec);
	return value;
}

#define ESP_REGIONS						46
#define ESP_VARIABLES		            14

struct SpanishRegional
{
	unsigned int MaxStake;
	unsigned int MaxStakeFromBank;
	unsigned int StakeInc;
	unsigned int MaxWinPerStake;
	unsigned int MaxCredit;
	unsigned int MaxReserve;
	unsigned int MaxBank;
	unsigned int NoteEscrow;
	unsigned int Rtp;
	unsigned int Gtime;
	unsigned int ChangeValue;
	unsigned int MaxNote;
	unsigned int CreditAndBank;
	unsigned int ChargeConvertPoints;
};

unsigned long EspRegionalVariableValues[ESP_REGIONS][ESP_VARIABLES] =
{
	//	Max     Max			Max								Note			Change        Cred	Charge
	//	Stake   Stake	    Win     Max		Max	    Max		Escrow		    >=	    Max   And   2Convert
	//	From    From  Stake Stake*	Cred	Reserve	Bank		RTP	  Gtime	Value   Note  Bank  Play
	//	Credits	Bank  Inc 	Value																Points
	100,	100,	20,	   400,	1000,	 40000,	 40000,	0,	7000,	12,	  0,	2000,	1,	1,
	60,	 60,	20,	   400,	1000,	 24000,	 24000,	0,	7000,	12,	  0,	2000,	1,	1,
	100,	100,	20,	   500,	1000,	     0,	 50000,	0,	7500,	15,	100,	5000,	1,	1,
	60,	 60,	20,	   400,	 500,	 24000,	 24000,	0,	7000,	12,	  0,	5000,	1,	1,
	80,	100,	20,	 50000,	 500,	 50000,	 50000,	0,	7000,	12,	  0,	5000,	1,	1,
	60,	 60,	20,	   400,	 500,	 24000,	 24000,	0,	7000,	12,	  0,	2000,	0,	1,
	100,	100,	20,	   500,	1000,	 50000,	 50000,	0,	7000,	12,	  0,	5000,	1,	1,
	60,	 60,	20,	   400,	1000,	 24000,	 24000,	1,	7000,	12,	  0,	2000,	1,	1,
	100,	100,	20,	   500,	1000,	 50000,  50000,	0,	7000,	12,	  0,	5000,	1,	1,
	50,     50,	10,    500, 1000,	 25000,	 25000,	0,	7000,	12,	  0,	5000,	1,	1,
	60,	 60,	20,	   400,	 500,	 24000,	 24000,	1,	7000,	12,	  1,	2000,	0,	1,
	100,	100,	20,	   500,	1000,	 50000,	 50000,	1,	7000,	12,	200,	2000,	1,	1,
	60,	 60,	20,	   400,	 800,	 24000,	 24000,	1,	7000,	12,	  0,	2000,	1,	1,
	100,	100,	20,	 50000,	1000,	 50000,	 50000,	0,	7000,	12,	  0,	5000,	1,	1,
	100,	100,	20,	   400,	1000,	 40000,	 40000,	0,	7000,	12,	  0,	5000,	1,	1,
	60,	 60,	20,	   400,	1000,	 24000,	 24000,	0,	7000,	12,	  0,	5000,	1,	1,
	100,	100,	20,	   500,	1000,	 50000,	 50000,	0,	7000,	12,	  0,	5000,	1,	1,
	10,	 10,	20,	 40000,	 500,	 24000,	  4000,	0,	7000,	12,	  0,	5000,	1,	1,
	60,	 60,	20,	   400,	1000,	 24000,	 24000,	1,	7500,	12,	  1,	2000,	1,	1,
	100,	100,	20,	   500,	2000,	 50000,	 50000,	0,	7000,	20,	100,	2000,	1,	1,
	100,    100,	20,	  1000,	1000,	100000,	100000,	0,	7000,	12,	  0,	2000,	1,	1,
	200,	200,	20,	  1000,	1000,	200000,	200000,	0,	7000,	12,	  0,	5000,	1,	1,
	200,	200,	20,	  1000,	1000,	     0,	200000,	0,	8000,	15,	100,	5000,	1,	1,
	100,	100,	20,	  1000,	 500,	100000,	100000,	0,	7000,	12,	  0,	5000,	1,	1,
	300,	300,	20,	  1000,	 500,	300000,	300000,	0,	8000,	12,	  0,	5000,	1,	1,
	200,	200,	20,	200000,	 500,	200000,	200000,	0,	7000,	12,	  0,	5000,	1,	1,
	300,	300,	20,	300000,	 500,	300000,	300000,	0,	8200,	20,	  0,	5000,	1,	1,
	100,	100,	20,	  1000,	 500,	100000,	100000,	0,	7000,	12,	  0,	2000,	0,	1,
	200,	200,	20,	  1000,	2000,	200000,	200000,	0,	7000,	20,	  0,	5000,	1,	1,
	600,	600,	10,	  1000,	2000,	600000,	600000,	0,	8000,	12,	  0,	5000,	1,	1,
	100,	100,	20,	  1000,	1000,	100000,	100000,	1,	7000,	12,	  0,	2000,	1,	1,
	200,	200,	20,	   500,	1000,	200000,	200000,	0,	8000,	20,	  0,	5000,	1,	1,
	100,	100,	20,	  1000,	 500,	200000,	200000,	1,	7000,	12,	  0,	2000,	0,	1,
	100,	100,	20,	  1000,	1000,	100000,	100000,	1,	7000,	12,	  1,	2000,	1,	1,
	200,	200,	20,	  1000,	1000,	200000,	200000,	1,	7000,	12,	  1,	2000,	1,	1,
	300,	300,	20,	  1000,	1000,	300000,	300000,	1,	7000,	12,	  1,	2000,	1,	1,
	100,	100,	20,	  1000,	 800,	 60000,	 60000,	1,	7000,	20,	200,	2000,	1,	1,
	600,	600,	20,	  1000,	 800,	600000,	600000,	1,	8000,	20,	200,	2000,	1,	1,
	200,	200,	20,	  1000,	1000,	200000,	200000,	1,	7000,	20,	  0,	5000,	1,	1,
	200,	200,	20,	  1000,	1000,	200000,	200000,	1,	7000,	12,	  0,	5000,	1,	1,
	200,	200,	20,	  1000,	1000,	200000,	200000,	0,	7000,	20,	  0,	5000,	1,	1,
	300,	300,	20,	  1000,	1000,	300000,	300000,	0,	7000,	20,	  0,	5000,	1,	1,
	100,	100,	20,	 60000,	1000,	 60000,	 60000,	0,	7000,	20,	  0,	5000,	1,	1,
	100,	100,	20,	  1000,	1000,	100000,	100000,	0,	7000,	20,	  0,	5000,	1,	1,
	100,	100,	20,	  1000,	1000,	100000,	100000,	0,	7500,	12,	  1,	2000,	1,	1,
	600,	600,	20,	   600,	2000,	360000,	360000,	1,	8000,	20,	100,	5000,	1,	1
};

#define ESP_MAX_STAKE_CREDITS			0
#define ESP_MAX_STAKE_BANK				1
#define ESP_STAKE_INC					2
#define ESP_MAX_WIN_X					3
#define ESP_MAX_CREDITS					4
#define ESP_MAX_RESERVE_CREDITS			5	
#define ESP_MAX_BANK					6
#define ESP_BNV_ESCROW_STATE			7		
#define ESP_RTP							8	
#define ESP_GAMETIME					9	
#define ESP_GIVE_CHANGE_THRESHOLD		10	
#define ESP_MAX_BANKNOTE_VALUE			11
#define ESP_USES_BANK_METER     		12	
#define ESP_CHARGE_2CONVERTPLAYPOINTS	13

void GetRegionalValues(int index, SpanishRegional *region)
{
	region->MaxStake			= EspRegionalVariableValues[index][ESP_MAX_STAKE_CREDITS];
	region->MaxStakeFromBank	= EspRegionalVariableValues[index][ESP_MAX_STAKE_BANK];
	region->StakeInc			= EspRegionalVariableValues[index][ESP_STAKE_INC];
	region->MaxWinPerStake		= EspRegionalVariableValues[index][ESP_MAX_WIN_X];
	region->MaxCredit			= EspRegionalVariableValues[index][ESP_MAX_CREDITS];
	region->MaxReserve			= EspRegionalVariableValues[index][ESP_MAX_RESERVE_CREDITS];
	region->MaxBank				= EspRegionalVariableValues[index][ESP_MAX_BANK];
	region->NoteEscrow			= EspRegionalVariableValues[index][ESP_BNV_ESCROW_STATE];
	region->Rtp					= EspRegionalVariableValues[index][ESP_RTP];
	region->Gtime				= EspRegionalVariableValues[index][ESP_GAMETIME];
	region->ChangeValue			= EspRegionalVariableValues[index][ESP_GIVE_CHANGE_THRESHOLD];
	region->MaxNote				= EspRegionalVariableValues[index][ESP_MAX_BANKNOTE_VALUE];
	region->CreditAndBank		= EspRegionalVariableValues[index][ESP_USES_BANK_METER];
	region->ChargeConvertPoints = EspRegionalVariableValues[index][ESP_CHARGE_2CONVERTPLAYPOINTS];
}

void getRegionalValues(int index, SpanishRegional *region)
{
	GetRegionalValues(index, region);
}

char *ErrorCodes[55] = {
	"Unknown error",
	"Comms buf critical full",
	"Comms Barcode error",
	"Comms Back office",
	"Remote RN array empty",
	"Critical MEM Corruption",
	"Compensator Data Reset",
	"Reel Position Data Reset",
	"Credit Data Reset",
	"Bank Data Reset",
	"Printer not found",
	"Printer failure",
	"Printer out of paper",
	"Game Data Reset",
	"Max Win Bank Exceeded",
	"Max Credits Exceeded",
	"Max Win Exceeded",
	"Comms Remote Credit",
	"Comms Send Time Out",
	"Comms Send Link Lost",
	"Comms Send Data Invalid",
	"Comms Fail Open Socket1",
	"Comms Fail Open Socket2",
	"Comms Winsock Wrong Vrn",
	"Comms Rng Slow Fill",
	"Comms BO BarCode Fail",
	"NV LRC Removed",
	"NV Stacker full",
	"NV Safe jam",
	"NV Unsafe jam",
	"NV Fraud attempt",
	"NV Software error",
	"NV Note Rejected",
	"Hopper opto fraud",
	"Left hopper opto fail",
	"Right hopper opto fail",
	"Hopper short payout",
	"Coin denomination wrong",
	"Data pac coms failure",
	"LeftHopper Tamper Detect",
	"RightHopper Tamper Detect",
	"Payout Interrupted",
	"Ticket Print Interrupted",
	"Remote Credit Too Large",
	"NV Command Unknown",
	"NV Parameter Count Wrong",
	"NV Parameter Out Of Range",
	"NV Cant Process Command",
	"NV Software",
	"NV SSP Fail",
	"NV Key Not Set",
	"Data Pac Running Slow",
	"NV Recycler Removed",
	"NV Recycler Emptied",
	""
};

char *getErrorMessage(char *str, int code)
{
	auto length = strlen(ErrorCodes[code]) + 1;
	strcpy_s(str, length, ErrorCodes[code]);
	return str;
}

int getUtilsRelease()
{
	return RELEASE_NUMBER;
}

unsigned long getTPlayMeter(unsigned char offset)
{
	return GetTPlayMeter(offset);
}
