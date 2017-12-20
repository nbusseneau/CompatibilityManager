using System;
using System.ComponentModel;

namespace CompatibilityManager.Enums
{
    [Flags]
    public enum OtherFlags
    {
        [Browsable(false)] None                    = 0b000,
        [Description("640X480")] RESOLUTION640X480 = 0b001,
        DISABLEDXMAXIMIZEDWINDOWEDMODE             = 0b010,
        RUNASADMIN                                 = 0b100,
    }
}
