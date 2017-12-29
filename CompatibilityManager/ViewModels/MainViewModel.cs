using Prism.Mvvm;

namespace CompatibilityManager.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public CompatibilitySettingsViewModel CompatibilitySettingsViewModel { get; private set; }
        public PathsViewModel PathsViewModel { get; private set; }

        public MainViewModel()
        {
            this.CompatibilitySettingsViewModel = new CompatibilitySettingsViewModel(this);
            this.PathsViewModel = new PathsViewModel(this);
        }
    }
}
