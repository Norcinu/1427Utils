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
using System.Windows.Threading;
using System.Timers;
using System.Runtime.InteropServices;

namespace PDTUtilsControlLib
{
    /// <summary>
    /// Interaction logic for SetDateControl.xaml
    /// </summary>
    public partial class SetDateControl : UserControl
    {
        DateTime m_currentDate;
		Timer t = new Timer(1000);

		public DateTime SystemDate
		{
			get
			{
				var hour = Convert.ToInt32(txtHour.Text);
				var minute = Convert.ToInt32(txtMinute.Text);
				var second = Convert.ToInt32(txtSeconds.Text);
				return new DateTime(m_currentDate.Day, m_currentDate.Month, m_currentDate.Year,
					hour, minute, second);
			}
		}

		public static DependencyProperty DateProperty = DependencyProperty.Register("SystemDate", 
			typeof(DateTime), typeof(SetTimeControl));

		public SetDateControl()
		{
			InitializeComponent();
			t.Enabled = true;
			var timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0)
			};
			
			timer.Tick += (o, e) =>
			{
				txtSeconds.Text = DateTime.Now.Second.ToString("00");
			};

			m_currentDate = DateTime.Now;
			txtHour.Text = m_currentDate.Day.ToString("00");
			txtMinute.Text = m_currentDate.Month.ToString("00");
			txtSeconds.Text = m_currentDate.Year.ToString("0000");
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var hour = IncrementValue(txtHour);
			txtHour.Text = hour.ToString("00");
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var hour = DecrementValue(txtHour);
			txtHour.Text = hour.ToString("00");
		}

		private void btnMinuteInc_Click(object sender, RoutedEventArgs e)
		{
			var minute = IncrementValue(txtMinute, false);
			txtMinute.Text = minute.ToString("00");
		}

		private void btnMinuteDec_Click(object sender, RoutedEventArgs e)
		{
			var minute = DecrementValue(txtMinute, false);
			txtMinute.Text = minute.ToString("00");
		}

		int IncrementValue(TextBlock tb, bool hour = true)
		{
			var value = Convert.ToInt32(tb.Text);
			var maxValue = (hour == true) ? 23 : 59;

			if (value < maxValue)
				value++;
			else
				value = 0;

			return value;
		}

		int DecrementValue(TextBlock tb, bool hour = true)
		{
			var value = Convert.ToInt32(tb.Text);
			var maxValue = (hour == true) ? 23 : 59;

			if (value > 0)
				value--;
			else
				value = maxValue;

			return value;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEMTIME
		{
			public short year;
			public short month;
			public short dayOfWeek;
			public short day;
			public short hour;
			public short minute;
			public short second;
			public short milliseconds;
		}

		[DllImport("kernel32.dll")]
		private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        private void btnSaveDate_Click(object sender, RoutedEventArgs e)
        {
            SYSTEMTIME newTime = new SYSTEMTIME();
            GetSystemTime(ref newTime);
            short hour = 0;
            short.TryParse(txtHour.Text, out hour);
            short minute = 0;
            short.TryParse(txtMinute.Text, out minute);
            newTime.hour = hour;
            newTime.minute = minute;
            SetSystemTime(ref newTime);
        }
	}
}
