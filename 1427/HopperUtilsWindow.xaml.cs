﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PDTUtils.Native;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Interaction logic for HopperUtilsWindow.xaml
	/// </summary>
	public partial class HopperUtilsWindow : Window
	{
		DoorAndKeyStatus m_keyDoor = new DoorAndKeyStatus();
		string[] ContentHeaders = new string[3] { "Set Hopper Floats", "Empty Hoppers", "Refill Hoppers" };
		bool[] m_clearHoopers = new bool[2] { false, false };
		
		private HopperUtilsWindow()
		{
			InitializeComponent();
			InitButtons();
		}

		public HopperUtilsWindow(DoorAndKeyStatus kd)
		{
			InitializeComponent();
			m_keyDoor = kd;
			InitButtons();
		}

		private void InitButtons()
		{
			for (int i = 0; i < 3; i++)
			{
				// add buttons here.
				dockPanel1.Children.Add(new Button());
				var b = dockPanel1.Children[i] as Button;
				b.Content = ContentHeaders[i];
				b.MinWidth = 100;
				b.HorizontalAlignment = HorizontalAlignment.Center;
				b.Click += button_DoEvent;
	
				if (m_keyDoor.DoorStatus == false && i < 2)
					b.IsEnabled = false;
				else if (m_keyDoor.DoorStatus == true && i == 2)
					b.IsEnabled = false;
			}
		}

		private void button_DoEvent(object sender, EventArgs e)
		{
			var button = sender as Button;
			if (button.Content.ToString() == ContentHeaders[0])
			{
				DoEmptyHoppers();
			}
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

		private void DoEmptyHoppers()
		{
			var leftLevel = BoLib.getHopperFloatLevel(BoLib.getLeftHopper());
			var rightLevel = BoLib.getHopperFloatLevel(BoLib.getRightHopper());

			Label left = new Label();
			Label right = new Label();
			left.Content = "£1 Hopper contains £ " + leftLevel.ToString("0.00");
			right.Content = "10p Hopper contains £ " + leftLevel.ToString("0.00");

			CheckBox chkLeft = new CheckBox();
			chkLeft.Name = "Left";
			chkLeft.Content = "Empty the Left Hopper";
			chkLeft.Foreground = Brushes.White;
			chkLeft.FontSize = 22;
			if (leftLevel == 0)
				chkLeft.IsEnabled = false;

			CheckBox chkRight = new CheckBox();
			chkRight.Name = "Right";
			chkRight.Foreground = Brushes.White;
			chkRight.FontSize = 22;
			chkRight.Content = "Empty the Right Hopper";
			if (rightLevel == 0)
				chkRight.IsEnabled = false;
		
			Button empty = new Button();
			empty.Content = "Empty";
			empty.Width = 75;
			
			stackPanel1.Children.Add(left);
			stackPanel1.Children.Add(right);
			stackPanel1.Children.Add(chkLeft);
			stackPanel1.Children.Add(chkRight);
			stackPanel1.Children.Add(empty);
		}
	}
}

