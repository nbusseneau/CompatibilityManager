using System;
using System.ComponentModel;
using System.Reflection;

namespace CompatibilityManager.Enums
{
    /// <summary>
    /// Generic extension methods for Enum types.
    /// The 'where T : struct, IComparable, IFormattable, IConvertible' hack is to try and constraint on Enum types only.
    /// </summary>
    public static class EnumExtensions
    {
        private static TAttribute GetAttributeOfType<TAttribute, TEnum>(this TEnum enumValue)
            where TAttribute : Attribute
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute(typeof(TAttribute), false) as TAttribute;
        }

        /// <summary>
        /// Get an Enum value's Description, unless marked as non-Browsable. If not specified, defaults to ToString() instead.
        /// </summary>
        /// <typeparam name="TEnum">Must be an Enum type.</typeparam>
        public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException(string.Format("{0} must be an Enum type.", nameof(TEnum))); }
            var browsable = enumValue.GetAttributeOfType<BrowsableAttribute, TEnum>()?.Browsable ?? true;
            var description = enumValue.GetAttributeOfType<DescriptionAttribute, TEnum>()?.Description ?? enumValue.ToString();
            return browsable ? description : string.Empty;
        }
    }
}
