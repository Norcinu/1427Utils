using System.Collections.ObjectModel;
using System.Windows.Controls;
using PDTUtils.Logic;
using PDTUtils.MVVM.ViewModels;
 
namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for Diagnostics.xaml
    /// </summary>
    public partial class DiagnosticsView
    {
        public DiagnosticsView()
        {
            InitializeComponent();
            DataContext = new DiagnosticViewModel(new MachineInfo());
        }
        
        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
