using CompatibilityManager.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CompatibilityManager.ViewModels
{
    public class AggregatedSettingsViewModel : SettingsViewModel
    {
        #region Settings collection

        private IEnumerable<SettingsViewModel> settings;

        #endregion

        #region Properties

        public override string Title => Resources.Strings.AggregatedSettings;

        #endregion

        #region Constructors

        public AggregatedSettingsViewModel(IEnumerable<SettingsViewModel> settings) : base()
        {
            // Initialize settings collection
            this.settings = settings;
            if (!settings.Any()) { return; } // Stop immediately if collection is empty

            // Initialize aggregated compatibility settings
            this.compatibilityMode = this.Aggregate(settings.Select(s => s.CompatibilityMode));
            this.colorMode = this.Aggregate(settings.Select(s => s.ColorMode));
            this.dpiScaling = this.Aggregate(settings.Select(s => s.DPIScaling));
            this.otherFlags = this.AggregateFlags(settings.Select(s => s.OtherFlags));
            this.additionalFlags = null;

            // Initialize checkboxes
            this.compatibilityModeChecked = this.Aggregate(settings.Select(s => s.CompatibilityModeChecked));
            if ((this.compatibilityModeChecked ?? false) && this.compatibilityMode == CompatibilityMode.None) { this.compatibilityModeChecked = null; }

            this.colorModeChecked = this.Aggregate(settings.Select(s => s.ColorModeChecked));
            if ((this.colorModeChecked ?? false) && this.colorMode == ColorMode.None) { this.colorModeChecked = null; }

            this.dpiScalingChecked = this.Aggregate(settings.Select(s => s.DPIScalingChecked));
            if ((this.dpiScalingChecked ?? false) && this.dpiScaling == DPIScaling.None) { this.dpiScalingChecked = null; }

            this.resolution640x480Checked = this.Aggregate(settings.Select(s => s.Resolution640x480Checked));
            if ((this.resolution640x480Checked ?? false) && !this.otherFlags.HasFlag(OtherFlags.RESOLUTION640X480)) { this.resolution640x480Checked = null; }

            this.disableFullscreenOptimizationsChecked = this.Aggregate(settings.Select(s => s.DisableFullscreenOptimizationsChecked));
            if ((this.disableFullscreenOptimizationsChecked ?? false) && !this.otherFlags.HasFlag(OtherFlags.DISABLEDXMAXIMIZEDWINDOWEDMODE)) { this.disableFullscreenOptimizationsChecked = null; }

            this.runAsAdministratorChecked = this.Aggregate(settings.Select(s => s.RunAsAdministratorChecked));
            if ((this.runAsAdministratorChecked ?? false) && !this.otherFlags.HasFlag(OtherFlags.RUNASADMIN)) { this.runAsAdministratorChecked = null; }

            // Subscribe events
            this.SubscribeEvents();
        }

        #endregion

        #region Helper methods

        private T Aggregate<T>(IEnumerable<T> collection)
        {
            var firstValue = collection.First();
            if (collection.All(b => b.Equals(firstValue))) { return firstValue; }
            return default(T);
        }

        private OtherFlags AggregateFlags(IEnumerable<OtherFlags> collection)
        {
            var firstValue = collection.First();
            var aggregatedValue = OtherFlags.None;
            foreach (var flag in Enum.GetValues(typeof(OtherFlags)).Cast<OtherFlags>())
            {
                if (firstValue.HasFlag(flag) && collection.All(f => f.HasFlag(flag))) { aggregatedValue |= flag; }
            }
            return aggregatedValue;
        }

        #endregion

        #region Command executes

        protected override void Clear()
        {
            this.CompatibilityModeChecked = false;
            this.ColorModeChecked = false;
            this.DPIScalingChecked = false;
            this.Resolution640x480Checked = false;
            this.DisableFullscreenOptimizationsChecked = false;
            this.RunAsAdministratorChecked = false;

            foreach (var setting in this.settings)
            {
                setting.AdditionalFlags = new ObservableRangeCollection<AdditionalFlagViewModel>();
            }
        }

        #endregion

        #region SetProperty overrides

        /// <summary>
        /// Set property to value on children settings.
        /// </summary>
        private void SetChildrenProperty<T>(string propertyName, T value)
        {
            var propertyInfo = typeof(SettingsViewModel).GetProperty(propertyName);
            foreach (var setting in this.settings) { propertyInfo.SetValue(setting, value); }
        }
        
        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            SetChildrenProperty(propertyName, value);
            return base.SetProperty(ref storage, value, propertyName);
        }

        protected override bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            SetChildrenProperty(propertyName, value);
            return base.SetProperty(ref storage, value, onChanged, propertyName);
        }

        #endregion
    }
}
