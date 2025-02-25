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

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public ICommand AcceptCommand { get; set; }

        public ICommand AddSetting { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }
        public ICommand DeleteSearchFilterCommand { get; set; }
        public ICommand OpenPathCommand { get; set; }

        private NetworkSearchVM BaseVM;

        public SearchFiltersVM(EmployeeModel curremployee, NetworkSearchVM basevm)
        {
            BaseVM = basevm;
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
            LoadSearchFilters();
            //List<SearchSettingsModel> settings = new List<SearchSettingsModel>();
            //settings.Add(new SearchSettingsModel("Projects", $"P:\\", 1, true));
            //settings.Add(new SearchSettingsModel("Drawings", $"N:\\dwg", 1, true));
            //settings.Add(new SearchSettingsModel("Architecturals", $"N:\\ARCHITECTURALS", 1, true));
            //settings.Add(new SearchSettingsModel("Plots", $"N:\\Plot", 1, true));
            //settings.Add(new SearchSettingsModel("Archive", $"R:\\", 1, true));
            //Settings = new ObservableCollection<SearchSettingsModel>(settings);
        }

        private void OpenPath(SearchSettingsModel mod)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                mod.FolderPath = dialog.FileName;
            }
        }

        private void MoveUp(SearchSettingsModel mod)
        {
            int id = Settings.IndexOf(mod);

            if (id != 0 && id != -1)
            {
                Settings.Move(id, id - 1);
                Reorder();
            }

        }

        private void MoveDown(SearchSettingsModel mod)
        {
            int id = Settings.IndexOf(mod);

            if (id != -1 && id != Settings.Count - 1)
            {
                Settings.Move(id, id + 1);
                Reorder();
            }
        }

        private void LoadSearchFilters()
        {
            List<SearchFilterDbModel> filters = SQLAccess.LoadSearchFilterByEmployeeId(CurrentEmployee.Id);
            List<SearchSettingsModel> ssms = new List<SearchSettingsModel>();

            foreach (SearchFilterDbModel filter in filters)
            {
                SearchSettingsModel ssm = new SearchSettingsModel(filter);
                ssms.Add(ssm);
            }
            List<SearchSettingsModel> ssmsordered = ssms.OrderBy(x => x.NumberOrder).ToList();
            Settings = new ObservableCollection<SearchSettingsModel>(ssmsordered);
        }

        private void DeleteSearch(SearchSettingsModel mod)
        {
            try
            {
                Settings.Remove(mod);
            }
            catch
            {

            }
        }

        private void SettingAdd()
        {
            Settings.Add(new SearchSettingsModel());
            Reorder();
        }

        private void Reorder()
        {
            for (int i = 0; i < Settings.Count; i++)
            {
                SearchSettingsModel mod = Settings[i];
                mod.NumberOrder = i;
            }
        }

        private void CloseWindow()
        {
            BaseVM.LoadFilters();
            DialogHost.Close("RootDialog");
        }

        private void Save()
        {
            //Reorder();
            List<SearchSettingsModel> itemstoremove = new List<SearchSettingsModel>();

            foreach (SearchSettingsModel ssm in Settings)
            {
                if (String.IsNullOrEmpty(ssm.Header) || String.IsNullOrEmpty(ssm.FolderPath))
                {
                    itemstoremove.Add(ssm);
                }
            }

            foreach (SearchSettingsModel ssm in itemstoremove)
            {
                Settings.Remove(ssm);
            }

            Reorder();

            foreach (SearchSettingsModel ssm in Settings)
            {
                int activeint = Convert.ToInt32(ssm.Active);
                int searchfiletype = Convert.ToInt32(ssm.SearchFileType);

                SearchFilterDbModel sfdb = new SearchFilterDbModel()
                {
                    EmployeeId = CurrentEmployee.Id,
                    Header = ssm.Header,
                    FolderPath = ssm.FolderPath,
                    NumberOrder = ssm.NumberOrder,
                    Active = activeint,
                    SearchFileType = searchfiletype,
                    SubLayer = ssm.SubLayer
                };

                if (ssm.Id != 0)
                {
                    try
                    {
                        sfdb.Id = ssm.Id;
                        SQLAccess.UpdateSearchFilter(sfdb);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    int id = SQLAccess.AddSearchFilter(sfdb);
                    ssm.Id = id;
                }


            }

            List<SearchFilterDbModel> allfilters = SQLAccess.LoadSearchFilterByEmployeeId(CurrentEmployee.Id);

            foreach (SearchFilterDbModel sfcheck in allfilters)
            {
                if (!Settings.Any(x => x.Id == sfcheck.Id))
                {
                    SQLAccess.DeleteSearchFilter(sfcheck.Id);
                }
            }

            Message = "Filters Saved";
            //save stuff
            //CloseWindow();
        }

    }
}
