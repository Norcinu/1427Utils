using System.Windows.Controls;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for UsbSoftwareUpdateView.xaml
    /// </summary>
    public partial class UsbSoftwareUpdateView : UserControl
    {
        PDTUtils.UserSoftwareUpdate usd = new PDTUtils.UserSoftwareUpdate();
        public PDTUtils.UserSoftwareUpdate Usd { get { return usd; } }
        
        public UsbSoftwareUpdateView()
        {
            InitializeComponent();
            usd.DoSoftwareUpdatePreparation();
        }
    }
}
