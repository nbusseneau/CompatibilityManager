using CompatibilityManager.Services;
using System.Collections.Generic;
using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    public enum ColorMode
    {
        [Browsable(false)] None,
        [Description("256COLOR")] COLOR256,
        [Description("BITCOLOR")] COLOR16BIT,
    }

    public static class ColorModeServices
    {
        private static Dictionary<ColorMode, string> descriptions;
        /// <summary>
        /// ColorMode Description lookup table.
        /// </summary>
        public static Dictionary<ColorMode, string> Descriptions => descriptions ?? (descriptions = EnumServices.GetDescriptions<ColorMode>());

        /// <summary>
        /// Convert a ColorMode value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this ColorMode enumValue)
        {
            return EnumServices.ToRegistryString(enumValue, ColorModeServices.Descriptions);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its ColorMode representation.
        /// </summary>
        public static ColorMode FromRegistryString(string registryString)
        {
            return EnumServices.FromRegistryString<ColorMode>(registryString, ColorModeServices.Descriptions);
        }
    }
}
