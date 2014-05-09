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
		const int m_visualButtonCount = 6;
		int m_buttonEnabledCount = 6;
		int m_currentTest = 0;
		int m_counter = 0;
		int m_currentButton = 0;
		string[] m_termButtonList = new string[8] { "LH1", "LH2", "LH3", "LH4", "LH5", "LH6", "LH7", "LH8" }; // Increase to 10.
		byte[] m_buttonMasks = new byte[8] {0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};
		List<Button> m_buttons = new List<Button>();
		string[] m_buttonContent = new string[6] {"Printer", "Buttons", "Lamps", "Dil Status", "Note Val", "Coin Mech" };
		int m_buttonStatus = 0;
		bool m_updateLabelOne = false;
		bool m_updateLabelTwo = false;
		Label[] m_labels;// = new Label[8]
		System.Timers.Timer startTimer = new System.Timers.Timer();

		public TestSuiteWindow()
		{
			InitializeComponent();
			m_labels = new Label[8]{label1, label2, label3, label4, label5, label6, label7, label8};
			//startTimer.Interval = 500;//1000;
			startTimer.Elapsed += timer_CheckButton;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 6; i++)
				stpButtons.Children.Add(new Button());
			for (int i = 0; i < 6; i++)
			{
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

		private void DoButtonTest()
		{
			if (stpMainPanel.Children.Count == 0)
			{
				Label l = new Label();
				l.FontSize = 22;
				l.Content = "Press Button: ";
				l.Name = "PressButton";
				stpMainPanel.Children.Add(l);
			}

			m_currentButton = 0;
			this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label1 });
			//this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel);
			//this.label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel2);

			startTimer.Enabled = true;
		}

		private void timer_UpdateLabel(Label l)
		{
			//label1.Content = "m_currentButton = " + m_currentButton.ToString();
			l.Content = "m_termButton = " + m_termButtonList[m_currentButton];
		}

		private void timer_UpdateLabel2()
		{
			label2.Content = "**m_currentButton = " + m_currentButton.ToString() + "**";
		}

		private void timer_buttonError(Label l)
		{
			l.Content = "m_termButton = " + m_termButtonList[m_currentButton] + " NOT FITTED/ERROR";
		}

		public delegate void DelegateUpdate(Label l);
		private void timer_CheckButton(object sender, ElapsedEventArgs e)
		{
			// must handle refill key :-( && door switch.
			uint status = 100;
			if (m_counter >= 0 && m_counter < 30) // 6
			{
				m_counter++;

				status = BoLib.getSwitchStatus(1, m_buttonMasks[m_currentButton]);
				if (status > 0)
				{
					if (m_currentButton == 0)
						this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label1 });
					else if (m_currentButton == 1)
						this.label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label2 });
					else if (m_currentButton == 2)
						this.label3.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label3 });
					else if (m_currentButton == 3)
						this.label4.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label4 });
					else if (m_currentButton == 4)
						this.label5.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label5 });
					else if (m_currentButton == 5)
						this.label6.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label6 });
					else if (m_currentButton == 6)
						this.label7.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label7 });
					else if (m_currentButton == 7)
					{
						this.label8.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label8 });
						startTimer.Enabled = false;
					}
				}
			}
			else
			{
				if (m_currentButton < 8) //6
				{
					if (status == 0 || status == 100)
					{
						m_labels[m_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_buttonError, new object[] { m_labels[m_currentButton] });
						//this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, new object[] { label5 });
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

		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
