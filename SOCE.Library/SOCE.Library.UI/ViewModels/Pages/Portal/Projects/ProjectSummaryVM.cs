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

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectSummaryVM : BaseVM
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

        public ObservableCollection<EmployeeModel> BaseEmployees = new ObservableCollection<EmployeeModel>();

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

                //Employees = BaseEmployees;
                ////set stuff
                //foreach (RolePerSubProjectModel role in _selectedProjectPhase.RolesPerSub)
                //{
                //    Employees.Remove(role.Employee);
                //}
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

        private bool _leftDrawerOpen =false;
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

            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();
            ObservableCollection<EmployeeModel> totalemployees = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employeenew));
            }

            //OverallFee = overallfee;
            Employees = totalemployees;

            pm.FormatData(true);
            SubProjects = pm.SubProjects;

            if (SubProjects.Count >0)
            {
                SelectedProjectPhase = SubProjects[0];
            }
        }

        private void DeleteRoleIfPossible(RolePerSubProjectModel rpsm)
        {
            if (rpsm.Id != 0)
            {
                LeftViewToShow = new AreYouSureView();
                AreYouSureVM aysvm = new AreYouSureVM(rpsm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = rpsm;
                LeftDrawerOpen = true;
            }
            else
            {
                SelectedProjectPhase.RolesPerSub.Remove(rpsm);
            }

            //update roles
            
        }

        private void DeleteSub(SubProjectModel spm)
        {
            if (SubProjects.Count>1)
            {
                LeftViewToShow = new AreYouSureView();
                AreYouSureVM aysvm = new AreYouSureVM(spm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = spm;
                LeftDrawerOpen = true;
            }
        }

        private void AddRole()
        {
            if (SelectedProjectPhase != null)
            {
                RolePerSubProjectModel rpspm = new RolePerSubProjectModel(SelectedProjectPhase, SelectedProjectPhase.Fee);
                rpspm.EditRoleFieldState = false;
                rpspm.SpentHours = 0;
                SelectedProjectPhase.RolesPerSub.Add(rpspm);
            }
        }

        private void AddSubProject()
        {
            LeftViewToShow = new AddSubProjectView();
            AddSubProjectVM addsubvm = new AddSubProjectVM(BaseProject, this);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
