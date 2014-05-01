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
using System.Windows.Shapes;
using System.Diagnostics;

namespace PDTUtils
{
	/// <summary>
	/// Interaction logic for IniSettingsWindow.xaml
	/// </summary>
	public partial class IniSettingsWindow : Window
	{
		string optionValue = "";

		#region options
		public string OptionValue
		{
			get { return optionValue; }
			set { optionValue = value; }
		}
		#endregion

		public IniSettingsWindow()
		{
			InitializeComponent();
			Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "/osk.exe");
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			optionValue = txtNewValue.Text;
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var processes = Process.GetProcesses();
			foreach (Process p in processes)
			{
				if (p.ProcessName == "osk")
					p.Kill();
			}
		}
	}
}
