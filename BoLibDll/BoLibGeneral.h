#define DllExport __declspec(dllexport)

//extern "C" DllExport char *GetPlayedGame(int game_no);
extern "C" DllExport unsigned long *GetLastGames();