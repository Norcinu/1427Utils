﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for NoteAdminView.xaml
    /// </summary>
    public partial class NoteAdminView : UserControl
    {
        public NoteAdminView()
        {
            InitializeComponent();
            DataContext = new PDTUtils.MVVM.ViewModels.NoteAdminViewModel();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // var dc = DataContext as PDTUtils.MVVM.ViewModels.NoteAdminViewModel();
         
        }
    }
}