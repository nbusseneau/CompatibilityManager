using CompatibilityManager.Enums;
using CompatibilityManager.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace CompatibilityManager.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        #region Properties

        #region State

        protected bool hasChanged;
        public bool HasChanged
        {
            get => this.hasChanged;
            set => SetProperty(ref this.hasChanged, value);
        }
        
        public bool IsCleared =>
            this.CompatibilityMode == CompatibilityMode.None
            && this.ColorMode == ColorMode.None
            && this.DPIScaling == DPIScaling.None
            && this.OtherFlags == OtherFlags.None;

        public virtual string Title => Resources.Strings.ApplicationSettings;

        #endregion

        #region Compatibility settings
        
        protected CompatibilityMode compatibilityMode;
        public CompatibilityMode CompatibilityMode
        {
            get => (this.CompatibilityModeChecked ?? false) ? this.compatibilityMode : CompatibilityMode.None;
            set => SetProperty(ref this.compatibilityMode, value, this.OnSettingsChanged);
        }

        protected ColorMode colorMode;
        public ColorMode ColorMode
        {
            get => (this.ColorModeChecked ?? false) ? this.colorMode : ColorMode.None;
            set => SetProperty(ref this.colorMode, value, this.OnSettingsChanged);
        }

        protected DPIScaling dpiScaling;
        public DPIScaling DPIScaling
        {
            get => (this.DPIScalingChecked ?? false) ? this.dpiScaling : DPIScaling.None;
            set => SetProperty(ref this.dpiScaling, value, this.OnSettingsChanged);
        }

        protected OtherFlags otherFlags;
        public OtherFlags OtherFlags
        {
            get => this.otherFlags;
            set => SetProperty(ref this.otherFlags, value, this.OnSettingsChanged);
        }

        #endregion

        #region Checkboxes

        protected bool? compatibilityModeChecked = false;
        public bool? CompatibilityModeChecked
        {
            get => this.compatibilityModeChecked;
            set => SetProperty(ref this.compatibilityModeChecked, value, this.OnSettingsChanged);
        }

        protected bool? colorModeChecked = false;
        public bool? ColorModeChecked
        {
            get => this.colorModeChecked;
            set => SetProperty(ref this.colorModeChecked, value, this.OnSettingsChanged);
        }

        protected bool? dpiScalingChecked = false;
        public bool? DPIScalingChecked
        {
            get => this.dpiScalingChecked;
            set => SetProperty(ref this.dpiScalingChecked, value, this.OnSettingsChanged);
        }

        protected bool? resolution640x480Checked = false;
        public bool? Resolution640x480Checked
        {
            get => this.resolution640x480Checked;
            set
            {
                SetProperty(ref this.resolution640x480Checked, value);
                SetFlagProperty(ref this.otherFlags, OtherFlags.RESOLUTION640X480, value, this.OnSettingsChanged, nameof(this.OtherFlags));
            }
        }

        protected bool? disableFullscreenOptimizationsChecked = false;
        public bool? DisableFullscreenOptimizationsChecked
        {
            get => this.disableFullscreenOptimizationsChecked;
            set
            {
                SetProperty(ref this.disableFullscreenOptimizationsChecked, value, this.OnSettingsChanged);
                SetFlagProperty(ref this.otherFlags, OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE, value, this.OnSettingsChanged, nameof(this.OtherFlags));
            }
        }

        protected bool? runAsAdministratorChecked = false;
        public bool? RunAsAdministratorChecked
        {
            get => this.runAsAdministratorChecked;
            set
            {
                SetProperty(ref this.runAsAdministratorChecked, value, this.OnSettingsChanged);
                SetFlagProperty(ref this.otherFlags, OtherFlags.RUNASADMIN, value, this.OnSettingsChanged, nameof(this.OtherFlags));
            }
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand ClearCommand { get; private set; }

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            this.ClearCommand = new DelegateCommand(this.Clear);
        }

        public SettingsViewModel(string registryString) : this()
        {
            this.suppressSettingsChanged = true;

            // Initialize compatibility settings
            this.CompatibilityMode = CompatibilityModeServices.FromRegistryString(registryString);
            this.ColorMode = ColorModeServices.FromRegistryString(registryString);
            this.DPIScaling = DPIScalingServices.FromRegistryString(registryString);
            this.OtherFlags = OtherFlagsServices.FromRegistryString(registryString);

            // Initialize checkboxes
            this.CompatibilityModeChecked = this.compatibilityMode != CompatibilityMode.None;
            this.ColorModeChecked = this.colorMode != ColorMode.None;
            this.DPIScalingChecked = this.dpiScaling != DPIScaling.None;
            this.Resolution640x480Checked = this.OtherFlags.HasFlag(OtherFlags.RESOLUTION640X480);
            this.DisableFullscreenOptimizationsChecked = this.OtherFlags.HasFlag(OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE);
            this.RunAsAdministratorChecked = this.OtherFlags.HasFlag(OtherFlags.RUNASADMIN);

            this.suppressSettingsChanged = false;
        }

        #endregion

        #region Helper methods

        private bool suppressSettingsChanged;
        public void OnSettingsChanged()
        {
            if (!this.suppressSettingsChanged)
            {
                this.HasChanged = true;
                RaisePropertyChanged(nameof(this.IsCleared));
            }
        }

        public string ToRegistryString()
        {
            return RegistryServices.ToRegistryString(this.CompatibilityMode, this.ColorMode, this.DPIScaling, this.OtherFlags);
        }

        #endregion

        #region Command executes

        private void Clear()
        {
            this.CompatibilityModeChecked = false;
            this.ColorModeChecked = false;
            this.DPIScalingChecked = false;
            this.Resolution640x480Checked = false;
            this.DisableFullscreenOptimizationsChecked = false;
            this.RunAsAdministratorChecked = false;
        }

        #endregion

        #region SetProperty overrides

        private bool SetFlagProperty(ref OtherFlags storage, OtherFlags flag, bool? value, Action onChanged, string propertyName)
        {
            OtherFlags newValue;
            if (value ?? false) { newValue = storage | flag; }
            else { newValue = storage & ~flag; }

            return SetProperty(ref storage, newValue, onChanged, propertyName);
        }

        #endregion
    }
}
