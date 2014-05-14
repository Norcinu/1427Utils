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
	Share2 |= 0x01;
}

