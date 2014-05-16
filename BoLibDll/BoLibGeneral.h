#define DllExport __declspec(dllexport)

extern "C" DllExport void enableNoteValidator();
extern "C" DllExport void disableNoteValidator();
extern "C" DllExport void printTestTicket();
extern "C" DllExport char *getBnvStringType(unsigned char bnv);
