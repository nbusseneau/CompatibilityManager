using CompatibilityManager.Enums;
using CompatibilityManager.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CompatibilityManager.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        protected EventAggregator eventAggregator;

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
            && this.OtherFlags == OtherFlags.None
            && !this.AdditionalFlags.Any();

        public virtual string Title => Resources.Strings.ApplicationSettings;

        #endregion

        #region Settings
        
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

        protected ObservableRangeCollection<AdditionalFlagViewModel> additionalFlags = new ObservableRangeCollection<AdditionalFlagViewModel>();
        public ObservableRangeCollection<AdditionalFlagViewModel> AdditionalFlags
        {
            get => this.additionalFlags;
            set => SetProperty(ref this.additionalFlags, value, this.OnSettingsChanged);
        }

        #endregion

        #region Checkboxes

        protected bool? compatibilityModeChecked = false;
        public bool? CompatibilityModeChecked
        {
            get => this.compatibilityModeChecked;
            set
            {
                SetEnumProperty(ref this.compatibilityMode, Properties.Settings.Default.LastCompatibilityMode, value, nameof(this.CompatibilityMode));
                SetProperty(ref this.compatibilityModeChecked, value, this.OnSettingsChanged);
            }
        }

        protected bool? colorModeChecked = false;
        public bool? ColorModeChecked
        {
            get => this.colorModeChecked;
            set
            {
                SetEnumProperty(ref this.colorMode, Properties.Settings.Default.LastColorMode, value, nameof(this.ColorMode));
                SetProperty(ref this.colorModeChecked, value, this.OnSettingsChanged);
            }
        }

        protected bool? dpiScalingChecked = false;
        public bool? DPIScalingChecked
        {
            get => this.dpiScalingChecked;
            set
            {
                SetEnumProperty(ref this.dpiScaling, Properties.Settings.Default.LastDPIScaling, value, nameof(this.DPIScaling));
                SetProperty(ref this.dpiScalingChecked, value, this.OnSettingsChanged);
            }
        }

        protected bool? resolution640x480Checked = false;
        public bool? Resolution640x480Checked
        {
            get => this.resolution640x480Checked;
            set
            {
                SetFlagProperty(ref this.otherFlags, OtherFlags.RESOLUTION640X480, value, nameof(this.OtherFlags));
                SetProperty(ref this.resolution640x480Checked, value, this.OnSettingsChanged);
            }
        }

        protected bool? disableFullscreenOptimizationsChecked = false;
        public bool? DisableFullscreenOptimizationsChecked
        {
            get => this.disableFullscreenOptimizationsChecked;
            set
            {
                SetFlagProperty(ref this.otherFlags, OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE, value, nameof(this.OtherFlags));
                SetProperty(ref this.disableFullscreenOptimizationsChecked, value, this.OnSettingsChanged);
            }
        }

        protected bool? runAsAdministratorChecked = false;
        public bool? RunAsAdministratorChecked
        {
            get => this.runAsAdministratorChecked;
            set
            {
                SetFlagProperty(ref this.otherFlags, OtherFlags.RUNASADMIN, value, nameof(this.OtherFlags));
                SetProperty(ref this.runAsAdministratorChecked, value, this.OnSettingsChanged);
            }
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand ClearCommand { get; protected set; }
        public DelegateCommand AddCommand { get; protected set; }

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            this.ClearCommand = new DelegateCommand(this.Clear);
            this.AddCommand = new DelegateCommand(this.AddFlag);
        }

        public SettingsViewModel(string registryString) : this()
        {
            if (string.IsNullOrWhiteSpace(registryString)) { return; } // Safeguard

            // Initialize event aggregator
            this.eventAggregator = new EventAggregator();

            // Initialize settings
            var tuple = RegistryServices.FromRegistryString(registryString);
            this.compatibilityMode = tuple.Item1;
            this.colorMode = tuple.Item2;
            this.dpiScaling = tuple.Item3;
            this.otherFlags = tuple.Item4;
            this.additionalFlags = new ObservableRangeCollection<AdditionalFlagViewModel>(tuple.Item5.Select(flag => new AdditionalFlagViewModel(this.eventAggregator, flag)));

            // Initialize checkboxes
            this.compatibilityModeChecked = this.compatibilityMode != CompatibilityMode.None;
            this.colorModeChecked = this.colorMode != ColorMode.None;
            this.dpiScalingChecked = this.dpiScaling != DPIScaling.None;
            this.resolution640x480Checked = this.otherFlags.HasFlag(OtherFlags.RESOLUTION640X480);
            this.disableFullscreenOptimizationsChecked = this.otherFlags.HasFlag(OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE);
            this.runAsAdministratorChecked = this.otherFlags.HasFlag(OtherFlags.RUNASADMIN);

            // Subscribe events
            this.SubscribeEvents();
        }

        #endregion

        #region Event subscriptions

        private SubscriptionToken flagChanged;
        private SubscriptionToken removeCommandIssued;

        private void SubscribeEvents()
        {
            this.flagChanged = this.eventAggregator.GetEvent<FlagChanged>().Subscribe(this.OnSettingsChanged);
            this.removeCommandIssued = this.eventAggregator.GetEvent<RemoveCommandIssued>().Subscribe(this.RemoveFlag);
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

        public void ReloadFromRegistryString(string registryString)
        {
            var settings = new SettingsViewModel(registryString);

            // Reload settings
            this.CompatibilityMode = settings.CompatibilityMode != CompatibilityMode.None ? settings.CompatibilityMode : this.CompatibilityMode;
            this.ColorMode = settings.ColorMode != ColorMode.None ? settings.ColorMode : this.ColorMode;
            this.DPIScaling = settings.DPIScaling != DPIScaling.None ? settings.DPIScaling : this.DPIScaling;
            this.OtherFlags = settings.OtherFlags;
            this.AdditionalFlags = new ObservableRangeCollection<AdditionalFlagViewModel>(settings.AdditionalFlags.Select(flag => new AdditionalFlagViewModel(this.eventAggregator, flag.Flag)));

            // Reload checkboxes
            this.CompatibilityModeChecked = settings.CompatibilityModeChecked;
            this.ColorModeChecked = settings.ColorModeChecked;
            this.DPIScalingChecked = settings.DPIScalingChecked;
            this.Resolution640x480Checked = settings.Resolution640x480Checked;
            this.DisableFullscreenOptimizationsChecked = settings.DisableFullscreenOptimizationsChecked;
            this.RunAsAdministratorChecked = settings.RunAsAdministratorChecked;

            // Reset HasChanged flag
            this.HasChanged = false;
        }

        public string ToRegistryString()
        {
            var additionalFlags = new List<string>(this.AdditionalFlags.Select(flag => flag.Flag));
            return RegistryServices.ToRegistryString(this.CompatibilityMode, this.ColorMode, this.DPIScaling, this.OtherFlags, additionalFlags);
        }

        #endregion

        #region Command executes

        protected virtual void Clear()
        {
            this.CompatibilityModeChecked = false;
            this.ColorModeChecked = false;
            this.DPIScalingChecked = false;
            this.Resolution640x480Checked = false;
            this.DisableFullscreenOptimizationsChecked = false;
            this.RunAsAdministratorChecked = false;
            this.AdditionalFlags = new ObservableRangeCollection<AdditionalFlagViewModel>();
        }

        protected void AddFlag()
        {
            this.AdditionalFlags.Add(new AdditionalFlagViewModel(this.eventAggregator));
            this.OnSettingsChanged();
        }

        protected void RemoveFlag(AdditionalFlagViewModel additionalFlagViewModel)
        {
            this.AdditionalFlags.Remove(additionalFlagViewModel);
            this.OnSettingsChanged();
        }

        #endregion

        #region SetProperty overrides

        /// <summary>
        /// Set an enum storage to newValue according to bool value, then trigger regular SetProperty.
        /// </summary>
        protected bool SetEnumProperty<TEnum>(ref TEnum storage, TEnum newValue, bool? value, string propertyName)
             where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            EnumServices.TEnumTypeCheck<TEnum>();

            // Initialize with newValue if checked and enum value is None
            if ((value ?? false) && storage.Equals(Activator.CreateInstance<TEnum>())) { return SetProperty(ref storage, newValue, propertyName); }

            // Do nothing if unchecked
            return false;
        }

        /// <summary>
        /// Flip a specific flag storage according to bool value, then trigger regular SetProperty.
        /// </summary>
        protected bool SetFlagProperty(ref OtherFlags storage, OtherFlags flag, bool? value, string propertyName)
        {
            OtherFlags newValue;
            if (value ?? false) { newValue = storage | flag; }
            else { newValue = storage & ~flag; }

            return SetProperty(ref storage, newValue, propertyName);
        }

        #endregion
    }
}
