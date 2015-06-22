using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Impls;
using PDTUtils.Logic;
using PDTUtils.Native;

using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using ElapsedEventHandler = System.Timers.ElapsedEventHandler;
using Timer = System.Timers.Timer;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for TestSuiteWindow.xaml
    /// </summary>
    partial class TestSuiteWindow : Window
    {
        bool _aTestIsRunning = false;
        const int VisualButtonCount = 6;
        int _buttonEnabledCount = 6;
        int _counter = 0;
        int _currentButton = 0;
        //List<Button> _buttons = new List<Button>();//
        Thread _testPrintThread;
        
        readonly ButtonTestImpl _btnImpl = new ButtonTestImpl();
        readonly CoinNoteValImpl _noteImpl = new CoinNoteValImpl();
        readonly string[] _termButtonList = new string[8] { "LH1", "LH2", "LH3", "LH4", "LH5", "LH6", "LH7", "LH8" };
        readonly byte[] _specialMasks = new byte[2] { 0x10, 0x02 };
        readonly byte[] _buttonMasks = new byte[8] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

        readonly string[] _buttonContent = new string[6] { "Printer", "Buttons", "Lamps", "Dil Status", "Note Val", "Coin Mech" };
        readonly int[] _buttonsPressed = new int[8];
        readonly Label[] _labels;
        readonly Timer _startTimer = new Timer() { Enabled = false, Interval = 100 };
        readonly Timer _lampTimer = new Timer() { Enabled = false, Interval = 100 };

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
            _labels = new Label[8] { Label3, Label4, Label5, Label6, Label7, Label8, Label9, Label10 };
            for (var i = 0; i < 7; i++)
                _buttonsPressed[i] = 0;
               
            _startTimer.Elapsed += timer_CheckButton;
            _startTimer.Elapsed += timer_CheckNoteValidator;
            BtnEndTest.Click    += btnEndTest_Click;
        }
        
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 6; i++)
            {
                StpButtons.Children.Add(new Button());
                var b = StpButtons.Children[i] as Button;
                //var l = new Label().Content = _buttonContent[i];
                //b.Content = _buttonContent[i];
                b.Content = _buttonContent[i];//l;
                b.MinWidth = 90;
                b.Margin = new Thickness(0, 0, 5, 0);
                
                b.Click += button_Click;
                if (i < 2)
                    DockPanel.SetDock(b, Dock.Left);
                else if (i > 3)
                    DockPanel.SetDock(b, Dock.Right);
            }
        }
        //
        void button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (_buttonEnabledCount == 6)
            {
                //ResetLabels(StpMainPanel.Children);
                _currentButton = StpButtons.Children.IndexOf(button);
                Debug.Assert(button != null, "button != null");
                if (button.Content.ToString() == _buttonContent[0])
                {
                    DoPrinterTest();
                }
                else if (button.Content.ToString() == _buttonContent[1])
                {
                    DoButtonTest();
                }
                else if (button.Content.ToString() == _buttonContent[2])
                {
                    DoLampTest();
                }
                else if (button.Content.ToString() == _buttonContent[3])
                {
                    DoDilSwitchTest();
                }
                else if (button.Content.ToString() == _buttonContent[4])
                {
                    DoNoteTest();
                }
                else if (button.Content.ToString() == _buttonContent[5])
                {
                    DoCoinTest();
                }
                
                for (var i = 0; i < VisualButtonCount; i++)
                {
                    if (button != StpButtons.Children[i])
                        StpButtons.Children[i].IsEnabled = false;
                }

                _buttonEnabledCount = 1;
            }
            else
            {
                _buttonEnabledCount = VisualButtonCount;
                for (var i = 0; i < VisualButtonCount; i++)
                {
                    StpButtons.Children[i].IsEnabled = true;
                }
            }
        }

        void TheActualLampTest()
        {
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

            Label1.Dispatcher.BeginInvoke((DelegateDil)label_updateMessage, new object[] { Label1, 
                "Testing Button Lamps Finished" });
        }

        void DoLampTest()
        {
            BtnEndTest.IsEnabled = true;
            Label1.Dispatcher.Invoke((DelegateDil)label_updateMessage, new object[] { Label1, 
                "Testing Button Lamps. \nCheck Flashing Lamps." });

            //Thread.Sleep(1200);
            
            Thread t = new Thread(TheActualLampTest);
            t.Start();

            for (short i = 128; i > 0; i /= 2)
                BoLib.setLampStatus(1, (byte)i, 0);

            for (var i = 0; i < VisualButtonCount; i++)
                StpButtons.Children[i].Dispatcher.BeginInvoke((DelegateEnableBtn)timer_buttonEnable, 
                    new object[] { StpButtons.Children[i] });
            
            _buttonEnabledCount = 6;
        }
        
        void DoPrinterTest()
        {
            _aTestIsRunning = true;
            BtnEndTest.IsEnabled = true;
            _testPrintThread = new Thread(new ThreadStart(BoLib.printTestTicket));
            _testPrintThread.Start();
            _aTestIsRunning = false;
        }
        
        void DoDilSwitchTest()
        {
            BtnEndTest.IsEnabled = true;
            _aTestIsRunning = true;
            var ctr = 1;
            
            for (var i = 1; i <= 8; i *= 2)
            {
                if (BoLib.getSwitchStatus(4, (byte)i) > 0)
                {
                    var converter = new BrushConverter();
                    _labels[ctr].Background = Brushes.SlateGray;
                    _labels[ctr].Foreground = Brushes.Green;
                    _labels[ctr].Dispatcher.Invoke((DelegateDil)label_updateMessage,
                            new object[] { _labels[ctr], "DIL SWITCH " + ctr.ToString() + " ON" });
                }
                else
                {
                    _labels[ctr].Background = Brushes.SlateGray;
                    _labels[ctr].Foreground = Brushes.Red;
                    _labels[ctr].Dispatcher.Invoke((DelegateDil)label_updateMessage,
                            new object[] { _labels[ctr], "DIL SWITCH " + ctr.ToString() + " OFF" });
                }
                ctr++;
            }
            
            //BogStandardResetTests();

            Thread.Sleep(5000);

            var timer = new System.Threading.Timer(o =>
            {
                Debug.WriteLine("WHY DIS NOT RUNNING?");

                foreach (var l in _labels)
                {
                    l.Dispatcher.BeginInvoke((DelegateUpdate)label_DefaultStyle, l);
                }
            }, null, 1000, System.Threading.Timeout.Infinite);
            
            BtnEndTest.IsEnabled = false;
            _aTestIsRunning = false;
        }
        
        void DoCoinTest()
        {
            _aTestIsRunning = true;
            _noteImpl.IsCoinTest = true;
            _noteImpl.IsRunning = true;
            BoLib.clearBankAndCredit();
            //BoLib.enableNoteValidator();
            BoLib.clearUtilBit((int)UtilBits.NoteTest);
            BoLib.setUtilBit((int)UtilBits.CoinTest);
            Label1.Background = Brushes.Black;
            Label1.Foreground = Brushes.Aqua;
            Label1.Content = "Please deposit coin into the machine.";
            _startTimer.Enabled = true;
            BtnEndTest.IsEnabled = true;
            //_aTestIsRunning = false;
        }
        
        void DoNoteTest()
        {
            _aTestIsRunning = true;
            _noteImpl.IsRunning = true;
            BoLib.clearBankAndCredit();
            //BoLib.enableNoteValidator();
            //BoLib.setUtilBit((int)UtilBits.CoinTest);
            BoLib.clearUtilBit((int)UtilBits.CoinTest);
            BoLib.setUtilBit((int)UtilBits.NoteTest);
            Label1.Background = Brushes.Black;
            Label1.Foreground = Brushes.Aqua;
            Label1.Content = "Please insert note into Note Validator.";
            _startTimer.Enabled = true;
            BtnEndTest.IsEnabled = true;
            //_aTestIsRunning = false;
        }
        
        void DoButtonTest()
        {
            for (var i = 0; i < 8; i++)
                _buttonsPressed[i] = 0;

            _currentButton = 0;
            Label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, Label1);
            _btnImpl.IsRunning = true;
            _startTimer.Enabled = true;
            _aTestIsRunning = true;
        }

        void ResetLabels(UIElementCollection col)
        {
            if (col == null) return;

            UIElementCollection mainPanel = col;
            for (int i = 0; i < col.Count; i++)
            {
                if (col[i] is Label)
                {
                    var label = col[i] as Label;
                    label.Dispatcher.Invoke((DelegateUpdate)label_DefaultStyle, label);
                }
            }
        }
        
        #region DELEGATES AND EVENTS
        void label_DefaultStyle(Label l)
        {
            l.Foreground = Brushes.Yellow;
            l.Background = Brushes.Black;
            l.BorderBrush = Brushes.Black;
            l.Content = "";
        }
        
        void label_updateMessage(Label l, string message)
        {
            l.Foreground = Brushes.Snow;
            l.Background = Brushes.SlateGray;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
            l.Content = message;
        }
        
        void timer_UpdateLabel(Label l)
        {
            l.Background = Brushes.Green;
            l.Foreground = Brushes.White;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
            l.Content = "SUCCESS " + _termButtonList[_currentButton] + " OK";
        }
        
        void timer_UpdateSpecials(Label l)
        {
            if (l == Label1)
            {
                l.Content = string.IsNullOrEmpty((string)l.Content)
                    ? "Please toggle the REFILL KEY off and on."
                    : "REFILL KEY OK";
            }
            else if (l == Label2)
            {
                if (string.IsNullOrEmpty((string)l.Content))
                    l.Content = "Please hold and release the DOOR SWITCH.";
                else
                {
                    if (!l.Content.ToString().Contains(" OK"))
                        l.Content = "DOOR SWITCH OK";
                }
            }
        }
        
        static void UpdateSpecialsError(Label l)
        {
            l.Background = Brushes.Red;
            l.Foreground = Brushes.Black;
            l.BorderBrush = Brushes.Red;
            l.BorderThickness = new Thickness(2);
            l.Content = "**WARNING** Button NOT FITTED/ERROR";
        }

        void timer_buttonError(Label l)
        {
            l.Background = Brushes.Red;
            l.Foreground = Brushes.Black;
            l.BorderBrush = Brushes.Red;
            l.BorderThickness = new Thickness(2);
            l.Content = "**WARNING** " + _termButtonList[_currentButton] + " NOT FITTED/ERROR";
        }

        void timer_ButtonShowTestMsg(Label l)
        {
            l.Background = Brushes.SlateGray;
            l.Foreground = Brushes.Yellow;
            l.BorderBrush = Brushes.Black;
            l.BorderThickness = new Thickness(2);
            l.Content = "Press And Hold the button " + _termButtonList[_currentButton];
        }

        void timer_updateNoteVal(Label l, int v)
        {
            if (v >= 500)
                l.Content = "Note of " + (v / 100).ToString("0.00") + " value inserted.";
            else
                l.Content = "Coin of " + ((v >= 100) ? (v / 100) : v).ToString(v >= 100 ? "0.00" : "") + " value inserted.";
             
            Debug.WriteLine("coin value", v.ToString());
        }
        
        void timer_buttonEnable(Button b)
        {
            b.IsEnabled = true;
        }

        string timer_getLabelContent(Label l)
        {
            return (string)l.Content;
        }
        
        void timer_CheckButton(object sender, ElapsedEventArgs e)
        {
            //set _btnImpl.isrunning to true then at the end set it to false.
            if (_btnImpl.IsRunning)
            {
                // test refill key and door switch.
                if (_btnImpl._doSpecials)
                {
                    if (_counter >= 0 && _counter < 60)
                    {
                        if (_btnImpl._currentSpecial == 0)
                        {
                            if (_btnImpl._toggled[0] == false)
                                _counter++;
                            
                            var comp = Label1.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent, Label1) as string;

                            if (string.IsNullOrEmpty(comp))
                                Label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, Label1);
                            
                            var mask = _specialMasks[0];
                            var status = BoLib.getSwitchStatus(2, mask);
                            if (status == 0)
                            {
                                if (_btnImpl._toggled[0] == false) // key toggled off
                                    _btnImpl._toggled[0] = true;
                            }
                            else
                            {
                                if (_btnImpl._toggled[0] != true) return;
                                _btnImpl._currentSpecial = 1;
                                _counter = 0;
                                Label1.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, Label1);
                            }
                        }
                        else if (_btnImpl._currentSpecial == 1)
                        {
                            var comp = Label2.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent, Label2) as string;

                            if (string.IsNullOrEmpty(comp))
                                Label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, Label2);

                            if (_btnImpl._toggled[1] == false)
                                _counter++;

                            var mask = _specialMasks[1];
                            var status = BoLib.getSwitchStatus(2, mask);
                            if (status == 0)
                            {
                                if (_btnImpl._toggled[1] == false) // toggle closed
                                    _btnImpl._toggled[1] = true;
                            }
                            else
                            {
                                if (_btnImpl._toggled[1] != true) return;
                                _btnImpl.DoSpecials = false;
                                _btnImpl.CurrentSpecial = 2; //!!!!
                                Label2.Dispatcher.Invoke((DelegateUpdate)timer_UpdateSpecials, Label2);
                            }
                        }
                    }
                    else
                    {
                        if (_btnImpl._currentSpecial < 1)
                        {
                            _btnImpl._currentSpecial = 1;

                            var comp = Label1.Dispatcher.Invoke((DelegateReturnString)timer_getLabelContent, Label1) as string;

                            if (comp == "Please toggle the REFILL KEY off and on.")
                                Label1.Dispatcher.Invoke((DelegateUpdate)UpdateSpecialsError, Label1);
                        }
                        else
                        {
                            if (_btnImpl.DoSpecials && _btnImpl._currentSpecial == 1)
                                Label2.Dispatcher.Invoke((DelegateUpdate)UpdateSpecialsError, Label2);
                            
                            _btnImpl._currentSpecial = 0;
                            _btnImpl._doSpecials = false;
                            _counter = 0;
                        }
                        if (_counter >= 60)
                            _counter = 0;
                    }
                }
                else // Button deck
                {
                    uint status = 100;
                    if ((_counter >= 0 && _counter < 30) && _currentButton < 8)
                    {
                        _counter++;

                        status = BoLib.getSwitchStatus(1, _buttonMasks[_currentButton]);
                        if (_currentButton == 0 && status == 0)
                            _labels[_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_ButtonShowTestMsg, 
                                _labels[_currentButton]);
                        
                        if (status > 0)
                        {
                            _buttonsPressed[_currentButton] = 1;
                            _labels[_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_UpdateLabel, _labels[_currentButton]);
                        }
                    }
                    else
                    {
                        if (_currentButton < 8)
                        {
                            if ((status == 0 || status == 100) && _buttonsPressed[_currentButton] == 0)
                            {
                                _labels[_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_buttonError, 
                                                                          _labels[_currentButton]);
                            }
                            _currentButton++;

                            if (_currentButton < 8)
                                _labels[_currentButton].Dispatcher.Invoke((DelegateUpdate)timer_ButtonShowTestMsg,
                                                                          _labels[_currentButton]);
                        }
                        else
                        {
                            _currentButton = 0;
                            _aTestIsRunning = false;
                            _startTimer.Enabled = false;
                            BtnEndTest.Dispatcher.Invoke((DelegateEnableBtn)timer_buttonEnable, BtnEndTest);
                        }
                        _counter = 0;
                    }
                }
            }
        }
        
        void timer_CheckNoteValidator(object sender, ElapsedEventArgs e)
        {
            if (!_noteImpl.IsRunning) return;
            var value = BoLib.getCredit() + BoLib.getBank() + BoLib.getReserveCredits;
            if (value <= 0) return;
            Label3.Dispatcher.Invoke((DelegateNoteVal)timer_updateNoteVal, Label3, value);
            BoLib.clearBankAndCredit();
        }

        void BogStandardResetTests()
        {
            if (_aTestIsRunning)
            {
                if (_testPrintThread != null)
                    while (_testPrintThread.IsAlive) ;

                foreach (Button b in StpButtons.Children)
                {
                    if (!b.IsEnabled)
                        b.IsEnabled = true;
                }

                ResetLabels(StpMainPanel.Children);

                BtnEndTest.IsEnabled = false;
                _aTestIsRunning = false;
            }
        }
        
        /// <summary>
        /// Clear the form, some tests like the coin and note need to run indefinitely 
        /// until otherwise told. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEndTest_Click(object sender, RoutedEventArgs e)
        {
            if (_aTestIsRunning)
            {
                if (_testPrintThread != null)
                    while (_testPrintThread.IsAlive) ;

                foreach (Button b in StpButtons.Children)
                {
                    if (!b.IsEnabled)
                        b.IsEnabled = true;
                }
                
                ResetLabels(StpMainPanel.Children);

                BoLib.clearUtilBit((int)UtilBits.CoinTest);
                BoLib.clearUtilBit((int)UtilBits.NoteTest);

                BtnEndTest.IsEnabled = false;
                _aTestIsRunning = false;
            }
        }
        
        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
        
        void Window_Closed(object sender, EventArgs e)
        {
            if (_startTimer.Enabled)
                _startTimer.Enabled = false;

            if (_lampTimer.Enabled)
                _lampTimer.Enabled = false;

            BoLib.clearUtilBit((int)UtilBits.CoinTest);
            BoLib.clearUtilBit((int)UtilBits.NoteTest);

            //BoLib.disableNoteValidator();

            // shut down print thread? - should never start up.
        }
    }
}
    