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
}
