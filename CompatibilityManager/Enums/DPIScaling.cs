using System.Collections.Generic;
using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    public enum DPIScaling
    {
        [Browsable(false)] None,
        HIGHDPIAWARE,
        DPIUNAWARE,
        [Description("GDIDPISCALING DPIUNAWARE")] GDIDPISCALING,
    }

    public static class DPIScalingHelpers
    {
        private static Dictionary<DPIScaling, string> descriptions;
        /// <summary>
        /// DPIScaling Description lookup table.
        /// </summary>
        public static Dictionary<DPIScaling, string> Descriptions => descriptions ?? (descriptions = EnumHelpers.GetDescriptions<DPIScaling>());

        /// <summary>
        /// Convert a DPIScaling value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this DPIScaling enumValue)
        {
            return EnumHelpers.ToRegistryString(enumValue, DPIScalingHelpers.Descriptions);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its DPIScaling representation.
        /// </summary>
        public static DPIScaling FromRegistryString(string registryString)
        {
            return EnumHelpers.FromRegistryString<DPIScaling>(registryString, DPIScalingHelpers.Descriptions);
        }
    }
}
