using Prism.Mvvm;

namespace CompatibilityManager.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public CompatibilityViewModel CompatibilityViewModel { get; private set; }
        public PathsViewModel PathsViewModel { get; private set; }

        public MainViewModel()
        {
            this.CompatibilityViewModel = new CompatibilityViewModel(this);
            this.PathsViewModel = new PathsViewModel(this);
        }
    }
}
