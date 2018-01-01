using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CompatibilityManager.Converters
{
    public class NullableBoolToBoolConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool?)value) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)value;
        }

        private static NullableBoolToBoolConverter converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new NullableBoolToBoolConverter());
        }
    }
}
