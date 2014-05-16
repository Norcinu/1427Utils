#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibGeneral.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

void enableNoteValidator()
{
	EnableNoteValidator();
}

void disableNoteValidator()
{
	DisableNoteValidator();
}

unsigned long getPrinterTicketState()
{
	return Share2;
}

void printTestTicket()
{
	Share2 |= 0x01; //advise shell that print needed

	int returnCode = GetCurrentError();
	do{
		Sleep(2);
		
		if(returnCode == ERR_SERIAL_PRINTER_NOT_FOUND)
			int a=0;
		else if(returnCode == ERR_PRINTER_FAILURE)
			int a=0;
		else if (returnCode == ERR_PRINTER_NO_PAPER)
			int a=0;

		returnCode = GetCurrentError();
	}while (Share2&0x01);

	//if (!GetCurrentError())
	//	ShowTestMessage(GetUtilText(TESTPRINT),1);

//	Sleep(50);
}

char *getBnvStringType(unsigned char bnv)
{
	auto type = GetBnvType();
	if (type == NO_BNV)
		return "NO_BNV";
	else if (type == AUTO_BNV)
		return "AUTO_BNV";
	else if (type == NV9_BNV)
		return "NV9";
	else if (type == MEI_BNV)
		return "MEI";
	else if (type == JCM_BNV)
		return "JCM";
	else if (type == NV11_BNV)
		return "NV11";
	else if (type >= LAST_BNV)
		return "Undefined BNV type.";

	return "";
}

