using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    public enum CompatibilityMode
    {
        [Browsable(false)] None,
        WIN95,
        WIN98,
        WINXPSP2,
        WINXPSP3,
        VISTARTM,
        VISTASP1,
        VISTASP2,
        WIN7RTM,
        WIN8RTM,
    }

    public static class CompatibilityModeExtensions
    {
        /// <summary>
        /// Convert a CompatibilityMode value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this CompatibilityMode enumValue)
        {
            return enumValue.GetDescription();
        }
    }
}
