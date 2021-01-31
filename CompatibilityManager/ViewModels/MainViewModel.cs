using CompatibilityManager.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace CompatibilityManager.ViewModels
{
    public sealed class MainViewModel : BindableBase
    {
        #region Singleton

        private static MainViewModel instance;
        public static MainViewModel Instance => instance ?? (instance = new MainViewModel());

        #endregion

        #region Event aggregator

        public static EventAggregator EventAggregator { get; } = new EventAggregator();

        #endregion

        #region Properties

        private bool isWaiting;
        public bool IsWaiting
        {
            get => this.isWaiting;
            set => SetProperty(ref this.isWaiting, value);
        }

        public ApplicationListViewModel HKCUApplications { get; private set; }
        public ApplicationListViewModel HKLMApplications { get; private set; }

        public IEnumerable<ApplicationViewModel> AllApplications
        {
            get
            {
                if (this.HKLMApplications != null) { return this.HKCUApplications.Applications.Concat(this.HKLMApplications.Applications); }
                else { return this.HKCUApplications.Applications; }
            }
        }
        public IEnumerable<ApplicationViewModel> SelectedApplications => this.AllApplications.Where(application => application.IsSelected);

        public AggregatedSettingsViewModel AggregatedSettings { get; private set; }
        public SettingsViewModel DisplayedSettings
        {
            get
            {
                if (this.SelectedApplications.Count() == 1) { return this.SelectedApplications.Single().Settings; }
                return this.AggregatedSettings;
            }
        }

        public bool IsAnySelected => this.SelectedApplications.Any();

        private const int HWND_BROADCAST = 0xffff;
        private const uint WM_SETTINGCHANGE = 0x001a;

        #endregion

        #region Commands
        
        public DelegateCommand ReloadCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand ElevateCommand { get; private set; }

        #endregion

        #region Constructors

        private MainViewModel()
        {
            this.HKCUApplications = new ApplicationListViewModel(isHKLM: false);
            this.HKLMApplications = RegistryServices.HKLMKey != null ? new ApplicationListViewModel(isHKLM: true) : null;

            this.ReloadCommand = new DelegateCommand(this.Reload, this.CanReload);
            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);

            this.ElevateCommand = new DelegateCommand(this.Elevate, this.CanElevate);

            this.SubscribeEvents();
        }

        #endregion

        #region Event subscriptions

        private SubscriptionToken selectionChanged;
        private SubscriptionToken isWaitingDisplayed;

        private void SubscribeEvents()
        {
            this.selectionChanged = SelectionChanged.Instance.Subscribe(this.OnSelectionChanged);
            this.isWaitingDisplayed = IsWaitingDisplayed.Instance.Subscribe(this.OnIsWaitingDisplayed);
            this.OnSelectionChanged();
        }

        #endregion

        #region Event callbacks

        private void OnSelectionChanged()
        {
            this.ComputeAggregatedSettings();

            RaisePropertyChanged(nameof(this.IsAnySelected));
            this.ReloadCommand.RaiseCanExecuteChanged();
            this.SaveCommand.RaiseCanExecuteChanged();
        }

        private void OnIsWaitingDisplayed(bool value)
        {
            this.IsWaiting = value;
        }

        #endregion

        #region Helper methods

        private void ComputeAggregatedSettings()
        {
            var settings = this.SelectedApplications.Select(application => application.Settings);
            this.AggregatedSettings = new AggregatedSettingsViewModel(settings);
            RaisePropertyChanged(nameof(this.DisplayedSettings));
        }

        private ApplicationListViewModel GetApplicationListViewModel(RegistryKey registryKey)
        {
            if (registryKey.Equals(RegistryServices.HKCUKey)) { return this.HKCUApplications; }
            else { return this.HKLMApplications; }
        }

        private async void Reload(IEnumerable<ApplicationViewModel> applications)
        {
            this.IsWaiting = true;

            await Task.Run(() =>
            {
                foreach (var application in this.SelectedApplications) { application.ReloadSettings(); }
            });
            this.ComputeAggregatedSettings();

            this.IsWaiting = false;
        }

        private async void Save(IEnumerable<ApplicationViewModel> applications)
        {
            var warning = MessageBox.Show(Application.Current.MainWindow, Resources.Strings.SaveWarning, Resources.Strings.WarningTitle, MessageBoxButton.OKCancel);
            if (warning != MessageBoxResult.OK) { return; }

            this.IsWaiting = true;

            var removedApplications = await Task.Run(() =>
            {
                var taskApplicationsRemoved = new List<ApplicationViewModel>();
                var taskApplicationsModified = new List<ApplicationViewModel>();

                foreach (var application in applications)
                {
                    var appCompatFlags = application.Settings.ToRegistryString();
                    var path = application.Path == Resources.Strings.GlobalConfig ? RegistryServices.GlobalCompatEnvVarRegistryName : application.Path;

                    if (string.IsNullOrWhiteSpace(appCompatFlags))
                    {
                        try { application.RegistryKey.DeleteValue(path); }
                        catch (ArgumentException) { } // Trying to delete application that does not exist in registry -- was added by user but no settings specified
                        finally
                        {
                            if (application.Path != Resources.Strings.GlobalConfig)
                            {
                                taskApplicationsRemoved.Add(application);
                            }
                        }
                    }
                    else
                    {
                        application.RegistryKey.SetValue(path, appCompatFlags);
                        taskApplicationsModified.Add(application);
                    }

                    if (application.Path == Resources.Strings.GlobalConfig)
                    {
                        SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, (UIntPtr)0, "Environment"); // Send message for applications to update the environment variable -- this makes changes take effect without restarting Explorer.
                    }
                }
                
                foreach (var application in taskApplicationsModified)
                {
                    application.ReloadSettings();
                }

                return taskApplicationsRemoved;
            });

            foreach (var application in removedApplications)
            {
                this.GetApplicationListViewModel(application.RegistryKey).Applications.Remove(application);
            }
            this.OnSelectionChanged();

            this.IsWaiting = false;
        }

        #endregion

        #region Command executes

        private void Reload()
        {
            this.Reload(this.SelectedApplications);
        }

        private bool CanReload()
        {
            return this.IsAnySelected;
        }

        private void Save()
        {
            this.Save(this.SelectedApplications);
        }

        private bool CanSave()
        {
            return this.IsAnySelected;
        }

        private void Elevate()
        {
            PrivilegesServices.Elevate();
        }

        private bool CanElevate()
        {
            return !PrivilegesServices.IsRunAsAdmin;
        }

        #endregion

        #region DLL Imports

        [DllImport("user32.dll")]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);

        #endregion
    }
}
