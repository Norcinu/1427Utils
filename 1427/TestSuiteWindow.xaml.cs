using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using PDTUtils.Native;

namespace PDTUtils
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
			//m_toggled = false;
			m_currentButton = 0;
			m_numberOfButtons = 8;
			m_specials = 2;
			m_currentSpecial = 0;
		}
	}

	public class NoteValImpl : Impl
	{
		public NoteValImpl()
		{
			
		}
	}

	public class LampTestImpl
	{
		public LampTestImpl()
		{

		}
	}

	/// <summary>
	/// Interaction logic for TestSuiteWindow.xaml
	/// </summary>
	public partial class TestSuiteWindow : Window
	{
		ButtonTestImpl m_btnImpl = new ButtonTestImpl();
		NoteValImpl m_noteImpl = new NoteValImpl();

		//LampTestImpl m_lampImpl = new LampTestImpl();
		const int m_visualButtonCount = 6;
		int m_buttonEnabledCount = 6;
		int m_counter = 0;
		int m_currentButton = 0;
		string[] m_termButtonList = new string[8] { "LH1", "LH2", "LH3", "LH4", "LH5", "LH6", "LH7", "LH8" }; // Increase to 10.
		byte[] m_specialMasks = new byte[2] { 0x10, 0x02 };
		byte[] m_buttonMasks = new byte[8] {0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};
		List<Button> m_buttons = new List<Button>();
		string[] m_buttonContent = new string[6] { "Printer", "Buttons", "Lamps", "Dil Status", "Note Val", "Coin Mech" };
		int[] m_buttonsPressed = new int[8];
		Label[] m_labels;// = new Label[8];
		System.Timers.Timer startTimer = new System.Timers.Timer();

		public TestSuiteWindow()
		{
			InitializeComponent();
			m_labels = new Label[8]{label3, label4, label5, label6, label7, label8, label9, label10};
			for (int i = 0; i < 8; i++)
				m_buttonsPressed[i] = 0;

			//startTimer.Interval = 500;//1000;
			startTimer.Elapsed += timer_CheckButton;
			startTimer.Elapsed += timer_CheckNoteValidator;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//for (int i = 0; i < 6; i++)
			//	stpButtons.Children.Add(new Button());
			for (int i = 0; i < 6; i++)
			{
				stpButtons.Children.Add(new Button());
				var b = stpButtons.Children[i] as Button;
				b.Content = m_buttonContent[i];
				b.MinWidth = 90;
				b.Margin = new Thickness(0, 0, 5, 0);
				b.Click += button_Click;
				if (i < 2) 
					DockPanel.SetDock(b, Dock.Left);
				else if (i > 3)
					DockPanel.SetDock(b, Dock.Right);
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			var button = sender as Button;
			if (m_buttonEnabledCount == 6)
			{
				m_currentButton = stpButtons.Children.IndexOf(button);
				if (button.Content.ToString() == m_buttonContent[0])
				{
				}
				else if (button.Content.ToString() == m_buttonContent[1])
				{
					DoButtonTest();
				}
				else if (button.Content.ToString() == m_buttonContent[2])
				{
				}
				else if (button.Content.ToString() == m_buttonContent[3])
				{
				}
				else if (button.Content.ToString() == m_buttonContent[4])
				{
					DoNoteTest();
				}
				else if (button.Content.ToString() == m_buttonContent[5])
				{
				}

				for (int i = 0; i < m_visualButtonCount; i++)
				{
					if (button != stpButtons.Children[i])
						stpButtons.Children[i].IsEnabled = false;
				}
				m_buttonEnabledCount = 1;
			}
			else
			{
				m_buttonEnabledCount = m_visualButtonCount;
				for (int i = 0; i < m_visualButtonCount; i++)
				{
					stpButtons.Children[i].IsEnabled = true;
				}
			}
		}

		private void DoNoteTest()
		{
			m_noteImpl.IsRunning = true;
			BoLib.clearBankAndCredit();
			BoLib.enableNoteValidator();
			label1.Content = "Please insert note in Note Validator";
			startTimer.Enabled = true;
		}

		private void DoButtonTest()
		{
			//if (stpMainPanel.Children.Count == 0)
			{
				Label l = new Label();
				l.FontSize = 22;
				l.Content = "Press Button: ";
				l.Name = "PressButton";
				
				stpMainPanel.Children.Add(l);
				for (int i = 0; i < 8; i++)
					m_buttonsPressed[i] = 0;
			}

			m_currentButton = 0;
			this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label1 });
			startTimer.Enabled = true;
		}

		private void timer_UpdateLabel(Label l)
		{
			l.Content = "m_termButton = " + m_termButtonList[m_currentButton];
		}

		private void timer_UpdateSpecials(Label l)
		{
			if (l == label1)
				l.Content = "Please toggle the REFILL KEY off and on.";
			else if (l == label2)
				l.Content = "Please hold and release the DOOR SWITCH.";
		}

		private void timer_buttonError(Label l)
		{
			l.Content = "m_termButton = " + m_termButtonList[m_currentButton] + " NOT FITTED/ERROR";
		}

		private void timer_updateNoteVal(Label l)
		{
			l.Content = "NOTE ADDED";
		}

		public delegate void DelegateUpdate(Label l);
		private void timer_CheckButton(object sender, ElapsedEventArgs e)
		{
			// test refill key and door switch.
			if (m_btnImpl.m_doSpecials == true)
			{
				if (m_counter >= 0 && m_counter < 60)
				{
					if (m_btnImpl.m_currentSpecial == 0)
					{
						if (m_btnImpl.m_toggled[0] == false)
							m_counter++;
						var mask = m_specialMasks[0];
						var status = BoLib.getSwitchStatus(2, mask);
						if (status == 0)
						{
							if (m_btnImpl.m_toggled[0] == false)
							{
								MessageBox.Show("toggled key off");
								m_btnImpl.m_toggled[0] = true;
							}
							else
							{
								MessageBox.Show("toggled key on");
								m_btnImpl.m_currentSpecial = 1;
							}
							
//							MessageBox.Show("toggled key");
						}
					}
				/*	else if (m_btnImpl.m_currentSpecial == 1)
					{
						if (m_btnImpl.m_toggled[1] == false)
							m_counter++;
						MessageBox.Show("toggled key");
						var mask = m_specialMasks[1];
						var status = BoLib.getSwitchStatus(2, mask);
						if (status == 0)
						{
							if (m_btnImpl.m_toggled[1] == false)
								m_btnImpl.m_toggled[1] = true;
							else
								m_btnImpl.m_currentSpecial++;
							
							MessageBox.Show("toggled key");
						}
					}*/
				}
				else
				{
					if (m_btnImpl.m_currentSpecial < 2) //6
					{
						m_btnImpl.m_currentSpecial = 1;
					}
					else
					{
						m_btnImpl.m_currentSpecial = 0;
						m_btnImpl.m_doSpecials = false;
					}
				}
			}
			else // Button deck
			{
				uint status = 100;
				if (m_counter >= 0 && m_counter < 30) // 6
				{
					m_counter++;

					status = BoLib.getSwitchStatus(1, m_buttonMasks[m_currentButton]);
					if (status > 0)
					{
						m_buttonsPressed[m_currentButton] = 1;
						m_labels[m_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel,
							new object[] { m_labels[m_currentButton] });
					}
				}
				else
				{
					if (m_currentButton < 8)
					{
						if ((status == 0 || status == 100) && m_buttonsPressed[m_currentButton] == 0)
						{
							m_labels[m_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_buttonError,
								new object[] { m_labels[m_currentButton] });
						}
						m_currentButton++;
					}
					else
					{
						m_currentButton = 0;
						startTimer.Enabled = false;
					}

					m_counter = 0;
				}
			}
		}

		private void timer_CheckNoteValidator(object sender, ElapsedEventArgs e)
		{
			if (m_noteImpl.IsRunning == true)
			{
				if ((BoLib.getCredit() + BoLib.getBank()) >= 500)
				{
					//label3.Content = "NOTE ADDED";
					label3.Dispatcher.Invoke((DelegateUpdate)timer_updateNoteVal, new object[] { label3 });
				}
			}
		}


		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
