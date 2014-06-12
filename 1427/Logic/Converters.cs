using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PDTUtils
{
	/// <summary>
	/// Used to set the colour of the grid cell containing the MD5 signature
	/// in the system settings view.
	/// </summary>
	public class GridColourConversion : IValueConverter
	{
		public object Convert(object value, Type targetType, 
							  object parameter, CultureInfo culture)
		{
			var input = value as string;
			
			if (input == "ERROR: NOT AUTHORISED") // Make this a resource
				return Brushes.Red;
			else if (input == "")
				return Brushes.Pink;
			else
				return Brushes.Green;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			return new NotImplementedException();
		}
	}
}
