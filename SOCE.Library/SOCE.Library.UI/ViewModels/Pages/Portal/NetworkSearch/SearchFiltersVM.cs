using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Office.Interop.Excel;

namespace SOCE.Library.UI.ViewModels
{
    public class SearchFiltersVM : BaseVM
    {
        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }

        public bool result = false;

        private ObservableCollection<SearchSettingsModel> _settings = new ObservableCollection<SearchSettingsModel>();
        public ObservableCollection<SearchSettingsModel> Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                RaisePropertyChanged(nameof(Settings));
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public ICommand AcceptCommand { get; set; }

        public ICommand AddSetting { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }
        public ICommand DeleteSearchFilterCommand { get; set; }
        public ICommand OpenPathCommand { get; set; }
        
        public SearchFiltersVM(EmployeeModel curremployee)
        {
            CurrentEmployee = curremployee;
            //load current setting
            this.MoveUpCommand = new RelayCommand<SearchSettingsModel>(this.MoveUp);
            this.MoveDownCommand = new RelayCommand<SearchSettingsModel>(this.MoveDown);
            this.DeleteSearchFilterCommand = new RelayCommand<SearchSettingsModel>(this.DeleteSearch);
            this.OpenPathCommand = new RelayCommand<SearchSettingsModel>(this.OpenPath);
            this.AddSetting = new RelayCommand(this.SettingAdd);
            this.AcceptCommand = new RelayCommand(this.Save);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            //for testing purposesonly
            List<SearchSettingsModel> settings = new List<SearchSettingsModel>();
            settings.Add(new SearchSettingsModel("Projects", $"P:\\", 1, true));
            settings.Add(new SearchSettingsModel("Drawings", $"N:\\dwg", 1, true));
            settings.Add(new SearchSettingsModel("Architecturals", $"N:\\ARCHITECTURALS", 1, true));
            settings.Add(new SearchSettingsModel("Plots", $"N:\\Plot", 1, true));
            settings.Add(new SearchSettingsModel("Archive", $"R:\\", 1, true));
            Settings = new ObservableCollection<SearchSettingsModel>(settings);
        }

        private void OpenPath(SearchSettingsModel mod)
        {

        }

        private void MoveUp(SearchSettingsModel mod)
        {

        }

        private void MoveDown(SearchSettingsModel mod)
        {

        }

        private void DeleteSearch(SearchSettingsModel mod)
        {

        }

        private void SettingAdd()
        {

        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }

        private void Save()
        {
            //save stuff
            CloseWindow();
        }

    }
}
