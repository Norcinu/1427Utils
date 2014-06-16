using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDTUtilsControlLib
{
	/// <summary>
	/// Interaction logic for SetTimeControl.xaml
	/// </summary>
	public partial class SetTimeControl : UserControl
	{
		System.Timers.Timer m_updateTimer = new System.Timers.Timer();
		DateTime m_currentDate;
		
		public SetTimeControl()
		{
			InitializeComponent();

			m_currentDate = DateTime.Now;

			txtHour.Text = m_currentDate.Hour.ToString();
			txtMinute.Text = m_currentDate.Minute.ToString();
			txtSeconds.Text = m_currentDate.Second.ToString();
		}
	}
}
