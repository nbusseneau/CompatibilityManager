using CompatibilityManager.Services;
using CompatibilityManager.Resources;
using Prism.Mvvm;
using Prism.Events;
using Microsoft.Win32;

namespace CompatibilityManager.ViewModels
{
    public class IsSelectedChanged : PubSubEvent
    {
        public static IsSelectedChanged Instance { get; } = MainViewModel.EventAggregator.GetEvent<IsSelectedChanged>();
    }

    public class ApplicationViewModel : BindableBase
    {
        #region Properties

        public RegistryKey RegistryKey { get; private set; }

        private SettingsViewModel settings;
        public SettingsViewModel Settings
        {
            get => this.settings;
            set => SetProperty(ref this.settings, value);
        }

        private string path;
        public string Path
        {
            get => this.path;
            set => SetProperty(ref this.path, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => this.isSelected;
            set => SetProperty(ref this.isSelected, value, () => IsSelectedChanged.Instance.Publish());
        }

        #endregion

        #region Constructors

        public ApplicationViewModel(string path, RegistryKey registryKey = null, bool isSelected = false)
        {
            this.Path = path;
            this.RegistryKey = registryKey ?? RegistryServices.HKCUKey;
            this.IsSelected = isSelected;
            
            this.Settings = new SettingsViewModel(RegistryServices.GetApplicationRegistryString(this.RegistryKey, this.Path));
        }

        #endregion

        #region Helper methods

        public void ReloadSettings()
        {
            this.Settings.ReloadFromRegistryString(RegistryServices.GetApplicationRegistryString(this.RegistryKey, this.Path));
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return this.Path;
        }

        #endregion
    }
}
