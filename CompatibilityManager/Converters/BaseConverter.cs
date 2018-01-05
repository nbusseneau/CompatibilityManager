using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CompatibilityManager.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public abstract class BaseConverter<T> : MarkupExtension, IValueConverter
        where T : class, IValueConverter, new()
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        private static T converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new T());
        }
    }
}
