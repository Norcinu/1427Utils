#define DllExport __declspec(dllexport)

extern "C" DllExport int SetEnvironment();
		  
extern "C" DllExport void Shutdown();
		   
extern "C" DllExport int GetCredit();
		    
extern "C" DllExport int GetBank();
		    
extern "C" DllExport int AddCredit(int pennies);
		    
extern "C" DllExport void ClearBankAndCredit();

extern "C" DllExport int GetCountryCode();

extern "C" DllExport int SetCountryCode(int countryCode);

extern "C" DllExport bool IsDualBank();

extern "C" DllExport int GetError();

extern "C" DllExport int ClearError();

extern "C" DllExport const char* GetErrorText();

extern "C" DllExport void TransferBankToCredit();

extern "C" DllExport int GetMaxCredits();

extern "C" DllExport int GetMaxBank();

extern "C" DllExport int GetRTP();

extern "C" DllExport void SetRTP(int Percentage);
