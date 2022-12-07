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

        private ObservableCollection<EmployeeModel> _employees = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
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

        public ICommand AddSubCommand { get; set; }

        public ICommand AddRoleCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand DeleteSubProject { get; set; }
        public ICommand DeleteRole { get; set; }
        public ProjectSummaryVM(ProjectModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;
            BaseProject = pm;
            //Roles.CollectionChanged += CollectionChanged;
            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            this.AddRoleCommand = new RelayCommand(this.AddRole);

            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.DeleteSubProject = new RelayCommand<SubProjectModel>(this.DeleteSub);
            this.DeleteRole = new RelayCommand<RolePerSubProjectModel>(this.DeleteRoleIfPossible);

            pm.FormatData(true);
            SubProjects = pm.SubProjects;

            if (SubProjects.Count >0)
            {
                SelectedProjectPhase = SubProjects[0];
            }

            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();
            ObservableCollection<EmployeeModel> totalemployees = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employeenew));
            }

            //OverallFee = overallfee;
            Employees = totalemployees;
        }

        private void DeleteRoleIfPossible(RolePerSubProjectModel rpsm)
        {
            if (rpsm.SpentHours == 0)
            {
                SQLAccess.DeleteRolesPerSubProject(rpsm.Id);
            }
            
        }

        private void DeleteSub(SubProjectModel spm)
        {
            //need a popup warning

            foreach(RolePerSubProjectModel rpspm in spm.RolesPerSub)
            {
                SQLAccess.DeleteRolesPerSubProject(rpspm.Id);
            }
            SQLAccess.ArchiveSubProject(spm.Id);
            SubProjects.Remove(spm);
        }

        private void AddRole()
        {
            RolePerSubProjectModel rpspm = new RolePerSubProjectModel(SelectedProjectPhase, SelectedProjectPhase.Fee);
            rpspm.EditRoleFieldState = false;
            rpspm.SpentHours = 0;
            SelectedProjectPhase.RolesPerSub.Add(rpspm);
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
