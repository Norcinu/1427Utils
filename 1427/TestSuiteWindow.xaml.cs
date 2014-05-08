using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PDTUtils.Native;
using System.Timers;

namespace PDTUtils
{
	/// <summary>
	/// Interaction logic for TestSuiteWindow.xaml
	/// </summary>
	public partial class TestSuiteWindow : Window
	{
		int m_counter = 0;
		int m_currentButton = 0;
		byte[] m_buttonMasks = new byte[8] {0x80, 0x040, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};
		List<Button> m_buttons = new List<Button>();
		string[] m_buttonContent = new string[6] {"Printer", "Buttons", "Lamps", "Dil Status", "Note Val", "Coin Mech" };
		
		public TestSuiteWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 6; i++)
				stpMainButtons.Children.Add(new Button());
			for (int i = 0; i < 6; i++)
			{
				var b = stpMainButtons.Children[i] as Button;//.Content = "Button " + i.ToString();
				b.Content = m_buttonContent[i];
				b.MinWidth = 100;
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
			if (button.Content.ToString() == m_buttonContent[0])
			{

			}
			else if (button.Content.ToString() == m_buttonContent[1])
			{
				DoButtonTest();
			}
		}

		private void DoButtonTest()
		{
			System.Timers.Timer startTimer = new System.Timers.Timer();
			startTimer.Interval = 1000;
			startTimer.Elapsed += timer_CheckButton;

			for (int i = 0; i < 8; i++)
			{

			}
		/*	if (onOrOff == false)
			{
				onOrOff = true;
				for (byte j = 128; j > 0; j /= 2)
					BoLib.setLampStatus(1, j, 0x00);
			}
			else
			{
				onOrOff = false;
				for (byte j = 128; j > 0; j /= 2)
					BoLib.setLampStatus(1, j, 1);
			}*/
			//BoLib.setLampStatus(1, 32, 0x00);
		}

		private void timer_CheckButton(object sender, ElapsedEventArgs e)
		{
			if (m_counter >= 0 && m_counter < 3)
			{
				m_currentButton++;
				m_counter++;
				// check if button held.
				BoLib.getSwitchStatus(1, m_buttonMasks[m_currentButton]);
			}
			else
			{
				m_currentButton = 0;
				m_counter = 0;
			}
		}
	}
}
