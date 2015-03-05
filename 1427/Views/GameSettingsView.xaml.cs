using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using PDTUtils.MVVM.ViewModels;

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
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }
    }
}
