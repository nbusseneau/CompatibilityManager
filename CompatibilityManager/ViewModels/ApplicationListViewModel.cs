using CompatibilityManager.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CompatibilityManager.ViewModels
{
    public class SelectionChanged : PubSubEvent
    {
        public static SelectionChanged Instance { get; } = MainViewModel.EventAggregator.GetEvent<SelectionChanged>();
    }

    public class ApplicationListViewModel : BindableBase
    {
        #region Properties

        public RegistryKey RegistryKey { get; private set; }
        public ObservableRangeCollection<ApplicationViewModel> Applications { get; private set; }
        public string Title { get; private set; }

        #endregion

        #region Commands

        public DelegateCommand AddFolderCommand { get; private set; }
        public DelegateCommand AddFilesCommand { get; private set; }
        public DelegateCommand SelectAllCommand { get; private set; }
        public DelegateCommand UnselectAllCommand { get; private set; }

        #endregion

        #region Constructors

        public ApplicationListViewModel(string title, RegistryKey registryKey = null)
        {
            this.Title = title;
            this.RegistryKey = registryKey ?? RegistryServices.HKCUKey;
            this.Applications = new ObservableRangeCollection<ApplicationViewModel>(RegistryServices.GetApplications(this.RegistryKey));

            this.AddFolderCommand = new DelegateCommand(this.AddFolder);
            this.AddFilesCommand = new DelegateCommand(this.AddFiles);
            this.SelectAllCommand = new DelegateCommand(this.SelectAll);
            this.UnselectAllCommand = new DelegateCommand(this.UnselectAll);

            this.SubscribeEvents();
        }

        #endregion

        #region Event subscriptions

        private SubscriptionToken isSelectedChanged;

        private void SubscribeEvents()
        {
            this.isSelectedChanged = IsSelectedChanged.Instance.Subscribe(() => this.OnIsSelectedChanged());
            this.OnIsSelectedChanged();
        }

        private void UnsubscribeEvents()
        {
            IsSelectedChanged.Instance.Unsubscribe(this.isSelectedChanged);
        }

        #endregion

        #region Event callbacks

        private void OnIsSelectedChanged()
        {
            SelectionChanged.Instance.Publish();
        }

        #endregion

        #region Command executes

        private void AddFolder()
        {
            var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = Resources.Strings.AddFolderDescription,
            };

            var win32Window = new System.Windows.Forms.NativeWindow();
            win32Window.AssignHandle(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle);

            if (openFolderDialog.ShowDialog(win32Window) == System.Windows.Forms.DialogResult.OK)
            {
                this.UnsubscribeEvents();

                var paths = System.IO.SafeDirectory.SafeEnumerateFiles(openFolderDialog.SelectedPath, "*.exe", System.IO.SearchOption.AllDirectories);
                paths = paths.Where(path => !this.Applications.Any(application => application.Path.Equals(path)));
                this.Applications.AddRange(paths.Select(path => new ApplicationViewModel(path, this.RegistryKey, isSelected: true)));

                this.SubscribeEvents();
            }
        }

        private void AddFiles()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Applications (.exe)|*.exe",
                Multiselect = true,
                CheckPathExists = true,
                Title = Resources.Strings.AddFilesDescription,
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                this.UnsubscribeEvents();

                var paths = openFileDialog.FileNames.Where(path => !this.Applications.Any(application => application.Path.Equals(path)));
                foreach (var path in paths)
                {
                    if (!this.Applications.Any(application => application.Path.Equals(path)))
                    {
                        this.Applications.Add(new ApplicationViewModel(path, this.RegistryKey, isSelected: true));
                    }
                }

                this.SubscribeEvents();
            }
        }

        private void SelectAll()
        {
            this.UnsubscribeEvents();
            foreach (var application in this.Applications) { application.IsSelected = true; }
            this.SubscribeEvents();
        }

        private void UnselectAll()
        {
            this.UnsubscribeEvents();
            foreach (var application in this.Applications) { application.IsSelected = false; }
            this.SubscribeEvents();
        }

        #endregion
    }
}
