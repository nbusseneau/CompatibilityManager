using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace CompatibilityManager.ViewModels
{
    public class PathsViewModel : BindableBase
    {
        #region AppCompatFlags registry key

        private static readonly string AppCompatFlags = @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";

        #endregion

        #region Parent ViewModel

        private MainViewModel parent;

        #endregion

        #region Properties

        public ObservableRangeCollection<string> Executables { get; set; }

        #endregion

        #region Commands

        public DelegateCommand AddFolderCommand { get; set; }
        public DelegateCommand AddFilesCommand { get; set; }
        public DelegateCommand ResetCommand { get; set; }
        public DelegateCommand ApplyCommand { get; set; }

        #endregion

        #region Constructor

        public PathsViewModel(MainViewModel parent)
        {
            this.parent = parent;

            this.Executables = new ObservableRangeCollection<string>();
            this.Executables.CollectionChanged += this.Executables_CollectionChanged;

            this.AddFolderCommand = new DelegateCommand(this.AddFolder);
            this.AddFilesCommand = new DelegateCommand(this.AddFiles);
            this.ResetCommand = new DelegateCommand(this.Reset);
            this.ApplyCommand = new DelegateCommand(this.Apply, this.CanApply);
        }

        #endregion

        #region Executables CollectionChanged

        private void Executables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ApplyCommand.RaiseCanExecuteChanged();
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
                var paths = System.IO.Directory.EnumerateFiles(openFolderDialog.SelectedPath, "*.exe", System.IO.SearchOption.AllDirectories);
                this.Executables.AddRange(paths);
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
                foreach (var filename in openFileDialog.FileNames) { this.Executables.Add(filename); }
            }
        }

        private void Reset()
        {
            this.Executables.Clear();
        }

        private void Apply()
        {
        }

        private bool CanApply()
        {
            return this.Executables.Any();
        }

        #endregion
    }
}
