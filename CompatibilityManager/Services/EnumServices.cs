using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CompatibilityManager.Services
{
    /// <summary>
    /// Generic helper and extension methods for Enum types.
    /// The 'where T : struct, IComparable, IFormattable, IConvertible' hack is to try and constraint on Enum types only.
    /// </summary>
    public static class EnumServices
    {
        /// <summary>
        /// Check that provided type is an Enum type. Throw ArgumentException otherwise.
        /// </summary>
        public static void TEnumTypeCheck<TEnum>()
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException(string.Format(Resources.Strings.EnumTypeArgumentException, nameof(TEnum))); }
        }

        private static TAttribute GetAttributeOfType<TAttribute, TEnum>(this TEnum enumValue)
            where TAttribute : Attribute
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute(typeof(TAttribute), false) as TAttribute;
        }

        /// <summary>
        /// Get an Enum value's Description. If not specified, defaults to ToString(). If marked as non-browsable, defaults to string.Empty.
        /// </summary>
        /// <typeparam name="TEnum">Must be an Enum type.</typeparam>
        public static string GetDescription<TEnum>(this TEnum enumValue)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnumTypeCheck<TEnum>();

            var browsable = enumValue.GetAttributeOfType<BrowsableAttribute, TEnum>()?.Browsable ?? true;
            var description = enumValue.GetAttributeOfType<DescriptionAttribute, TEnum>()?.Description ?? enumValue.ToString();
            return browsable ? description : string.Empty;
        }

        /// <summary>
        /// Get a dictionary of all Enum values associated to their Descriptions(as returned by GetDescription()) from an Enum type.
        /// </summary>
        /// <typeparam name="TEnum">Must be an Enum type.</typeparam>
        public static Dictionary<TEnum, string> GetDescriptions<TEnum>()
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnumTypeCheck<TEnum>();

            var descriptions = new Dictionary<TEnum, string>();
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                descriptions.Add(enumValue, enumValue.GetDescription());
            }
            return descriptions;
        }

        /// <summary>
        /// Convert a TEnum value to its AppCompatFlag REG_SZ representation based on a Description lookup table.
        /// </summary>
        /// <typeparam name="TEnum">Must be an Enum type.</typeparam>
        public static string ToRegistryString<TEnum>(this TEnum enumValue, Dictionary<TEnum, string> descriptions)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnumTypeCheck<TEnum>();

            return descriptions[enumValue];
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its TEnum representation based on a Description lookup table.
        /// </summary>
        /// <typeparam name="TEnum">Must be an Enum type.</typeparam>
        public static TEnum FromRegistryString<TEnum>(ref List<string> substrings, Dictionary<TEnum, string> descriptions)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnumTypeCheck<TEnum>();

            // For future reference, the reason for such a complicated matching is due to DPIScaling:
            //  DPIScaling.DPIUNAWARE => "DPIUNAWARE"
            //  DPIScaling.GDIDPISCALING => "GDIDPISCALING DPIUNAWARE"
            // Since GDIDPISCALING registry string also contains DPIUNAWARE, we have to make sure we don't use DPIUNAWARE instead of GDIDPISCALING by mistake.
            // Instead, we match on all separate strings found in the description.
            var reverse = descriptions.Reverse();
            foreach (var kvp in reverse)
            {
                var match = true;
                var values = kvp.Value.Split(null);

                foreach (var value in values)
                {
                    match &= substrings.Contains(value);
                }

                if (match)
                {
                    foreach (var value in values) { substrings.Remove(value); }
                    return kvp.Key;
                }
            }
            return default(TEnum);            
        }
    }
}
