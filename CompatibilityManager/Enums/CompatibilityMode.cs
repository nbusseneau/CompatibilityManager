using CompatibilityManager.Services;
using System.Collections.Generic;
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

    public static class CompatibilityModeServices
    {
        private static Dictionary<CompatibilityMode, string> descriptions;
        /// <summary>
        /// CompatibilityMode Description lookup table.
        /// </summary>
        public static Dictionary<CompatibilityMode, string> Descriptions => descriptions ?? (descriptions = EnumServices.GetDescriptions<CompatibilityMode>());

        /// <summary>
        /// Convert a CompatibilityMode value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this CompatibilityMode enumValue)
        {
            return EnumServices.ToRegistryString(enumValue, CompatibilityModeServices.Descriptions);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its CompatibilityMode representation.
        /// </summary>
        public static CompatibilityMode FromRegistryString(string registryString)
        {
            return EnumServices.FromRegistryString<CompatibilityMode>(registryString, CompatibilityModeServices.Descriptions);
        }
    }
}
