﻿using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using PDTUtils.MVVM.ViewModels;
using PDTUtils.MVVM.Models;

namespace PDTUtils.Views
{
    /// <summary>
    /// Interaction logic for GameSettingsView.xaml
    /// </summary>
    public partial class GameSettingsView : UserControl
    {
        public GameSettingsView()
        {
            InitializeComponent();
            DataContext = new GameSettingViewModel();
            //not sure if this is really needed. does it really add anything
            //oy vey what a putz imo.
            var dc = DataContext as GameSettingViewModel;
            if (dc.FirstPromo != null)
                listViewPromo.SelectedItems.Add(dc.FirstPromo);
            if (dc.SecondPromo != null)
                listViewPromo.SelectedItems.Add(dc.SecondPromo);
            
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }
        
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewPromo.SelectedItems.Count > 2)
            {
                listViewPromo.SelectedItems.RemoveAt(0);
            }

            if (listViewPromo.SelectedItems.Count == 1)
            {
                var dc = DataContext as GameSettingViewModel;
                dc.UpdatePromoSelection(listViewPromo.SelectedItems[0] as GameSettingModel, null);
            }
            else if (listViewPromo.SelectedItems.Count == 2)
            {
                var dc = DataContext as GameSettingViewModel;
                dc.UpdatePromoSelection(listViewPromo.SelectedItems[0] as GameSettingModel,
                                        listViewPromo.SelectedItems[1] as GameSettingModel);
            }
        }
    }
}
