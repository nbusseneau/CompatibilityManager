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

    public static class ColorModeHelpers
    {
        private static Dictionary<ColorMode, string> descriptions;
        /// <summary>
        /// ColorMode Description lookup table.
        /// </summary>
        public static Dictionary<ColorMode, string> Descriptions => descriptions ?? (descriptions = EnumHelpers.GetDescriptions<ColorMode>());

        /// <summary>
        /// Convert a ColorMode value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this ColorMode enumValue)
        {
            return EnumHelpers.ToRegistryString(enumValue, ColorModeHelpers.Descriptions);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its ColorMode representation.
        /// </summary>
        public static ColorMode FromRegistryString(string registryString)
        {
            return EnumHelpers.FromRegistryString<ColorMode>(registryString, ColorModeHelpers.Descriptions);
        }
    }
}
