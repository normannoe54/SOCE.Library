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

namespace SOCE.Library.UI.ViewModels
{
    public class AddServiceVM : BaseVM
    {

        private UserControl _leftViewToShow = new UserControl();
        public UserControl LeftViewToShow
        {
            get { return _leftViewToShow; }
            set
            {
                _leftViewToShow = value;
                RaisePropertyChanged(nameof(LeftViewToShow));
            }
        }

        private object _itemToDelete;
        public object ItemToDelete
        {
            get { return _itemToDelete; }
            set
            {
                _itemToDelete = value;
                RaisePropertyChanged(nameof(ItemToDelete));
            }
        }

        public bool result = false;

        private ObservableCollection<SubProjectModel> _subProjects = new ObservableCollection<SubProjectModel>();
        public ObservableCollection<SubProjectModel> SubProjects
        {
            get { return _subProjects; }
            set
            {
                _subProjects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private SubProjectModel _selectedAddService;
        public SubProjectModel SelectedAddService
        {
            get { return _selectedAddService; }
            set
            {
                if ((_selectedAddService != null || !string.IsNullOrEmpty(_selectedAddService?.PointNumber)))
                {

                    if (!_selectedAddService.EditSubFieldState)
                    {
                        _selectedAddService.EditSubFieldState = true;
                    }
                }

                _selectedAddService = value;
                //Employees = BaseEmployees;
                ////set stuff
                //foreach (RolePerSubProjectModel role in _selectedProjectPhase.RolesPerSub)
                //{
                //    Employees.Remove(role.Employee);
                //}
                RaisePropertyChanged("SelectedAddService");
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

        private bool _leftDrawerOpen = false;
        public bool LeftDrawerOpen
        {
            get { return _leftDrawerOpen; }
            set
            {
                _leftDrawerOpen = value;
                RaisePropertyChanged("LeftDrawerOpen");
            }
        }

        private bool _canAddPhase;
        public bool CanAddPhase
        {
            get { return _canAddPhase; }
            set
            {
                _canAddPhase = value;
                RaisePropertyChanged("CanAddPhase");
            }
        }

        private bool _canEditPhase;
        public bool CanEditPhase
        {
            get { return _canEditPhase; }
            set
            {
                _canEditPhase = value;
                RaisePropertyChanged("CanEditPhase");
            }
        }

        private ProjectModel _baseProject;
        public ProjectModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged("BaseProject");
            }
        }

        private bool _activeProject;
        public bool ActiveProject
        {
            get { return _activeProject; }
            set
            {
                _activeProject = value;

                if (!_activeProject)
                {
                    CanAddPhase = false;
                }
                RaisePropertyChanged("ActiveProject");
            }
        }

        public ICommand AddSubCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand DeleteSubProject { get; set; }
        public ICommand ExportDataCommand { get; set; }

        public ICommand OpenAdCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        //public ICommand AdserviceCommand { get; set; }
        //public ICommand SynchAdserviceFileCommand { get; set; }
        //public ICommand RemoveAdserviceCommand { get; set; }

        public AddServiceVM(ProjectModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;
            BaseProject = pm;

            //Roles.CollectionChanged += CollectionChanged;
            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.DeleteSubProject = new RelayCommand<SubProjectModel>(this.DeleteSub);
            this.ExportDataCommand = new RelayCommand(this.RunExport);

            //this.AdserviceCommand = new RelayCommand(this.RunAdserviceCommand);

            ActiveProject = pm.IsActive;
           
            pm.FormatData(true);

            List<SubProjectModel> subs = new List<SubProjectModel>();
            foreach (SubProjectModel sub in pm.SubProjects)
            {
                if (sub.IsAddService)
                {
                    subs.Add(sub);
                }
            }


            SubProjects = new ObservableCollection<SubProjectModel>(subs);

            if (SubProjects.Count > 0)
            {
                SelectedAddService = SubProjects[0];
                SubProjects = SubProjects.Renumber(true);
            }
        }

        private async void RunExport()
        {
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();

            aysvm.Message = $"Are you sure you want to export";
            aysvm.SubMessage = $"[{BaseProject.ProjectNumber}] {BaseProject.ProjectName}";
            view.DataContext = aysvm;

            //show the dialog
            var Result = await DialogHost.Show(view, "RootDialog");

            YesNoVM vm = view.DataContext as YesNoVM;
            bool resultvm = vm.Result;

            if (resultvm)
            {
                ExportConfirmView ecv = new ExportConfirmView();
                ExportConfirmVM ecvm = new ExportConfirmVM(new List<ProjectModel> { BaseProject });
                //show progress bar and do stuff
                ecv.DataContext = ecvm;
                var newres = await DialogHost.Show(ecv, "RootDialog");
            }
        }


        private void DeleteSub(SubProjectModel spm)
        {
            if (SubProjects.Count > 1)
            {
                LeftViewToShow = new YesNoView();
                YesNoVM aysvm = new YesNoVM(spm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = spm;
                LeftDrawerOpen = true;
            }
        }

        private void AddSubProject()
        {
            LeftViewToShow = new AddAddServiceView();
            AddAddServiceVM addsubvm = new AddAddServiceVM(BaseProject, this);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
