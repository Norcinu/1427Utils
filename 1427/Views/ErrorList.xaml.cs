using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for ErrorList.xaml
    /// </summary>
    public partial class ErrorList : UserControl
    {
        public ErrorList()
        {
            InitializeComponent();
            this.DataContext = new ErrorStatusViewModel();
        }
    }
}
