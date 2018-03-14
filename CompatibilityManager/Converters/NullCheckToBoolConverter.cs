using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CompatibilityManager.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullCheckToBoolConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
