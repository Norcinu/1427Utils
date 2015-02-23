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
using PDTUtils.MVVM.ViewModels;
using PDTUtils.MVVM.Models;
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
            this.DataContext = new BirthCertViewModel();
        }

        void UpdateIniItem(object sender)
        {
            var l = sender as ListView;
            if (l.SelectedIndex == -1)
                return;
            
            var c = l.Items[l.SelectedIndex] as BirthCertModel;
            var items = l.ItemsSource;

            IniSettingsWindow w = new IniSettingsWindow(c.Field, c.Value);
            w.btnComment.IsEnabled = false;
            w.btnComment.Visibility = Visibility.Hidden;
            if (w.ShowDialog() == false)
            {
                switch (w.RetChangeType)
                {
                    case ChangeType.AMEND:
                        AmendOption(w, sender, ref c);
                        l.SelectedIndex = -1;
                        break;
                    case ChangeType.CANCEL:
                        l.SelectedIndex = -1;
                        break;
                    default:
                        break;
                }
            }
        }
        
        void AmendOption(IniSettingsWindow w, object sender, ref BirthCertModel c)
        {
            string newValue = w.OptionValue;
            
            var listView = sender as ListView;
            var current = listView.Items[listView.SelectedIndex] as BirthCertModel;
            //oh well I've commited to it now :|
            if (newValue != c.Value || (newValue == c.Value && current.Field[0] == '#'))
            {
                current.Value = newValue;
                current.Value = newValue;
                listView.Items.Refresh();
                
                NativeWinApi.WritePrivateProfileString("Config", c.Field, c.Value, Properties.Resources.birth_cert);
            }
        }
        
        void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIniItem(sender);
        }
    }
}
