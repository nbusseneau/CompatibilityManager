using CompatibilityManager.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #endregion

        #region Commands
        
        public DelegateCommand ReloadCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand ElevateCommand { get; private set; }

        #endregion

        #region Constructors

        private MainViewModel()
        {
            this.HKCUApplications = new ApplicationListViewModel(Resources.Strings.HKCU, RegistryServices.HKCUKey);
            this.HKLMApplications = RegistryServices.HKLMKey != null ? new ApplicationListViewModel(Resources.Strings.HKLM, RegistryServices.HKLMKey) : null;

            this.ReloadCommand = new DelegateCommand(this.Reload, this.CanReload);
            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);
            this.ElevateCommand = new DelegateCommand(this.Elevate, this.CanElevate);

            this.SubscribeEvents();
        }

        #endregion

        #region Event subscriptions

        private SubscriptionToken selectionChanged;
        
        private void SubscribeEvents()
        {
            this.selectionChanged = SelectionChanged.Instance.Subscribe(this.OnSelectionChanged);
            this.OnSelectionChanged();
        }

        private void UnsubscribeEvents()
        {
            SelectionChanged.Instance.Unsubscribe(this.selectionChanged);
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

        private async void Save(IEnumerable<ApplicationViewModel> applications)
        {
            var warning = MessageBox.Show(Application.Current.MainWindow, Resources.Strings.SaveWarning, Resources.Strings.WarningTitle, MessageBoxButton.OKCancel);
            if (warning != MessageBoxResult.OK) { return; }

            this.IsWaiting = true;

            var applicationsRemoved = await System.Threading.Tasks.Task.Run(() =>
            {
                var taskApplicationsCopy = new List<ApplicationViewModel>(applications); // Copy required for iteration since we might remove items
                var taskApplicationsRemoved = new List<ApplicationViewModel>();

                foreach (var application in taskApplicationsCopy)
                {
                    var appCompatFlags = application.Settings.ToRegistryString();
                    if (string.IsNullOrWhiteSpace(appCompatFlags))
                    {
                        try { application.RegistryKey.DeleteValue(application.Path); }
                        catch (ArgumentException) { } // Trying to delete application that does not exist in registry -- was added by user but no settings specified
                        finally { taskApplicationsRemoved.Add(application); }
                    }
                    else
                    {
                        application.RegistryKey.SetValue(application.Path, appCompatFlags);
                        application.ReloadSettings();
                    }
                }

                return taskApplicationsRemoved;
            });

            foreach (var application in applicationsRemoved)
            {
                this.GetApplicationListViewModel(application.RegistryKey).Applications.Remove(application);
            }
            this.ComputeAggregatedSettings();

            this.IsWaiting = false;
        }

        #endregion

        #region Command executes

        private void Reload()
        {
            foreach (var application in this.SelectedApplications) { application.ReloadSettings(); }
            this.ComputeAggregatedSettings();
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
    }
}
