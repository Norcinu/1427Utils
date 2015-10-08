using System.Windows;
using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for RouletteBettingView.xaml
    /// </summary>
    public partial class RouletteBettingView : UserControl
    {
        public RouletteBettingView()
        {
            DataContext = new RouletteBettingViewModel();
            InitializeComponent();
        }
        
        /// <summary>
        /// Sigh. I cant get the ListView to update correctly when the value is changed.
        /// So yes I am breaking the MVVM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as RouletteBettingViewModel;
            dc.Items = BetSettings;
        }
    }
}
