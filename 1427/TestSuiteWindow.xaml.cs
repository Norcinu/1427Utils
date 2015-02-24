using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Impls;
using PDTUtils.Native;
using PDTUtils.Logic;


namespace PDTUtils
{
	/// <summary>
	/// Interaction logic for TestSuiteWindow.xaml
	/// </summary>
	public partial class TestSuiteWindow : Window
	{
		ButtonTestImpl m_btnImpl = new ButtonTestImpl();
		CoinNoteValImpl m_noteImpl = new CoinNoteValImpl();
		const int m_visualButtonCount = 6;
		int m_buttonEnabledCount = 6;
		int m_counter = 0;
		int m_currentButton = 0;
		string[] m_termButtonList = new string[8] { "LH1", "LH2", "LH3", "LH4", "LH5", "LH6", "LH7", "LH8" };
        byte[] m_specialMasks = new byte[2] { 0x10, 0x02 };
        byte[] m_buttonMasks = new byte[8] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
		List<Button> m_buttons = new List<Button>();
		string[] m_buttonContent = new string[6] { "Printer", "Buttons", "Lamps", "Dil Status", "Note Val", "Coin Mech" };
		int[] m_buttonsPressed = new int[8];
		Label[] m_labels;
		System.Timers.Timer startTimer = new System.Timers.Timer();
		Thread m_testPrintThread;

		#region DELEGATE TYPES
		public delegate void DelegateDil(Label l, string message);
		public delegate void DelegateNoteVal(Label l, int v);
		public delegate void DelegateUpdate(Label l);
		public delegate void DelegatePrintErr(Label l, string message);
		public delegate void DelegateEnableBtn(Button b);
        public delegate string DelegateReturnString(Label l);
		#endregion

