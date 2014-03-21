using System.Windows;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RowOne.Height = new GridLength(75);
            ColumnOne.Width = new GridLength(200);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
        }

		private void btnHoppers_Click(object sender, RoutedEventArgs e)
		{
			if (MyTab.Visibility == System.Windows.Visibility.Visible)
				MyTab.Visibility = System.Windows.Visibility.Hidden;
			else
				MyTab.Visibility = System.Windows.Visibility.Visible;
		}

		private void btnLogfiles_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnHopperOK_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
				MyTab.Visibility = System.Windows.Visibility.Hidden;
		}
    }
}
