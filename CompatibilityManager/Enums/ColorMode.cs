using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    public enum ColorMode
    {
        [Browsable(false)] None,
        [Description("256COLOR")] COLOR256,
        [Description("BITCOLOR")] COLOR16BIT,
    }
}
