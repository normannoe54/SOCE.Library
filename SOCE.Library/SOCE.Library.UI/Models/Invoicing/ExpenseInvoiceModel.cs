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
    public class ExpenseInvoiceModel : PropertyChangedBase
    {

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

        private double _total;
        public double Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
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

        private string _typeExpense;
        public string TypeExpense
        {
            get
            {
                return _typeExpense;
            }
            set
            {
                _typeExpense = value;
                RaisePropertyChanged(nameof(TypeExpense));
            }
        }

        private double _totalCost;
        public double TotalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                _totalCost = value;
                RaisePropertyChanged(nameof(TotalCost));
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

        private bool _isBillable;
        public bool IsBillable
        {
            get
            {
                return _isBillable;
            }
            set
            {
                _isBillable = value;
                RaisePropertyChanged(nameof(IsBillable));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public ExpenseInvoiceModel()
        {
        }

        public ExpenseInvoiceModel(ExpenseReportDbModel dbmodel)
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
            DateExp = DateTime.ParseExact(dbmodel.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DescriptionExp = dbmodel.Description;
            TotalCost = dbmodel.TotalCost;
            IsBillable = Convert.ToBoolean(dbmodel.IsClientBillable);
            TypeExpense = ((ExpenseEnum)dbmodel.TypeExpense).ToString();
            IsInvoiced = Convert.ToBoolean(dbmodel.Invoiced);
            //}
            IsSelected = IsInvoiced;
        }

        //private void SumTable()
        //{
        //    Total = HotelExp + TransportExp + ParkingExp + MealsExp + MiscExp + MileageTotalCostExp;
        //}

    }
}
