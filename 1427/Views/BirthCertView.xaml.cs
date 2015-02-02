using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDTUtils.MVVM.ViewModels;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for BirthCertView.xaml
    /// </summary>
    public partial class BirthCertView : UserControl
    {
        public BirthCertView()
        {
            InitializeComponent();
            this.DataContext = new BirthCertViewModel();
        }
    }
}
