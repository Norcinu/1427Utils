﻿using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

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
            this.DataContext = new HopperViewModel();
        }
        //oh well, if he clears then yesh he must be fit.
        private void cmbHoppers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = this.DataContext as HopperViewModel;
            var b = s.EmptyingHoppers;
            //var str = cmbHoppers.SelectedValue.Content as string;
            /*var cmb = sender as ComboBox;
            var str = cmb.SelectedItem.ToString().Substring(cmb.SelectedItem.ToString().Length - 9);
            txtSelHopper.Text = str;*/
        }
    }
}
