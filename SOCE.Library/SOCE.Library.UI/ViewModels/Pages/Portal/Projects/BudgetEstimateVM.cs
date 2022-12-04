using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;

namespace SOCE.Library.UI.ViewModels
{
    public class BudgetEstimateVM : BaseVM
    {
        
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

        private ObservableCollection<RolePerSubProjectModel> _rolesPerSub = new ObservableCollection<RolePerSubProjectModel>();
        public ObservableCollection<RolePerSubProjectModel> RolesPerSub
        {
            get { return _rolesPerSub; }
            set
            {
                _rolesPerSub = value;
                RaisePropertyChanged(nameof(RolesPerSub));
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

        private SubProjectModel _selectedEmployee;
        public SubProjectModel SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged("SelectedEmployee");
            }
        }

        private double _percentAllocated;
        public double PercentAllocated
        {
            get { return _percentAllocated; }
            set
            {
                _percentAllocated = value;
                RaisePropertyChanged("PercentAllocated");
            }
        }

        private double _budgetedHours=0;
        public double BudgetedHours
        {
            get { return _budgetedHours; }
            set
            {
                _budgetedHours = value;
                RaisePropertyChanged("BudgetedHours");
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

        private bool _canAddRole = true;
        public bool CanAddRole
        {
            get { return _canAddRole; }
            set
            {
                _canAddRole = value;
                RaisePropertyChanged("CDPhase");
            }
        }

        public ICommand GoToAddRole { get; set; }
        public ICommand RemoveRoleCommand { get; set; }

        private double _overallfee;

        public BudgetEstimateVM(SubProjectModel spm, double overallfee)
        {
            SelectedProjectPhase = spm;
            this.GoToAddRole = new RelayCommand(this.AddRole);
            this.RemoveRoleCommand = new RelayCommand<RolePerSubProjectModel>(this.RemoveRole);

            ObservableCollection<RolePerSubProjectModel> totalrolespersub = new ObservableCollection<RolePerSubProjectModel>();

            if (spm.Id !=0 )
            {
                List<RolePerSubProjectDbModel> rolespersubdb = SQLAccess.LoadRolesPerSubProject(spm.Id);
                foreach (RolePerSubProjectDbModel roles in rolespersubdb)
                {
                    RolePerSubProjectModel rpm = new RolePerSubProjectModel(roles.Id, roles.Rate, (DefaultRoleEnum)roles.Role, roles.EmployeeId, spm, roles.BudgetHours, overallfee);
                    totalrolespersub.Add(rpm);
                }
            }

            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();
            ObservableCollection<EmployeeModel> totalemployees = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel employee in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employee));
            }
        
            _overallfee = overallfee;
            Employees = totalemployees;
            RolesPerSub = totalrolespersub;
        }

        private void AddRole()
        {
            RolesPerSub.Add(new RolePerSubProjectModel(SelectedProjectPhase, _overallfee));
        }

        private void RemoveRole(RolePerSubProjectModel rpsm)
        {
            RolesPerSub.Remove(rpsm);
        }
    }
}
