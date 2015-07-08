using System.Windows;
using System.Windows.Controls;
using PDTUtils.Native;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for HopperTestView.xaml
    /// </summary>
    public partial class HopperTestView : UserControl
    {
        public HopperTestView()
        {
            InitializeComponent();
        }

        void btnLeftHopper_Click(object sender, RoutedEventArgs e)
        {
            if (BoLib.getHopperFloatLevel((int)Hoppers.Left) == 0)
            {
                MessageBox.Show("Hopper is Empty. Please place a coin into Hopper.", "Warning", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            
            BoLib.setUtilRequestBitState((int)UtilBits.TestLeftHopper);
            System.Threading.Thread.Sleep(500);
            BoLib.clearUtilRequestBitState((int)UtilBits.TestLeftHopper);
        }
        
        void btnRightHopper_Click(object sender, RoutedEventArgs e)
        {
            if (BoLib.getHopperFloatLevel((int)Hoppers.Right) == 0)
            {
                MessageBox.Show("Hopper is Empty. Please place a coin into Hopper.", "Warning", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            
            BoLib.setUtilRequestBitState((int)UtilBits.TestRightHopper);
            System.Threading.Thread.Sleep(500);
            BoLib.clearUtilRequestBitState((int)UtilBits.TestRightHopper);
        }
    }
}
