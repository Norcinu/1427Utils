using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using PDTUtils.Logic;

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
			if (str.Contains(".png") == true)
				return @"D:\1427\bmp\bitmap.bmp";
			else if (str.Contains(".wav") == true)
				return @"D:\1427\bmp\wav.bmp";
			else if (str.ToString().Contains(".ini") == true)
				return @"D:\1427\bmp\ini.bmp";
			else if (str.ToString().Contains(".exe") == true)
				return @"D:\1427\bmp\exe.bmp";
			else
				return @"D:\1427\bmp\unknown.png";	
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
			{
				return null;
			}			
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

	/*public class CustomBoolToVisConverter : IValueConverter
	{

	}*/
}
