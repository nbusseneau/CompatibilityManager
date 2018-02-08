using CompatibilityManager.Services;
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

    public static class DPIScalingServices
    {
        private static Dictionary<DPIScaling, string> descriptions;
        /// <summary>
        /// DPIScaling Description lookup table.
        /// </summary>
        public static Dictionary<DPIScaling, string> Descriptions => descriptions ?? (descriptions = EnumServices.GetDescriptions<DPIScaling>());

        /// <summary>
        /// Convert a DPIScaling value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this DPIScaling enumValue)
        {
            return EnumServices.ToRegistryString(enumValue, DPIScalingServices.Descriptions);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its DPIScaling representation.
        /// </summary>
        public static DPIScaling FromRegistryString(ref List<string> substrings)
        {
            return EnumServices.FromRegistryString<DPIScaling>(ref substrings, DPIScalingServices.Descriptions);
        }
    }
}
