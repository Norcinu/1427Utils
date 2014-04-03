#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibGeneral.h"
#include <string>

struct LastGameLog
{
	std::string game;
	std::string time_stamp;
	std::string stake;
	std::string win;
	std::string credit;
};

unsigned long *GetLastGames()
{
	unsigned long games[5] = {0};
	return games;
}


/*char *GetLastGames(int game_no)
{
	unsigned char last_game = LastGameNo;

	if (LastGames[last_game][5])
	{
		for (int i = 0; i < 10; i++)
		{
			if (LastGames[last_game][5])
			{

			}
		}
	}

	return 0;
}*/