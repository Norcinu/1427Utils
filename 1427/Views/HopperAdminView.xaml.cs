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

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for HopperAdminView.xaml
    /// </summary>
    public partial class HopperAdminView : UserControl
    {
        public HopperAdminView()
        {
            InitializeComponent();
        }

        private void cmbHoppers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var str = cmbHoppers.SelectedValue.Content as string;
            var cmb = sender as ComboBox;
            var str = cmb.SelectedItem.ToString().Substring(cmb.SelectedItem.ToString().Length - 9);
            txtSelHopper.Text = str;
        }
    }
}
