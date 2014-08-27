using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
											new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(
											CultureInfo.CurrentCulture.IetfLanguageTag)));
		}        
    }
}
