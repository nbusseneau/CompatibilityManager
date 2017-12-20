using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    public enum ColorMode
    {
        [Browsable(false)] None,
        [Description("256COLOR")] COLOR256,
        [Description("BITCOLOR")] COLOR16BIT,
    }

    public static class ColorModeExtensions
    {
        /// <summary>
        /// Convert a ColorMode value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this ColorMode enumValue)
        {
            return enumValue.GetDescription();
        }
    }
}
