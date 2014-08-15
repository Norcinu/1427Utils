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
		public bool IsRunning { get; set; }
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
        #region Properties
        public bool m_doSpecials;
		public bool DoSpecials
		{
			get { return m_doSpecials; }
			set { m_doSpecials = value; }
		}
		public bool[] m_toggled;
		public bool[] Toggled
		{
			get { return m_toggled; }
			set { m_toggled = value; }
		}
		public int m_currentButton;
		public int CurrentButton
		{
			get { return m_currentButton; }
			set { m_currentButton = value; }
		}
		public int m_numberOfButtons;
		public int NumberOfButtons
		{
			get { return m_numberOfButtons; }
			set { m_numberOfButtons = value; }
		}
		public int m_specials;
		public int Specials
		{
			get { return m_specials; }
			set { m_specials = value; }
		}
		public int m_currentSpecial;
		public int CurrentSpecial
		{
			get { return m_currentSpecial; }
			set { m_currentSpecial = value; }
		}
        #endregion

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

	public class HopperImpl : Impl
	{
		public bool m_dumpSwitchPressed = false;
		public bool DumpSwitchPressed
		{
			get { return m_dumpSwitchPressed; }
			set { m_dumpSwitchPressed = value; }
		}
		public HopperImpl() : base() { }
	}

	public class LampTestImpl
	{
		public LampTestImpl()
		{

		}
	}
}
