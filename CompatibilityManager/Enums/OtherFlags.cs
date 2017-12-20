using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

    public static class OtherExtensions
    {
        /// <summary>
        /// Convert Other flags to their AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this OtherFlags enumValue)
        {
            var appCompatFlags = new List<string>();
            var exceptions = new OtherFlags[] { OtherFlags.None };
            foreach (var flag in Enum.GetValues(typeof(OtherFlags)).Cast<OtherFlags>().Except(exceptions))
            {
                if (enumValue.HasFlag(flag)) { appCompatFlags.Add(flag.GetDescription()); }
            }
            return string.Join(" ", appCompatFlags);
        }
    }
}
