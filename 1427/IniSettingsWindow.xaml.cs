using System;
using System.Diagnostics;
using System.Windows;

namespace PDTUtils
{
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
		}
        
        public IniSettingsWindow(string v)
        {     
            InitializeComponent();
            OptionValue = v;
            txtNewValue.Text = OptionValue;
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
		}
	}
}
