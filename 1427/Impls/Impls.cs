using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.Impls
{
	public class Impl
	{
		bool m_isRunning;
		string m_name = "";

		#region Properties
		public bool IsRunning
		{
			get { return m_isRunning; }
			set { m_isRunning = value; }
		}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}
		#endregion

		public Impl()
		{
			this.m_isRunning = false;
			this.m_name = "";
		}
	}

	public class ButtonTestImpl : Impl
	{
		public bool m_doSpecials;
		public bool[] m_toggled;
		public int m_currentButton;
		public int m_numberOfButtons;
		public int m_specials;
		public int m_currentSpecial;

		public ButtonTestImpl()
		{
			m_doSpecials = true;
			m_toggled = new bool[2] { false, false };
			m_currentButton = 0;
			m_numberOfButtons = 8;
			m_specials = 2;
			m_currentSpecial = 0;
		}
	}

	public class CoinNoteValImpl : Impl
	{
		public bool m_isCoinTest;

		public CoinNoteValImpl()
		{
			m_isCoinTest = false;
		}

		public CoinNoteValImpl(bool coinTest) 
		{
			m_isCoinTest = coinTest;
		}
	}

	public class LampTestImpl
	{
		public LampTestImpl()
		{

		}
	}
}
