using System.Windows.Controls;
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
            DataContext = new HopperViewModel();
        }
        
        private void cmbHoppers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var s = DataContext as HopperViewModel;
            var b = s.EmptyingHoppers;*/
            //var str = cmbHoppers.SelectedValue.Content as string;
            /*var cmb = sender as ComboBox;
            var str = cmb.SelectedItem.ToString().Substring(cmb.SelectedItem.ToString().Length - 9);
            txtSelHopper.Text = str;*/
        }
    }
}
