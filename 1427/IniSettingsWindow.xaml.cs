using System;
using System.Diagnostics;
using System.Windows;

namespace PDTUtils
{
    public enum ChangeType { Amend, Comment, Uncomment, Cancel, None };
	public partial class IniSettingsWindow : Window
	{
		#region options
        public string OptionValue { get; set; }
        public string OptionField { get; set; }
		#endregion
        public ChangeType RetChangeType { get; set; }

        public IniSettingsWindow()
		{
			InitializeComponent();
		}
        
        public IniSettingsWindow(string f, string v)
        {
            InitializeComponent();
            OptionField = f;
            OptionValue = v;
            TxtNewValue.Text = OptionValue;
            RetChangeType = ChangeType.None;
        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.Cancel;
			Close();
		}
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.Amend;
			OptionValue = TxtNewValue.Text;
			Close();
		}
        
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}
        
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            if (OptionField[0] == '#')
            {
                OptionField = OptionField.Substring(1);
                RetChangeType = ChangeType.Uncomment;
                Close();
            }
            else
            {
                OptionField = OptionField.Insert(0, "#");
                RetChangeType = ChangeType.Comment;
                Close();
            }
        }
	}
}
