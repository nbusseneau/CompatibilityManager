using System;
using System.Globalization;
using System.Windows.Data;

namespace CompatibilityManager.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
