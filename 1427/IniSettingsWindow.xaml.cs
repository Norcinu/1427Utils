using System;
using System.Diagnostics;
using System.Windows;

namespace PDTUtils
{
    public enum ChangeType { AMEND, COMMENT, UNCOMMENT, CANCEL, NONE };
	public partial class IniSettingsWindow : Window
	{
		#region options
		/*public string OptionValue
		{
			get { return optionValue; }
			set { optionValue = value; }
		}*/
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
            txtNewValue.Text = OptionValue;
        }
        
		private void button2_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.CANCEL;
			this.Close();
		}
        
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.AMEND;
			OptionValue = txtNewValue.Text;
			this.Close();
		}
        
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}
        
        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            if (OptionField[0] == '#')
            {
                OptionField = OptionField.Substring(1);
                RetChangeType = ChangeType.UNCOMMENT;
                this.Close();
            }
            else
            {
                OptionField = OptionField.Insert(0, "#");
                RetChangeType = ChangeType.COMMENT;
                this.Close();
            }
        }
	}
}
