using Prism.Mvvm;
using CompatibilityManager.Enums;
using System.Collections.Generic;
using System.Linq;

namespace CompatibilityManager.ViewModels
{
    public class CompatibilityViewModel : BindableBase
    {
        #region Parent ViewModel

        private MainViewModel parent;

        #endregion

        #region Checkboxes properties

        private bool compatibilityModeChecked;
        public bool CompatibilityModeChecked
        {
            get => this.compatibilityModeChecked;
            set => SetProperty(ref this.compatibilityModeChecked, value, () => { if (!value) { this.CompatibilityMode = CompatibilityMode.None; } });
        }

        private bool colorModeChecked;
        public bool ColorModeChecked
        {
            get => this.colorModeChecked;
            set => SetProperty(ref this.colorModeChecked, value, () => { if (!value) { this.ColorMode = ColorMode.None; } });
        }

        private bool dpiScalingChecked;
        public bool DPIScalingChecked
        {
            get => this.dpiScalingChecked;
            set => SetProperty(ref this.dpiScalingChecked, value, () => { if (!value) { this.DPIScaling = DPIScaling.None; } });
        }

        private bool resolution640x480Checked;
        public bool Resolution640x480Checked
        {
            get => this.resolution640x480Checked;
            set => SetProperty(ref this.resolution640x480Checked, value, () => { this.SetOtherFlag(OtherFlags.RESOLUTION640X480, value); });
        }

        private bool disableFullscreenOptimizationsChecked;
        public bool DisableFullscreenOptimizationsChecked
        {
            get => this.disableFullscreenOptimizationsChecked;
            set => SetProperty(ref this.disableFullscreenOptimizationsChecked, value, () => { this.SetOtherFlag(OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE, value); });
        }

        private bool runAsAdministratorChecked;
        public bool RunAsAdministratorChecked
        {
            get => this.runAsAdministratorChecked;
            set => SetProperty(ref this.runAsAdministratorChecked, value, () => { this.SetOtherFlag(OtherFlags.RUNASADMIN, value); });
        }

        private bool hklmChecked;
        public bool HKLMChecked
        {
            get => this.hklmChecked;
            set => SetProperty(ref this.hklmChecked, value);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Set or unset a specific OtherFlag according to given value.
        /// </summary>
        public void SetOtherFlag(OtherFlags flag, bool value)
        {
            if (value) { this.OtherFlags |= flag; }
            else { this.OtherFlags &= ~flag; }
        }

        #endregion

        #region Compatibility properties

        public CompatibilityMode CompatibilityMode { get; set; }
        public ColorMode ColorMode { get; set; }
        public DPIScaling DPIScaling { get; set; }
        public OtherFlags OtherFlags { get; set; }

        public string RegistryString
        {
            get
            {
                var settings = new List<string>()
                {
                    this.CompatibilityMode.ToRegistryString(),
                    this.ColorMode.ToRegistryString(),
                    this.DPIScaling.ToRegistryString(),
                    this.OtherFlags.ToRegistryString(),
                }.Where(s => !string.IsNullOrWhiteSpace(s));

                var appCompatFlags = string.Join(" ", settings);

                // On Windows 8 or above, a tilde is appended at beginning of the string.
                if (!string.IsNullOrWhiteSpace(appCompatFlags) && Services.OSVersionServices.IsWindows8OrAbove())
                {
                    appCompatFlags = string.Format("~ {0}", appCompatFlags);
                }

                return appCompatFlags;
            }
        }

        #endregion

        #region Constructor

        public CompatibilityViewModel(MainViewModel parent)
        {
            this.parent = parent;
        }

        #endregion
    }
}
