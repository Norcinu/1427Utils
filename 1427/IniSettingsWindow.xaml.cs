using System;
using System.Diagnostics;
using System.Windows;

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
			//Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "/osk.exe");
		}
        
        public IniSettingsWindow(string v)
        {     
            InitializeComponent();
            OptionValue = v;
            txtNewValue.Text = OptionValue;
         //   Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "/osk.exe");
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
			/*var processes = Process.GetProcesses();
			foreach (Process p in processes)
			{
				if (p.ProcessName == "osk")
					p.Kill();
			}*/
		}
	}
}
