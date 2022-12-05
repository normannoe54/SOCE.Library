using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class RolePerSubProjectModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private SubProjectModel _subproject { get; set; }
        public SubProjectModel Subproject
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

        private EmployeeModel _employee { get; set; }
        public EmployeeModel Employee
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

        public RolePerSubProjectModel(SubProjectModel subproject, double overallFee)
        {
            Subproject = subproject;
            OverallFee = overallFee;
        }

        public RolePerSubProjectModel(int id, double rate, DefaultRoleEnum role, int employeeId, SubProjectModel subproject, double budgetedhours, double overallFee)
        {
            Id = id;
            Employee = new EmployeeModel(SQLAccess.LoadEmployeeById(employeeId));
            Subproject = subproject;
            Role = role;
            BudgetedHours = budgetedhours;
            OverallFee = overallFee;
            Rate = rate;
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

            SQLAccess.UpdateRolesPerSubProject(rpp);
        }

    }
}
