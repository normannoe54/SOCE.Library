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
        public bool globaleditmode = false;

        private bool _editRoleFieldState = true;
        public bool EditRoleFieldState
        {
            get { return _editRoleFieldState; }
            set
            {
                if (!_editRoleFieldState && value && !globaleditmode)
                {
                    if (Employee == null)
                    {
                        return;
                    }

                    UpdateRolePerSub();
                    Subproject.baseproject.FormatData(false);
                    //need to update sub project.
                }
                else if (_editRoleFieldState && !value && !globaleditmode)
                {
                    if (Id != 0)
                    {
                        //foreach (SubProjectModel sub in Subproject.baseproject.SubProjects)
                        //{

                        if (!string.IsNullOrEmpty(Subproject.PointNumber))
                        {
                            foreach (RolePerSubProjectModel role in Subproject.RolesPerSub)
                            {
                                if (role.Id != Id)
                                {
                                    role.EditRoleFieldState = !value;
                                }
                            }


                        }
                    }
                }

                _editRoleFieldState = value;
                EditComboRoleFieldState = !_editRoleFieldState;

                if (CanDelete)
                {
                    EditComboEmployeeFieldState = !_editRoleFieldState;
                }
                RaisePropertyChanged(nameof(EditRoleFieldState));
            }
        }

        private bool _editComboRoleFieldState = false;
        public bool EditComboRoleFieldState
        {
            get { return _editComboRoleFieldState; }
            set
            {

                _editComboRoleFieldState = value;
                RaisePropertyChanged(nameof(EditComboRoleFieldState));
            }
        }

        private bool _editComboEmployeeFieldState = false;
        public bool EditComboEmployeeFieldState
        {
            get { return _editComboEmployeeFieldState; }
            set
            {

                _editComboEmployeeFieldState = value;
                RaisePropertyChanged(nameof(EditComboEmployeeFieldState));
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

        public RolePerSubProjectModel()
        {
        }
        public RolePerSubProjectModel(SubProjectModel subproject, double overallFee)
        {
            Subproject = subproject;
            OverallFee = overallFee;
            SpentHours = 0;
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
        }

    }
}
