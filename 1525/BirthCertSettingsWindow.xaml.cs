using System;
using System.Diagnostics;
using System.Windows;

namespace PDTUtils
{
    //public enum ChangeType { Amend, Comment, Uncomment, Cancel, None };
	public partial class BirthCertSettingsWindow : Window
	{
	    #region options
        public string OptionValue { get; set; }
        public string OptionField { get; set; }
        #endregion

        public ChangeType RetChangeType { get; set; }

        public BirthCertSettingsWindow()
		{
			InitializeComponent();
		}

        public BirthCertSettingsWindow(string f, string v)
        {
            InitializeComponent();
            OptionField = f;
            OptionValue = v;
            TxtNewValue.Text = OptionValue;
            RetChangeType = ChangeType.None;
            
            if (OptionField[0] == '#')
                BtnComment.Content = "Enable";
            else
                BtnComment.Content = "Disable";
            
            this.Left = (1920 / 2) - (300 / 2);
            this.Top = (1080 / 2) - (136 / 2);
            
            var w = new Window();
            
            w.Width = 300;
            w.Height = 136;
            w.Left = (this.Left + this.Width) + 25;
            w.Top = this.Top;
            w.Show();
        }
        
        void button2_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.Cancel;
			Close();
		}
        
        void btnSave_Click(object sender, RoutedEventArgs e)
		{
            RetChangeType = ChangeType.Amend;
			OptionValue = TxtNewValue.Text;
			Close();
		}

		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
