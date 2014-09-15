using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using PDTUtils.Logic;
using PDTUtils.Properties;

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

	public class CustomImagePathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, 
									   object parameter, CultureInfo culture)
		{
			string str = value as string;
			if (str[str.Length - 4] != '.')
				return @Resources.FILE_TYPE_FOLDER;
			else if (str.Contains(".png") == true)
				return @Resources.FILE_TYPE_IMG;
			else if (str.Contains(".wav") == true)
				return @Resources.FILE_TYPE_WAV;
			else if (str.ToString().Contains(".ini") == true)
				return @Resources.FILE_TYPE_INI;
			else if (str.ToString().Contains(".exe") == true)
				return @Resources.FILE_TYPE_EXE;
			else
				return @Resources.FILE_TYPE_UNKNOWN;
		}

		public object ConvertBack(object value, Type targetType, 
										   object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class CheckStringIsFileOrPath : IValueConverter
	{
		public object Convert(object value, Type targetType, 
									   object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public object ConvertBack(object value, Type targetType, 
										   object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

    [ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
        public bool Reverse { get; set; }
		public Visibility TrueValue { get; set; }
		public Visibility FalseValue { get; set; }

		public BoolToVisibilityConverter()
		{
			TrueValue = Visibility.Visible;
			FalseValue = Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			if (!(value is bool))
				return null;

			return (bool)value ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			if (Equals(value, TrueValue))
				return true;
			if (Equals(value, FalseValue))
				return false;
			return null;
		}
	}
}
