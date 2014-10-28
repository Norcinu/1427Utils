#include "BoLibHandPay.h"
#include "General.h"

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