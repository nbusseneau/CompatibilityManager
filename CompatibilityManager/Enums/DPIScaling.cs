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

    public static class DPIScalingExtensions
    {
        /// <summary>
        /// Convert a DPIScaling value to its AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this DPIScaling enumValue)
        {
            return enumValue.GetDescription();
        }
    }
}
