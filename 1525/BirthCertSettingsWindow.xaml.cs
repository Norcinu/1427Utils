using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PDTUtils
{
    public class HelpMessageWindow : Window
    {
        public string Message { get; set; }
        
        public HelpMessageWindow()
        {
        }

        public HelpMessageWindow(string message)
        {
            Background = System.Windows.Media.Brushes.HotPink;
            SizeToContent = SizeToContent.Width;
            Message = message;
            this.AddChild(new Label()
            {
                Content = Message,
                Margin = new Thickness(15),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 24,
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.Yellow
            });
        }
    }
    
    //public enum ChangeType { Amend, Comment, Uncomment, Cancel, None };
	public partial class BirthCertSettingsWindow : Window
	{
        string[] _theHelpMessages = new string[27]
        {
            @"Payout Type: 0 = Hopper. 1 = Printer. 2 = Combined.", 
            @"Number Of Hoppers: 0 = No Hopper. 1 = High Value Coin Only. 2 = High & Low Hopper",
            @"Hopper Type 1 (Large): 0 No Hopper. 1 = Universal. 2 = Snow. 3 = Compact", 
            @"Hopper Type 2 (Small): 0 No Hopper. 1 = Universal. 2 = Snow. 3 = Compact",
            @"Payout Coin 1(Large Hop): 0 = No coins. XXX = Value in Cents unit.", 
            @"Payout Coin 2(Small Hop): 0 = No coins. XXX = Value in Cents unit.",
            @"Printer Type: 0 = No Printer. 1 = TL60. 2 = TPT52. 3 = NV200_NT. 4 = Onyx\Gen2", 
            @"Printer Baud Rate: 115200 or 9600",
            @"Cpu Type: 0 = S410. 1 = S430", 
            @"Cabinet Type: 0 = V19. 1 = Slant Top. 2 = TS22. 3 = TS22_2015. 4 = BS100_2014",
            @"BNV Type: 0 = None. 1 = Auto Detect. 2 = NV9. 3 = MEI. 4 = JCM. 5 = NV11. 6 = NV200",
            @"Coin Validator: 0 = Eagle. 1 = Calibri",
            @"Note Validator Float Control: 0 = NV Never Off. XYZ = NV Off Float when < XYZ",
            @"Recycler Channel: 2 = 10 Euro. 3 = 20 Euro.",
            @"Card Reader: 0 = No Card Reader. 1 = Card Reader Active.",
            @"Screen Count: 1 = 1 Screen. 2 = 2 Screens.",
            @"Dump Switch Fitted for hopper dumps: 0 = Note Used. 1 = Require dump switch for emptying hoppers.",
            @"Hand Pay Threshold: Hand Pay above this level. Value is in Cent units. €100 = 10000",
            @"Large Hopper Divert Level: Divert to cashbox when reached this level. Value is in Cent units. €100 = 10000",
            @"Small Hopper Divert Level: Divert to cashbox when reached this level. Value is in Cent units. €100 = 10000",
            @"Volume Control: 0 = Volume set by remote server. 1 = Set via the terminal.",
            @"Hand Pay Only: = 0: Combined Payment. 1 = Hand Pay Only",
            @"OverrideRecycler: 0 = Include Note Payment. 1 = Disable Note Payment",
            @"TiToEnabled: 0: Disabled. 1 = Enabled.",
            @"CommunityMember: 0 = No Community Link. 1 = Community active.",
            @"CommunityMaster: 0 = Not Master. 1 Act as Master.",
            @"CommunityIP: IP Address. E.g. 192.168.1.1"
        };
        
	    #region options
        public string OptionValue { get; set; }
        public string OptionField { get; set; }
        #endregion

        public ChangeType RetChangeType { get; set; }

        public BirthCertSettingsWindow()
		{
			InitializeComponent();
		}
        
        public BirthCertSettingsWindow(string f, string v, int index)
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

            var w = new HelpMessageWindow(_theHelpMessages[index]);
            
            w.Width = 300;
            w.Height = 136;
            w.HorizontalAlignment = HorizontalAlignment.Center;
            w.Left = this.Left - (w.Width / 2);//(this.Left + this.Width) + 10;
            w.Top = this.Top - 150;
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
