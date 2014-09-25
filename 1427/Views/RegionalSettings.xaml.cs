using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for RegionalSettings.xaml
    /// </summary>
    public partial class RegionalSettings : UserControl
    {
        public RegionalSettings()
        {
            InitializeComponent();
            this.DataContext = new RegionalSettingsViewModel();
        }
    }
}
