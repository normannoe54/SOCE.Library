using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public class RoleSummaryModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private SubProjectSummaryModel _subproject { get; set; }
        public SubProjectSummaryModel Subproject
        {
            get
            {
                return _subproject;
            }
            set
            {
                _subproject = value;
                RaisePropertyChanged(nameof(Subproject));
            }
        }

        private ObservableCollection<EmployeeLowResModel> _employeeList { get; set; } = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> EmployeeList
        {
            get
            {
                return _employeeList;
            }
            set
            {
                _employeeList = value;
                RaisePropertyChanged(nameof(EmployeeList));
            }
        }

        private EmployeeLowResModel _employee { get; set; } = new EmployeeLowResModel();
        public EmployeeLowResModel Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                if (Rate == 0)
                {
                    Rate = Employee.Rate;
                }
                RaisePropertyChanged(nameof(Employee));
            }
        }

        private DefaultRoleEnum _role { get; set; }
        public DefaultRoleEnum Role
        {
            get
            {
                return _role;
            }
            set
            {
                _role = value;
                RaisePropertyChanged(nameof(Role));
            }
        }

        private double _rate { get; set; }
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                UpdatePercentBudget();
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private double _budgetedHours { get; set; }
        public double BudgetedHours
        {
            get
            {
                return _budgetedHours;
            }
            set
            {
                _budgetedHours = value;
                UpdatePercentBudget();
                RaisePropertyChanged(nameof(BudgetedHours));
            }
        }

        private double _percentofBudget { get; set; }
        public double PercentofBudget
        {
            get
            {
                return _percentofBudget;
            }
            set
            {
                _percentofBudget = value;
                RaisePropertyChanged(nameof(PercentofBudget));
            }
        }

        private double _percentofRegulatedBudget { get; set; }
        public double PercentofRegulatedBudget
        {
            get
            {
                return _percentofRegulatedBudget;
            }
            set
            {
                _percentofRegulatedBudget = value;
                RaisePropertyChanged(nameof(PercentofRegulatedBudget));
            }
        }

        private double _overallFee { get; set; }
        public double OverallFee
        {
            get
            {
                return _overallFee;
            }
            set
            {
                _overallFee = value;
                UpdatePercentBudget();

            }
        }

        private bool _textBoxVisible { get; set; } = false;
        public bool TextBoxVisible
        {
            get
            {
                return _textBoxVisible;
            }
            set
            {
                _textBoxVisible = value;
                RaisePropertyChanged(nameof(TextBoxVisible));
            }
        }

        private double _spentHours { get; set; }
        public double SpentHours
        {
            get
            {
                return _spentHours;
            }
            set
            {
                _spentHours = value;
                //SpentBudget = _spentHours * Rate;

                if (SpentHours == 0)
                {
                    SpentBudget = 0;
                }

                if (_spentHours > 0)
                {
                    TextBoxVisible = true;
                }

                PercentSpent = (_spentHours / BudgetedHours) * 100;

                if (Subproject != null)
                {
                    if ((Subproject?.Description == "All Phases") || (!Subproject.baseproject.IsActive))
                    {
                        CanDelete = false;
                    }
                    else
                    {
                        CanDelete = _spentHours == 0;
                    }
                }
                else
                {
                    CanDelete = false;
                }

                RaisePropertyChanged(nameof(SpentHours));

            }
        }

        private double _spentBudget { get; set; }
        public double SpentBudget
        {
            get
            {
                return _spentBudget;
            }
            set
            {
                _spentBudget = value;
                //PercentSpent = (_spentBudget / OverallFee) * 100;
                RaisePropertyChanged(nameof(SpentBudget));

            }
        }

        private double _plannedBudget { get; set; }
        public double PlannedBudget
        {
            get
            {
                return _plannedBudget;
            }
            set
            {
                _plannedBudget = value;
                RaisePropertyChanged(nameof(PlannedBudget));
            }
        }

        private double _percentSpent { get; set; }
        public double PercentSpent
        {
            get
            {
                return _percentSpent;
            }
            set
            {
                _percentSpent = value;
                RaisePropertyChanged(nameof(PercentSpent));
            }
        }

        private bool _editRoleFieldState = true;
        public bool EditRoleFieldState
        {
            get { return _editRoleFieldState; }
            set
            {
                if (!_editRoleFieldState && value)
                {
                    UpdateRolePerSub();
                }

                _editRoleFieldState = value;
                RaisePropertyChanged(nameof(EditRoleFieldState));
            }
        }

        private bool _canDelete { get; set; }
        public bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            set
            {
                _canDelete = value;
                RaisePropertyChanged(nameof(CanDelete));

            }
        }

        //private bool _textBoxVisible { get; set; } = false;
        //public bool TextBoxVisible
        //{
        //    get
        //    {
        //        return _textBoxVisible;
        //    }
        //    set
        //    {
        //        _textBoxVisible = value;
        //        RaisePropertyChanged(nameof(TextBoxVisible));
        //    }
        //}
        public ProjectSummaryVM projectsummaryvm;

        public RoleSummaryModel()
        {
        }

        public RoleSummaryModel(RolePerSubProjectDbModel dbrole, SubProjectSummaryModel sub, ProjectSummaryVM vm, ObservableCollection<EmployeeLowResModel> employees, List<TimesheetRowDbModel> time)
        {
            projectsummaryvm = vm;
            Subproject = sub;
            Id = dbrole.Id;
            EmployeeList = employees;

            EmployeeLowResModel emlowres = EmployeeList.Where(x => x.Id == dbrole.EmployeeId).FirstOrDefault();

            if (emlowres == null)
            {
                EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(dbrole.EmployeeId);
                EmployeeLowResModel emnew = new EmployeeLowResModel(emdb);
                EmployeeList.Add(emnew);
                Employee = emnew;
            }
            else
            {
                Employee = emlowres;
            }

            Role = (DefaultRoleEnum)dbrole.Role;
            BudgetedHours = dbrole.BudgetHours;
            Rate = dbrole.Rate;
            //List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByIds(dbrole.EmployeeId, sub.Id);
            SpentHours = time.Sum(x => x.TimeEntry);
            SpentBudget = time.Sum(x => x.BudgetSpent);
            PlannedBudget = BudgetedHours * Rate;
            PercentofRegulatedBudget = (SpentBudget / (PlannedBudget)) * 100;
        }

        public RoleSummaryModel(SubProjectSummaryModel subproject, double overallFee, ProjectSummaryVM vm, ObservableCollection<EmployeeLowResModel> employees)
        {
            projectsummaryvm = vm;
            EmployeeList = employees;
            Subproject = subproject;
            OverallFee = overallFee;
            SpentHours = 0;
        }

        private void UpdatePercentBudget()
        {
            double totalspent = _rate * BudgetedHours;
            PercentofBudget = Math.Round((totalspent / OverallFee) * 100, 2);
        }

        public void UpdateRolePerSub()
        {

            RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
            {
                Id = Id,
                EmployeeId = Employee.Id,
                SubProjectId = Subproject.Id,
                Rate = Rate,
                Role = Convert.ToInt32(Role),
                BudgetHours = BudgetedHours
            };

            if (Id == 0)
            {

                int val = SQLAccess.AddRolesPerSubProject(rpp);
                Id = val;
            }
            else
            {
                SQLAccess.UpdateRolesPerSubProject(rpp);
            }

            projectsummaryvm.CollectSubProjectsInfo();
        }
    }
}
