﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PDTUtils.Properties;
using PDTUtils.Access;

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

		    switch (input)
		    {
		        case "ERROR: NOT AUTHORISED":
		            return Brushes.Red;
		        case "":
		            return Brushes.Pink;
		        default:
		            return Brushes.Green;
		    }
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
			var str = value as string;
			if (str[str.Length - 4] != '.')
				return @Resources.FILE_TYPE_FOLDER;
		    if (str.Contains(".png"))
		        return @Resources.FILE_TYPE_IMG;
		    if (str.Contains(".wav"))
		        return @Resources.FILE_TYPE_WAV;
		    if (str.Contains(".ini"))
		        return @Resources.FILE_TYPE_INI;
		    if (str.Contains(".exe"))
		        return @Resources.FILE_TYPE_EXE;
		    
            return str.Contains(".raw") ? @Resources.FILE_TYPE_RAW : @Resources.FILE_TYPE_UNKNOWN;
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
            if (Math.Abs(screenHeight - 1080) < double.Epsilon)
                return 956;
            return Math.Abs(screenHeight - 768) < double.Epsilon ? 645 : 479;
        }
        
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Method not implemented.");
        }
    }

    public class NegateBoolValue : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var conversion = value as bool?;
            return conversion != true;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            var conversion = value as bool?;
            return conversion == true;
        }
    }

    #region Yuk
    //---- Yuk Yuk Yuk
    public class IsEnglishCulture : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (culture.TwoLetterISOLanguageName == "en")
                return true;
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
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (culture.TwoLetterISOLanguageName == "es")
                return true;
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
		
            return Equals(value, FalseValue) ? (object) false : null;
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

            var stripper = value as string;
            var ss = stripper.Split("£$€,.".ToCharArray());
            bool ret;
            try
            {
                ret = System.Convert.ToDecimal(ss[1]) != 0;
            }
            catch(Exception ex)
            {
                ret = false;
                Debug.WriteLine(ex.Message);
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
            return new[] { value[0], value[1] };
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class SmartCardStateConverter : IValueConverter
    {
        public bool Reverse { get; set; }
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public SmartCardStateConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            /*var paramString = parameter as string;*/
            return (bool)value ? TrueValue : FalseValue;
        }
        
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;

            return Equals(value, FalseValue) ? (object)false : null;
        }
    }

    /**
     * Smartcard Converters
     **/

    /// <summary>
    /// 
    /// </summary>
    /*[ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class CashierLevelConv : IValueConverter
    {
        
    }*/

    /// <summary>
    /// For both levels above cashier
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class OperatorAndManuLevelConv : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public OperatorAndManuLevelConv()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;


            return (GlobalAccess.Level > 2) ? TrueValue : FalseValue;
        }
        
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;

            return Equals(value, FalseValue) ? (object)false : null;
        }
    }

    /// <summary>
    /// for only manufacturer
    /// </summary>
/*
    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class ManufactureOnlyConv : IValueConverter
    {

    }*/

    


 /*   [ValueConversion(typeof(bool), typeof(Visibility))]
    public class SmartCardStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramString = parameter as string;
            
            if (string.IsNullOrEmpty(paramString))
            {
                var split = paramString.Split("|");

            }

            return null;
        }
        
        public object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException("SmartCardStateConverter::ConvertBack Not Implemented");
        }

    }*/
}
