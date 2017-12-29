using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CompatibilityManager.Enums
{
    /// <summary>
    /// Generic helper and extension methods for Enum types.
    /// The 'where T : struct, IComparable, IFormattable, IConvertible' hack is to try and constraint on Enum types only.
    /// </summary>
    public static class EnumHelpers
    {
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
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException(string.Format("{0} must be an Enum type.", nameof(TEnum))); }
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
        public static string ToRegistryString<TEnum>(this TEnum enumValue, Dictionary<TEnum, string> descriptions)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return descriptions[enumValue];
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its TEnum representation based on a Description lookup table.
        /// </summary>
        public static TEnum FromRegistryString<TEnum>(string registryString, Dictionary<TEnum, string> descriptions)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var matches = descriptions.Where(kvp => registryString.Contains(kvp.Value));
            if (matches.Any()) { return matches.Last().Key; }
            return default(TEnum);

            // For future reference, the reason for using Last instead of First is due to DPIScaling:
            //  DPIScaling.DPIUNAWARE => "DPIUNAWARE"
            //  DPIScaling.GDIDPISCALING => "GDIDPISCALING DPIUNAWARE"
            // Since GDIDPISCALING registry string also contains DPIUNAWARE, we have to make sure we don't use DPIUNAWARE instead of GDIDPISCALING by mistake.
        }
    }
}
