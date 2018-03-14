using System;
using System.Globalization;
using System.Windows.Data;

namespace CompatibilityManager.Converters
{
    [ValueConversion(typeof(bool?), typeof(bool))]
    public class NullableBoolToBoolConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool?)value) ?? false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)value;
        }
    }
}
