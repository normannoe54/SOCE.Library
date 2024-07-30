using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class AddExpenseVM : BaseVM
    {
        public bool result = false;

        private ProjectViewResModel baseProject = new ProjectViewResModel();
        public ProjectViewResModel BaseProject
        {
            get { return baseProject; }
            set
            {
                baseProject = value;
                RaisePropertyChanged(nameof(BaseProject));
            }
        }

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
            }
        }

        private ExpenseEnum _typeExpense;
        public ExpenseEnum TypeExpense
        {
            get
            {
                return _typeExpense;
            }
            set
            {
                _typeExpense = value;

                MileageVis = _typeExpense == ExpenseEnum.Mileage;

                if (MileageVis)
                {
                    TotalCostExp = 0;
                    MileageExp = 0;
                }

                RaisePropertyChanged(nameof(TypeExpense));
            }
        }

        private DateTime _dateExp = DateTime.Now;
        public DateTime DateExp
        {
            get
            {
                return _dateExp;
            }
            set
            {
                _dateExp = value;
                RaisePropertyChanged(nameof(DateExp));
            }
        }

        private string _descriptionExp;
        public string DescriptionExp
        {
            get
            {
                return _descriptionExp;
            }
            set
            {
                _descriptionExp = value;
                RaisePropertyChanged(nameof(DescriptionExp));
            }
        }

        private double _rate = 0.67;
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private double _mileageExp;
        public double MileageExp
        {
            get
            {
                return _mileageExp;
            }
            set
            {
                _mileageExp = value;
                TotalCostExp = Rate * _mileageExp;
                RaisePropertyChanged(nameof(MileageExp));
            }
        }

        private double _totalCostExp;
        public double TotalCostExp
        {
            get
            {
                return _totalCostExp;
            }
            set
            {
                _totalCostExp = value;
                RaisePropertyChanged(nameof(TotalCostExp));
            }
        }

        private bool _mileageVis;
        public bool MileageVis
        {
            get
            {
                return _mileageVis;
            }
            set
            {
                _mileageVis = value;
                RaisePropertyChanged(nameof(MileageVis));
            }
        }

        private bool _isClientBillable = true;
        public bool IsClientBillable
        {
            get
            {
                return _isClientBillable;
            }
            set
            {
                _isClientBillable = value;
                RaisePropertyChanged(nameof(IsClientBillable));
            }
        }

        private bool _isReimbursable = true;
        public bool IsReimbursable
        {
            get
            {
                return _isReimbursable;
            }
            set
            {
                _isReimbursable = value;
                RaisePropertyChanged(nameof(IsReimbursable));
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

        public ICommand AddExpenseCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private ExpenseProjectVM baseViewModel;

        public AddExpenseVM(ProjectViewResModel pm, ExpenseProjectVM epvm, EmployeeModel curremployee)
        {
            CurrentEmployee = curremployee;
            baseViewModel = epvm;
            TypeExpense = ExpenseEnum.Miscellaneous;
            this.AddExpenseCommand = new RelayCommand(this.AddExpense);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            BaseProject = pm;
        }

        public void AddExpense()
        {
            if (String.IsNullOrEmpty(DescriptionExp) || DateExp == null || TotalCostExp == 0)
            {
                ErrorMessage = "Please revise inputs and resubmit";
                return;
            }

            ExpenseReportDbModel exdb = new ExpenseReportDbModel()
            {
                ProjectId = baseProject.Id,
                EmployeeId = CurrentEmployee.Id,
                Description = DescriptionExp,
                Date = (int)long.Parse(DateExp.ToString("yyyyMMdd")),
                TypeExpense = (int)TypeExpense,
                Mileage = MileageExp,
                TotalCost = TotalCostExp,
                MileageRate = Rate,
                Invoiced = 0,
                IsClientBillable = Convert.ToInt32(IsClientBillable),
                Reimbursable = Convert.ToInt32(IsReimbursable),
                IsCustom = 1
            };

            SQLAccess.AddExpenseReport(exdb);

            result = true;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            baseViewModel.LoadExpensesForViewing();
            baseViewModel.LeftDrawerOpen = false;
        }
    }
}
