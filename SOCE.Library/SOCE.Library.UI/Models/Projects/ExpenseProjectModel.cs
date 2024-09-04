using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SOCE.Library.Db;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SOCE.Library.UI.ViewModels;
using System.Windows.Input;
using System.Globalization;

namespace SOCE.Library.UI
{
    public class ExpenseProjectModel : PropertyChangedBase
    {

        //private ProjectLowResModel _project;
        //public ProjectLowResModel Project
        //{
        //    get
        //    {
        //        return _project;
        //    }
        //    set
        //    {
        //        _project = value;

        //        if (_project != null)
        //        {
        //            IsThisEditable = false;
        //            ProjectList = BaseProjectList;
        //        }
        //        else
        //        {
        //            IsThisEditable = true;
        //        }

        //        RaisePropertyChanged(nameof(Project));
        //    }
        //}

        private int _id = -1;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private DateTime _dateExp;
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

        private string _employeeExp;
        public string EmployeeExp
        {
            get
            {
                return _employeeExp;
            }
            set
            {
                _employeeExp = value;
                RaisePropertyChanged(nameof(EmployeeExp));
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

        private double _rate;
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
                RaisePropertyChanged(nameof(MileageExp));
            }
        }

        private bool _isInvoiced;
        public bool IsInvoiced
        {
            get
            {
                return _isInvoiced;
            }
            set
            {
                _isInvoiced = value;
                RaisePropertyChanged(nameof(IsInvoiced));
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
                RaisePropertyChanged(nameof(TypeExpense));
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

        private bool _isClientBillable;
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

        private bool _isReimbursable;
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

        private bool _isCustom;
        public bool IsCustom
        {
            get
            {
                return _isCustom;
            }
            set
            {
                _isCustom = value;
                RaisePropertyChanged(nameof(IsCustom));
            }
        }

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    UpdateExpenseReport();
                }
                _editFieldState = value;
                //ComboFieldState = !_editFieldState;

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        public ExpenseProjectModel(ExpenseReportDbModel dbmodel)
        {
            Id = dbmodel.Id;
            EmployeeDbModel dbem = SQLAccess.LoadEmployeeById(dbmodel.EmployeeId);

            if (dbem != null)
            {
                EmployeeExp = dbem.FullName;
            }
            else
            {
                EmployeeExp = "Administration";
            }

            //if (dbem != null)
            //{
            //    EmployeeExp = dbem.FullName;
            DateExp = DateTime.ParseExact(dbmodel.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DescriptionExp = dbmodel.Description;
            TypeExpense = (ExpenseEnum)dbmodel.TypeExpense;
            DateExp = DateTime.ParseExact(dbmodel.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DescriptionExp = dbmodel.Description;
            MileageExp = dbmodel.Mileage;
            TotalCostExp = dbmodel.TotalCost;
            Rate = dbmodel.MileageRate;
            IsClientBillable = Convert.ToBoolean(dbmodel.IsClientBillable);
            IsReimbursable = Convert.ToBoolean(dbmodel.Reimbursable);
            IsInvoiced = Convert.ToBoolean(dbmodel.Invoiced);

            if (IsInvoiced)
            {
                IsCustom = false;
            }
            else
            {
                IsCustom = Convert.ToBoolean(dbmodel.IsCustom);
            }
            //}
        }

        public void UpdateExpenseReport()
        {
            ExpenseReportDbModel exdb = new ExpenseReportDbModel()
            {
                Id = Id,
                Description = DescriptionExp,
                Date = (int)long.Parse(DateExp.ToString("yyyyMMdd")),
                TypeExpense = (int)TypeExpense,
                Mileage = MileageExp,
                TotalCost = TotalCostExp,
                MileageRate = Rate,
                IsClientBillable = Convert.ToInt32(IsClientBillable),
                Reimbursable = Convert.ToInt32(IsReimbursable),
            };

            SQLAccess.UpdateExpenseReport(exdb);
        }
    }
}