        public TestSuiteWindow()
        {
            InitializeComponent();
            m_labels = new Label[8] { label3, label4, label5, label6, label7, label8, label9, label10 };
            for (int i = 0; i < 7; i++)
                m_buttonsPressed[i] = 0;
            
            startTimer.Elapsed += timer_CheckButton;
            startTimer.Elapsed += timer_CheckNoteValidator;
            btnEndTest.Click += btnEndTest_Click;
        }
        
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
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
					DoPrinterTest();
				}
				else if (button.Content.ToString() == m_buttonContent[1])
				{
					DoButtonTest();
				}
				else if (button.Content.ToString() == m_buttonContent[2])
				{
					DoLampTest();
				}
				else if (button.Content.ToString() == m_buttonContent[3])
				{
					DoDilSwitchTest();
				}
				else if (button.Content.ToString() == m_buttonContent[4])
				{
					DoNoteTest();
				}
				else if (button.Content.ToString() == m_buttonContent[5])
				{
					DoCoinTest();
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
        
		private void DoLampTest()
		{
            btnEndTest.IsEnabled = true;
            label1.Dispatcher.Invoke((DelegateDil)label_updateMessage, new object[] { label1, "Testing Button Lamps" });
            
            Thread.Sleep(1200);

            for (short i = 128; i > 0; i /= 2)
            {
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
                BoLib.setLampStatus(1, (byte)i, 1);
                Thread.Sleep(200);
            }

            for (short i = 128; i > 0; i /= 2)
                BoLib.setLampStatus(1, (byte)i, 0);

            for (int i = 0; i < m_visualButtonCount; i++)
                stpButtons.Children[i].Dispatcher.Invoke((DelegateEnableBtn)timer_buttonEnable, new object[] { stpButtons.Children[i] });
            m_buttonEnabledCount = 6;

            label4.Dispatcher.Invoke((DelegateDil)label_updateMessage, new object[] { label4, "Testing Button Lamps Finished" });
		}
        
		private void DoPrinterTest()
		{
			btnEndTest.IsEnabled = true;
			m_testPrintThread = new Thread(new ThreadStart(BoLib.printTestTicket));
			m_testPrintThread.Start();
		}
        
        private void DoDilSwitchTest()
        {
            btnEndTest.IsEnabled = true;

            int ctr = 1;
            for (int i = 1; i <= 8; i *= 2)
            {
                if (BoLib.getSwitchStatus(4, (byte)i) > 0)
                {
                    var converter = new BrushConverter();
                    m_labels[ctr].Foreground = Brushes.Green;
                    m_labels[ctr].Dispatcher.Invoke((DelegateDil)label_updateMessage,
                            new object[] { m_labels[ctr], "DIL SWITCH " + ctr.ToString() + " ON" });
                }
                else
                {
                    m_labels[ctr].Foreground = Brushes.Red;
                    m_labels[ctr].Dispatcher.Invoke((DelegateDil)label_updateMessage,
                            new object[] { m_labels[ctr], "DIL SWITCH " + ctr.ToString() + " OFF" });
                }
                ctr++;
            }
        }
        
		private void DoCoinTest()
		{
			m_noteImpl.m_isCoinTest = true;
			m_noteImpl.IsRunning = true;
			BoLib.clearBankAndCredit();
			BoLib.enableNoteValidator();
			label1.Background = Brushes.Black;
			label1.Foreground = Brushes.Aqua;
			label1.Content = "Please deposit coin into the machine.";
			startTimer.Enabled = true;
			btnEndTest.IsEnabled = true;
		}
        
		private void DoNoteTest()
		{
			m_noteImpl.IsRunning = true;
			BoLib.clearBankAndCredit();
			BoLib.enableNoteValidator();
			label1.Background = Brushes.Black;
			label1.Foreground = Brushes.Aqua;
			label1.Content = "Please insert note into Note Validator.";
			startTimer.Enabled = true;
			btnEndTest.IsEnabled = true;
		}
        
		private void DoButtonTest()
		{
		    for (int i = 0; i < 8; i++)
			    m_buttonsPressed[i] = 0;
			
			m_currentButton = 0;
			this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label1 });
            m_btnImpl.IsRunning = true;
            startTimer.Enabled = true;
		}
        
		#region DELEGATES AND EVENTS
		private void label_updateMessage(Label l, string message)
		{
			l.Content = message;
		}
        
		private void timer_UpdateLabel(Label l)
		{
            l.Background = Brushes.Green;
            l.Foreground = Brushes.White;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
			l.Content = "SUCCESS " + m_termButtonList[m_currentButton] + " OK";
		}
        
		private void timer_UpdateSpecials(Label l)
		{
            if (l == label1)
            {
                if ((string)l.Content == "" || l.Content == null)
                    l.Content = "Please toggle the REFILL KEY off and on.";
                else
                    l.Content = "REFILL KEY OK";
            }
            else if (l == label2)
            {
                if ((string)l.Content == "" || l.Content == null)
                    l.Content = "Please hold and release the DOOR SWITCH.";
                else
                {
                    if (!l.Content.ToString().Contains(" OK"))
                        l.Content = "DOOR SWITCH OK";
                }
            }
		}

        private void UpdateSpecialsError(Label l)
        {
            l.Background = Brushes.Red;
            l.Foreground = Brushes.Black;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
            l.Content = "**WARNING** Button NOT FITTED/ERROR";
        }
        
		private void timer_buttonError(Label l)
		{
			l.Background = Brushes.Red;
			l.Foreground = Brushes.Black;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
			l.Content = "**WARNING** " + m_termButtonList[m_currentButton] + " NOT FITTED/ERROR";
		}
        
		private void timer_updateNoteVal(Label l, int v)
		{
			if (v >= 500)
				l.Content = "Note of " + (v / 100).ToString("0.00") + " value inserted.";
			else
				l.Content = "Coin of " + (v / 100).ToString("0.00") + " value inserted.";
		}
        
		private void timer_buttonEnable(Button b)
		{
			b.IsEnabled = true;
		}
        //the manc 
        private string timer_getLabelContent(Label l)
        {
            return (string)l.Content;
        }
        
		private void timer_CheckButton(object sender, ElapsedEventArgs e)
		{
            //set m_btnImpl.isrunning to true then at the end set it to false.
            if (m_btnImpl.IsRunning)
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

                            var comp = this.label1.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent,
                                new object[] { label1 }) as string;

                            if (comp == "" || comp == null)
                                this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label1 });

                            var mask = m_specialMasks[0];
                            var status = BoLib.getSwitchStatus(2, mask);
                            if (status == 0)
                            {
                                if (m_btnImpl.m_toggled[0] == false) // key toggled off
                                    m_btnImpl.m_toggled[0] = true;
                            }
                            else
                            {
                                if (m_btnImpl.m_toggled[0] == true) // key toggled on
                                {
                                    m_btnImpl.m_currentSpecial = 1;
                                    m_counter = 0;
                                    this.label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label1 });
                                }
                            }
                        }
                        else if (m_btnImpl.m_currentSpecial == 1)
                        {
                            var comp = this.label2.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent,
                                new object[] { label2 }) as string;

                            if (comp == "" || comp == null)
                                this.label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label2 });

                            if (m_btnImpl.m_toggled[1] == false)
                                m_counter++;

                            var mask = m_specialMasks[1];
                            var status = BoLib.getSwitchStatus(2, mask);
                            if (status == 0)
                            {
                                if (m_btnImpl.m_toggled[1] == false) // toggle closed
                                    m_btnImpl.m_toggled[1] = true;
                            }
                            else
                            {
                                if (m_btnImpl.m_toggled[1] == true) // toggle open
                                {
                                    m_btnImpl.DoSpecials = false;
                                    this.label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, new object[] { label2 });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_btnImpl.m_currentSpecial < 1)
                        {
                            m_btnImpl.m_currentSpecial = 1;

                            var comp = this.label1.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent,
                               new object[] { label1 }) as string;

                            if (comp == "Please toggle the REFILL KEY off and on.")
                                this.label1.Dispatcher.Invoke((DelegateUpdate)UpdateSpecialsError, new object[] { label1 });
                        }
                        else
                        {
                            if (m_btnImpl.DoSpecials == true && m_btnImpl.m_currentSpecial == 1)
                                this.label2.Dispatcher.Invoke((DelegateUpdate)UpdateSpecialsError, new object[] { label2 });

                            m_btnImpl.m_currentSpecial = 0;
                            m_btnImpl.m_doSpecials = false;
                        }
                        if (m_counter >= 60)
                            m_counter = 0;
                    }
                }
                else // Button deck
                {
                    uint status = 100;
                    if ((m_counter >= 0 && m_counter < 30) && m_currentButton < 8)
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
                            btnEndTest.Dispatcher.Invoke((DelegateEnableBtn)timer_buttonEnable, new object[] { btnEndTest });
                        }
                        m_counter = 0;
                    }
                }
			}
		}
        
		private void timer_CheckNoteValidator(object sender, ElapsedEventArgs e)
		{
			if (m_noteImpl.IsRunning == true)
			{
				int value = BoLib.getCredit() + BoLib.getBank();
				if (value > 0)
				{
					label3.Dispatcher.Invoke((DelegateNoteVal)timer_updateNoteVal, 
						new object[] { label3, value });
					BoLib.clearBankAndCredit();
				}
			}
		}
        
		/// <summary>
		/// Clear the form, some tests like the coin and note need to run indefinitely 
		/// until otherwise told. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnEndTest_Click(object sender, RoutedEventArgs e)
		{
			if (m_testPrintThread != null)
				while (m_testPrintThread.IsAlive) ;
			
			for (int i = 0; i < m_visualButtonCount; i++)
				stpButtons.Children[i].IsEnabled = true;

			m_buttonEnabledCount = m_visualButtonCount;
            
			label1.Background = Brushes.Black; //null;
			label1.Foreground = Brushes.Yellow;
            
			if (m_noteImpl.IsRunning)
			{
				BoLib.clearBankAndCredit();
				BoLib.disableNoteValidator();
			}
            
			if (startTimer.Enabled == true)
				startTimer.Enabled = false;
            
			var labels = Extension.GetChildOfType<Label>(stpMainPanel);
			foreach (var l in labels)
			{
				l.Content = "";
				l.Foreground = Brushes.Yellow; // null;
				l.Background = Brushes.Black; //null
				l.FontSize = 22;
			}
		    
			btnEndTest.IsEnabled = false;
		}
        
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion
        
		private void Window_Closed(object sender, EventArgs e)
		{
            if (startTimer.Enabled)
                startTimer.Enabled = false;
            
            // shut down print thread? - should never start up.
		}
	}
}
