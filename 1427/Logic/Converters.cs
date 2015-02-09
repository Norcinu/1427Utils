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
            else if (str.ToString().Contains(".raw") == true)
                return @Resources.FILE_TYPE_RAW;
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
    
    /// <summary>
    /// Converts the screen height and scales the grid size.
    /// </summary>
    public class ConvertScreenHeight : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            if (screenHeight == 1080)
                return 956;
            else if (screenHeight == 768)
                return 645;
            else
                return 479;
        }
        
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Method not implemented.");
        }
    }

    #region Yuk
    //---- Yuk Yuk Yuk
    public class IsEnglishCulture : IValueConverter
    {
        public IsEnglishCulture() { }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (culture.TwoLetterISOLanguageName == "en")
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return new NotImplementedException("Not Implemented");
        }
    }

    public class IsSpanishCulture : IValueConverter
    {
        public IsSpanishCulture() { }
        
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (culture.TwoLetterISOLanguageName == "es")
                return true;
            else
                return false;
        }
        
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return new NotImplementedException("Not Implemented");
        }
    }
    //---- Yuk Yuk Yuk    
    #endregion

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

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class FalseToHiddenConv : IValueConverter
    {
        public bool Reverse { get; set; }
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public FalseToHiddenConv()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            return !(bool)value ? TrueValue : FalseValue;
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
    
    [ValueConversion(typeof(decimal), typeof(Visibility))]
    public sealed class ConvertStakeVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return false;

            string stripper = value as string;
            string[] ss = stripper.Split("£$€,.".ToCharArray());
            bool ret = false;
            try
            {
                ret = System.Convert.ToDecimal(ss[1]) == 0 ? false : true;
            }
            catch(Exception ex)
            {
                ret = false;
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return ret;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return false;

            return ((bool)value == false) ? 0 : 1;
        }
    }
    
    public class MultibindConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object[] { value[0], value[1] };
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
