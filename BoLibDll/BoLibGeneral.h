#define DllExport __declspec(dllexport)

extern "C" DllExport void enableNoteValidator();
extern "C" DllExport void disableNoteValidator();
extern "C" DllExport void printTestTicket();
extern "C" DllExport char *getBnvStringType(unsigned char bnv);
extern "C" DllExport unsigned long useMoneyInType(int value);
extern "C" DllExport unsigned long useMoneyOutType(int value);
extern "C" DllExport unsigned long useRefillType(int value);
extern "C" DllExport unsigned long useVtpMeter(int value);
extern "C" DllExport unsigned long useWonMeter(int value);
extern "C" DllExport unsigned long useHandPayMeter(int meter);
extern "C" DllExport unsigned long useTicketsMeter(int meter);
