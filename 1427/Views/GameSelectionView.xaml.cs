using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for GameSelection.xaml
    /// </summary>
    public partial class GameSelection : UserControl
    {
        public GameSelection()
        {
            InitializeComponent();
            this.DataContext = new GameSelectionViewModel();
        }
    }
}
