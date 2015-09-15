using System.Windows;
using System.Windows.Controls;
using PDTUtils.MVVM.Models;
using PDTUtils.MVVM.ViewModels;
using PDTUtils.Native;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for BirthCertView.xaml
    /// </summary>
    public partial class BirthCertView : UserControl
    {
        public BirthCertView()
        {
            InitializeComponent();
            DataContext = new BirthCertViewModel();
        }
        //wanna geta  papa johns imo. would be quality to get one instead of 
        //
        void UpdateIniItem(object sender)
        {
            var l = sender as ListView;
            if (l.SelectedIndex == -1)
                return;

            var c = l.Items[l.SelectedIndex] as BirthCertModel;
            /*var dc = DataContext as BirthCertViewModel;
            dc.SetHelpMessage(l.SelectedIndex);*/

            var w = new BirthCertSettingsWindow(c.Field, c.Value, l.SelectedIndex)
            {
                BtnComment = { IsEnabled = false, Visibility = Visibility.Hidden }
            };

            if (w.ShowDialog() != false) return;
            switch (w.RetChangeType)
            {
                case ChangeType.Amend:
                    AmendOption(w, sender, ref c);
                    l.SelectedIndex = -1;
                    break;
                case ChangeType.Cancel:
                    l.SelectedIndex = -1;
                    break;
            }
        }

        void AmendOption(BirthCertSettingsWindow w, object sender, ref BirthCertModel c)
        {
            var newValue = w.OptionValue;
            
            var listView = sender as ListView;
            
            if (listView == null) return;
            var current = listView.Items[listView.SelectedIndex] as BirthCertModel;

            if (newValue == c.Value && (newValue != c.Value || current.Field[0] != '#')) return;
            
            current.Value = newValue;
            current.Value = newValue;
            listView.Items.Refresh();
                  
            NativeWinApi.WritePrivateProfileString("Config", c.Field, c.Value, Properties.Resources.birth_cert);
        }
        
        void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIniItem(sender);
        }
    }
}
