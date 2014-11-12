#include "BoLibHandPay.h"
#include "General.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

void setHandPayThreshold(unsigned int value)
{
	SetHandPayThreshold(value);
}

unsigned int getHandPayThreshold()
{
	return GetHandPayThreshold();
}

bool getHandPayActive()
{
	return GetHandPayActive() ? true : false;
}

void sendHandPayToServer(unsigned int paid_out, unsigned int release)
{
	SendHandPay2Server(paid_out, release);
}

void addHandPayToEDC(unsigned int value)
{
	HandPayToEdc += value;
}

void performHandPay()
{
	if (GetTerminalType() != PRINTER)
	{
		if ((GetHandPayActive()) || (GetCountry() == CC_EURO))
		{
			auto totalCredits = GetBankDeposit() + GetCredits();
			SendHeaderOnly(HANDPAY_CONFIRM, 1);
			AddToPerformanceMeters(HAND_PAY_LT, totalCredits);
			SetMeterPulses(2, 1, totalCredits);
			addHandPayToEDC(totalCredits);
			SendHandPay2Server(totalCredits, 1427);
			
			if (GetInTournamentPlay())
			{
				AddToTPlayLog(totalCredits, TPLAY_SESSION_HAND_PAID);
				ClearTPlaySessionActive();
			}
			
			zero_cdeposit();
			ZeroBankDeposit();
		}
	}
}

void cancelHandPay()
{
	SendHeaderOnly(HANDPAY_CANCEL, 1);
}
