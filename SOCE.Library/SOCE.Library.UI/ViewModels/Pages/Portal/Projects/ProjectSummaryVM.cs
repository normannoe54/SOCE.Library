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

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectSummaryVM : BaseVM
    {
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

        private SubProjectModel _selectedProjectPhase;
        public SubProjectModel SelectedProjectPhase
        {
            get { return _selectedProjectPhase; }
            set
            {
                _selectedProjectPhase = value;
                RaisePropertyChanged("SelectedProjectPhase");
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

        public ICommand AddSubCommand { get; set; }
        public ICommand CloseCommand { get; set; }


        public ProjectSummaryVM(ProjectModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;

            if (employee.Status != AuthEnum.Standard)
            {

            }
            //Roles.CollectionChanged += CollectionChanged;
            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            pm.FormatData(true);
            SubProjects = pm.SubProjects;
        }

        private void AddSubProject()
        {

        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
