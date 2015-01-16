using System.Collections.ObjectModel;
using System.Windows.Controls;
using PDTUtils.MVVM.ViewModels;
 
namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for Diagnostics.xaml
    /// </summary>
    public partial class DiagnosticsView : UserControl
    {
        public DiagnosticsView()
        {
            InitializeComponent();
            this.DataContext = new DiagnosticViewModel();
        }

        
        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var tab = sender as TabControl;
            if (tab.SelectedIndex == 0)
                this.DataContext = this;
            else if (tab.SelectedIndex == 1)
                this.DataContext = new MachineLogsController();*/
        }
    }
}
