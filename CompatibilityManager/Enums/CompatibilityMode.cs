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
}
