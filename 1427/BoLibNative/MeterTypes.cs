using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.BoLibNative
{
	public static class MeterTypes
	{
		public enum Performance
		{
			MONEY_IN_LT = 0,MONEY_OUT_LT,HAND_PAY_LT,CASHBOX_LT,NO_GAMES_LT,WON_LT,
			MONEY_IN_ST,MONEY_OUT_ST,HAND_PAY_ST,CASHBOX_ST,NO_GAMES_ST,WAGERED_ST,WON_ST
		}

		public enum MaxValues
		{
		}

		public enum ErrorCodes
		{
		}
	}
}
