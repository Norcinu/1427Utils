using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Impls;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Interaction logic for HopperUtilsWindow.xaml
	/// </summary>
	public partial class HopperUtilsWindow : Window
	{
		DoorAndKeyStatus m_keyDoor = new DoorAndKeyStatus();
		string[] m_contentHeaders = new string[3] { "Set Hopper Floats", "Empty Hoppers", "Refill Hoppers" };
		bool[] m_clearHoopers = new bool[2] { false, false };
		System.Timers.Timer m_switchTimer = new System.Timers.Timer();
		HopperImpl m_hopperImpl = new HopperImpl();
		bool doLeft = true;
		
		private HopperUtilsWindow()
		{
			this.FontSize = 22;
			InitializeComponent();
			m_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
		}

		public HopperUtilsWindow(DoorAndKeyStatus kd)
		{
			this.FontSize = 14;
			InitializeComponent();
			m_keyDoor = kd;
			m_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
			var open = BoLib.getDoorStatus();
			int first = -1;

			for (int i = 0; i < tabHoppers.Items.Count; i++)
			{
				var t = tabHoppers.Items[i] as TabItem;
				if (i < 2)
				{
					if (open > 0)
					{
						t.IsEnabled = true;
						if (first == -1)
							first = i;
					}
					else
						t.IsEnabled = false;
				}
				else
				{
					if (open > 0)
						t.IsEnabled = false;
					else
					{
						t.IsEnabled = true;
						if (first == -1)
							first = i;
						else
							t.IsEnabled = false;
					}
				}

				tabHoppers.SelectedIndex = first;
			}

			emptyLeftHopValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getLeftHopper()).ToString("0.00");
			emptyRightHopValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getRightHopper()).ToString("0.00");
		}

		private void button_DoEvent(object sender, EventArgs e)
		{
			var button = sender as Button;
			if (button.Content.ToString() == m_contentHeaders[0])
			{
				DoSetFloats();
			}
			else if (button.Content.ToString() == m_contentHeaders[1])
				DoEmptyHoppers();
			else if (button.Content.ToString() == m_contentHeaders[2])
			{

			}
		}

		private void button_EmptyEvent(object sender, EventArgs e)
		{
			//label1.Content = "Press and Hold Dump Switch";
			//label1.Foreground = Brushes.CadetBlue;
			m_switchTimer.Enabled = true;
		}

		private void checkBox_Checked(object sender, EventArgs e)
		{
			var chkbox = sender as CheckBox;
			if (chkbox.Name == "Left")
				m_clearHoopers[0] = true;
			else if (chkbox.Name == "Right")
				m_clearHoopers[1] = true;
		}

		private void checkbox_UnChecked(object sender, EventArgs e)
		{
			var chkbox = sender as CheckBox;
			if (chkbox.Name == "Left")
				m_clearHoopers[0] = false;
			else if (chkbox.Name=="Right")
				m_clearHoopers[1] = false;
		}

		private void DoSetFloats()
		{
			stackPanel1.IsEnabled = true;
			stackPanel1.Visibility = Visibility.Visible;
			lblLeftHopperValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getLeftHopper()).ToString("0.00");
			lblRightHopperValue.Content = "£" + BoLib.getHopperFloatLevel(BoLib.getRightHopper()).ToString("0.00");
		}

		private void DoEmptyHoppers()
		{
			var leftLevel = BoLib.getHopperFloatLevel(BoLib.getLeftHopper());
			var rightLevel = BoLib.getHopperFloatLevel(BoLib.getRightHopper());

			Label left = new Label() { Content = "£1 Hopper contains £ " + leftLevel.ToString("0.00"), Foreground = Brushes.Pink };
			Label right = new Label() { Content = "10p Hopper contains £ " + rightLevel.ToString("0.00"), Foreground = Brushes.Pink };

			CheckBox chkLeft = new CheckBox() { Name = "Left", Content = "Empty the Left Hopper", Foreground = Brushes.DeepPink, FontSize = 22 };
			if (leftLevel == 0)
				chkLeft.IsEnabled = false;

			CheckBox chkRight = new CheckBox() { Name = "Right", Content = "Empty the Right Hopper", Foreground = Brushes.DeepPink, FontSize = 22 };
			if (rightLevel == 0)
				chkRight.IsEnabled = false;

			Button empty = new Button() { Content = "Empty", Width = 75 };
			empty.Click += button_DoEvent;
		
			stackPanel1.Children.Add(left);
			stackPanel1.Children.Add(right);
			stackPanel1.Children.Add(chkLeft);
			stackPanel1.Children.Add(chkRight);
			stackPanel1.Children.Add(empty);

		//	m_switchTimer.Enabled = true;
			/*
			 * Hold dump switch for > 1 second
			 */
		}

		
		private void timer_CheckHopperDumpSwitch(object sender, ElapsedEventArgs e)
		{
			if (m_hopperImpl.DumpSwitchPressed == false)
			{
				if (BoLib.getHopperDumpSwitch() > 0)
				{
					//label1.Dispatcher.Invoke((DelegateUpdate)emptyHoppers, new object[] { label1 });
					m_hopperImpl.DumpSwitchPressed = true;
					m_switchTimer.Interval = 1000;
					BoLib.setRequestEmptyLeftHopper();
				}
			}
			else
			{
				if (doLeft == true)
				{
					var result = BoLib.getRequestEmptyLeftHopper();
					if (result == 0 && BoLib.getHopperFloatLevel(BoLib.getLeftHopper()) == 0)
					{
						doLeft = false;
						BoLib.setRequestEmptyRightHopper();
					}
				}
				else
				{
					var result = BoLib.getRequestEmptyRightHopper();
					if (result == 0 && BoLib.getHopperFloatLevel(BoLib.getRightHopper()) == 0)
					{
						doLeft = false;
						m_switchTimer.Enabled = false;
						m_switchTimer.Elapsed -= timer_CheckHopperDumpSwitch;
						btnEmptyHoppers.IsEnabled = true;
					}
				}

				if (BoLib.getRequestEmptyLeftHopper() > 0)
				{
					emptyLeftHopValue.Dispatcher.Invoke((DelegateUpdate)emptyHoppers, new object[] { emptyLeftHopValue });
				}
				else if (BoLib.getRequestEmptyRightHopper() > 0)
				{
					emptyRightHopValue.Dispatcher.Invoke((DelegateUpdate)emptyHoppers, new object[] { emptyRightHopValue });
				}
			}
		}

		public delegate void DelegateUpdate(Label l);
		private void emptyHoppers(Label l)
		{
			l.Foreground = Brushes.Aqua;
			l.Content = "Hopper Value : £" + BoLib.getHopperFloatLevel(
				(doLeft == true) ? BoLib.getLeftHopper() : BoLib.getRightHopper()).ToString("0.00");
		}

		private void btnSetLeft_Click(object sender, RoutedEventArgs e)
		{
			var leftHopper = BoLib.getLeftHopper();
			BoLib.setHopperFloatLevel(leftHopper, BoLib.getHopperDivertLevel(leftHopper));
		}

		private void btnSetRight_Click(object sender, RoutedEventArgs e)
		{
			var rightHopper = BoLib.getRightHopper();
			BoLib.setHopperFloatLevel(rightHopper, BoLib.getHopperDivertLevel(rightHopper));
		}

		private void btnEmptyHoppers_Click(object sender, RoutedEventArgs e)
		{
			m_switchTimer.Elapsed += timer_CheckHopperDumpSwitch;
			m_switchTimer.Enabled = true;
			btnEmptyHoppers.IsEnabled = false;
			lblGeneralMsg.Content = "Press and hold dump switch";
		}
	}
}

