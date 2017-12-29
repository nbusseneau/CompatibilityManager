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

    public static class OtherFlagsHelpers
    {
        private static Dictionary<OtherFlags, string> descriptions;
        /// <summary>
        /// OtherFlags Description lookup table.
        /// </summary>
        public static Dictionary<OtherFlags, string> Descriptions => descriptions ?? (descriptions = EnumHelpers.GetDescriptions<OtherFlags>());

        /// <summary>
        /// Convert Other flags to their AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(this OtherFlags enumValue)
        {
            var appCompatFlags = new List<string>();
            foreach (var flag in Enum.GetValues(typeof(OtherFlags)).Cast<OtherFlags>())
            {
                var description = OtherFlagsHelpers.Descriptions[flag];
                if (enumValue.HasFlag(flag) && !string.IsNullOrWhiteSpace(description)) { appCompatFlags.Add(description); }
            }
            return string.Join(" ", appCompatFlags);
        }

        /// <summary>
        /// Convert an AppCompatFlag REG_SZ to its OtherFlags representation.
        /// </summary>
        public static OtherFlags FromRegistryString(string registryString)
        {
            var otherFlags = OtherFlags.None;
            var substrings = registryString.Split();
            foreach (var substring in substrings)
            {
                var matches = OtherFlagsHelpers.Descriptions.Where(kvp => kvp.Value.Equals(substring));
                if (matches.Any()) { otherFlags |= matches.First().Key; }
            }
            return otherFlags;
        }
    }
}
