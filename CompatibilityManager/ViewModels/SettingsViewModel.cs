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
            set
            {
                SetProperty(ref this.compatibilityMode, value, this.OnSettingsChanged);
                SaveSettings(this.compatibilityMode);
            }
        }

        protected ColorMode colorMode;
        public ColorMode ColorMode
        {
            get => (this.ColorModeChecked ?? false) ? this.colorMode : ColorMode.None;
            set
            {
                SetProperty(ref this.colorMode, value, this.OnSettingsChanged);
                SaveSettings(this.colorMode);
            }
        }

        protected DPIScaling dpiScaling;
        public DPIScaling DPIScaling
        {
            get => (this.DPIScalingChecked ?? false) ? this.dpiScaling : DPIScaling.None;
            set
            {
                SetProperty(ref this.dpiScaling, value, this.OnSettingsChanged);
                SaveSettings(this.dpiScaling);
            }
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
            set
            {
                SetProperty(ref this.compatibilityModeChecked, value);
                SetEnumProperty(ref this.compatibilityMode, Properties.Settings.Default.LastCompatibilityMode, value, this.OnSettingsChanged, nameof(this.CompatibilityMode));
            }
        }

        protected bool? colorModeChecked = false;
        public bool? ColorModeChecked
        {
            get => this.colorModeChecked;
            set
            {
                SetProperty(ref this.colorModeChecked, value);
                SetEnumProperty(ref this.colorMode, Properties.Settings.Default.LastColorMode, value, this.OnSettingsChanged, nameof(this.ColorMode));
            }
        }

        protected bool? dpiScalingChecked = false;
        public bool? DPIScalingChecked
        {
            get => this.dpiScalingChecked;
            set
            {
                SetProperty(ref this.dpiScalingChecked, value);
                SetEnumProperty(ref this.dpiScaling, Properties.Settings.Default.LastDPIScaling, value, this.OnSettingsChanged, nameof(this.DPIScaling));
            }
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
                SetProperty(ref this.disableFullscreenOptimizationsChecked, value);
                SetFlagProperty(ref this.otherFlags, OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE, value, this.OnSettingsChanged, nameof(this.OtherFlags));
            }
        }

        protected bool? runAsAdministratorChecked = false;
        public bool? RunAsAdministratorChecked
        {
            get => this.runAsAdministratorChecked;
            set
            {
                SetProperty(ref this.runAsAdministratorChecked, value);
                SetFlagProperty(ref this.otherFlags, OtherFlags.RUNASADMIN, value, this.OnSettingsChanged, nameof(this.OtherFlags));
            }
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand ClearCommand { get; protected set; }

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            this.ClearCommand = new DelegateCommand(this.Clear);
        }

        public SettingsViewModel(string registryString) : this()
        {
            // Initialize compatibility settings
            this.compatibilityMode = CompatibilityModeServices.FromRegistryString(registryString);
            this.colorMode = ColorModeServices.FromRegistryString(registryString);
            this.dpiScaling = DPIScalingServices.FromRegistryString(registryString);
            this.otherFlags = OtherFlagsServices.FromRegistryString(registryString);

            // Initialize checkboxes
            this.compatibilityModeChecked = this.compatibilityMode != CompatibilityMode.None;
            this.colorModeChecked = this.colorMode != ColorMode.None;
            this.dpiScalingChecked = this.dpiScaling != DPIScaling.None;
            this.resolution640x480Checked = this.OtherFlags.HasFlag(OtherFlags.RESOLUTION640X480);
            this.disableFullscreenOptimizationsChecked = this.OtherFlags.HasFlag(OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE);
            this.runAsAdministratorChecked = this.OtherFlags.HasFlag(OtherFlags.RUNASADMIN);
        }

        #endregion

        #region Helper methods

        protected void SaveSettings(CompatibilityMode compatibilityMode)
        {
            Properties.Settings.Default.LastCompatibilityMode = compatibilityMode;
            Properties.Settings.Default.Save();
        }

        protected void SaveSettings(ColorMode colorMode)
        {
            Properties.Settings.Default.LastColorMode = colorMode;
            Properties.Settings.Default.Save();
        }

        protected void SaveSettings(DPIScaling dpiScaling)
        {
            Properties.Settings.Default.LastDPIScaling = dpiScaling;
            Properties.Settings.Default.Save();
        }

        protected void OnSettingsChanged()
        {
            this.HasChanged = true;
            RaisePropertyChanged(nameof(this.IsCleared));
        }

        public string ToRegistryString()
        {
            return RegistryServices.ToRegistryString(this.CompatibilityMode, this.ColorMode, this.DPIScaling, this.OtherFlags);
        }

        #endregion

        #region Command executes

        protected void Clear()
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

        /// <summary>
        /// Set an enum storage to newValue according to bool value, then trigger regular SetProperty.
        /// </summary>
        protected bool SetEnumProperty<TEnum>(ref TEnum storage, TEnum newValue, bool? value, Action onChanged, string propertyName)
             where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            EnumServices.TEnumTypeCheck<TEnum>();

            // Initialize with newValue if checked and enum value is None
            if ((value ?? false) && storage.Equals(Activator.CreateInstance<TEnum>())) { return SetProperty(ref storage, newValue, onChanged, propertyName); }

            // Do nothing if unchecked
            return false;
        }

        /// <summary>
        /// Flip a specific flag storage according to bool value, then trigger regular SetProperty.
        /// </summary>
        protected bool SetFlagProperty(ref OtherFlags storage, OtherFlags flag, bool? value, Action onChanged, string propertyName)
        {
            OtherFlags newValue;
            if (value ?? false) { newValue = storage | flag; }
            else { newValue = storage & ~flag; }

            return SetProperty(ref storage, newValue, onChanged, propertyName);
        }

        #endregion
    }
}
