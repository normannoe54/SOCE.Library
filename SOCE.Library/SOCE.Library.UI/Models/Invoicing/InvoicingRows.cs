using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public class InvoicingRows : PropertyChangedBase
    {
        private int _id { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private int _subId { get; set; }
        public int SubId
        {
            get
            {
                return _subId;
            }
            set
            {
                _subId = value;
                RaisePropertyChanged(nameof(SubId));
            }
        }

        private int _invoiceId { get; set; }
        public int InvoiceId
        {
            get
            {
                return _invoiceId;
            }
            set
            {
                _invoiceId = value;
                RaisePropertyChanged(nameof(InvoiceId));
            }
        }

        private double _contractFee { get; set; }
        public double ContractFee
        {
            get
            {
                return _contractFee;
            }
            set
            {
                _contractFee = value;
                //IsContractVisible = _contractFee == 0 ? false : true; 
                RaisePropertyChanged(nameof(ContractFee));
            }
        }

        private bool _canBeEdited { get; set; } = true;
        public bool CanBeEdited
        {
            get
            {
                return _canBeEdited;
            }
            set
            {
                _canBeEdited = value;
                RaisePropertyChanged(nameof(CanBeEdited));
            }
        }

        private ExpenseEnum _typeOfExpense { get; set; } = ExpenseEnum.Miscellaneous;
        public ExpenseEnum TypeOfExpense
        {
            get
            {
                return _typeOfExpense;
            }
            set
            {
                _typeOfExpense = value;
                RaisePropertyChanged(nameof(TypeOfExpense));
            }
        }
        private bool _isExpense { get; set; } = false;
        public bool IsExpense
        {
            get
            {
                return _isExpense;
            }
            set
            {
                _isExpense = value;
                RaisePropertyChanged(nameof(IsExpense));
            }
        }

        //private bool _isContractVisible { get; set; } = true;
        //public bool IsContractVisible
        //{
        //    get
        //    {
        //        return _isContractVisible;
        //    }
        //    set
        //    {
        //        _isContractVisible = value;
        //        RaisePropertyChanged(nameof(IsContractVisible));
        //    }
        //}

        private bool _isBillable { get; set; } = true;
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

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    if (!IsHourly && !IsExpense)
                    {
                        if (BasePercentComplete > PercentComplete)
                        {
                            PercentComplete = BasePercentComplete;
                        }
                        else if (PercentComplete > 100)
                        {
                            PercentComplete = 100;
                        }

                        InvoicedtoDate = ((PercentComplete / 100) * ContractFee);
                        ThisPeriodInvoiced = InvoicedtoDate - PreviousInvoiced;
                    }
                    else
                    {
                        double outstanding = Math.Max(ContractFee - ThisPeriodInvoiced - InvoicedtoDate,0);

                        if (outstanding < 0)
                        {
                            ThisPeriodInvoiced = 0;
                        }
                        else
                        {
                            InvoicedtoDate = ThisPeriodInvoiced + PreviousInvoiced;
                        }
                    }

                    //RaisePropertyChanged(nameof(PercentComplete));
                    viewmodel.SumValues();
                }

                _editFieldState = value;
                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        bool firstload = true;
        public double BasePercentComplete { get; set; } = 0;
        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                //if (!firstload)
                //{
                //    if (value <= 100 && value >= BasePercentComplete && IsContractVisible)
                //    {
                //        _percentComplete = value;
                //        InvoicedtoDate = ((value / 100) * ContractFee) - PreviousInvoiced;
                //        ThisPeriodInvoiced = InvoicedtoDate - PreviousInvoiced;
                //        RaisePropertyChanged(nameof(PercentComplete));
                //        viewmodel.SumValues();
                //    }

                //}
                //else
                //{
                //    if (IsContractVisible)
                //    {
                //        InvoicedtoDate = ((value / 100) * ContractFee) - PreviousInvoiced;

                //    }
                //    BasePercentComplete = value;
                //    _percentComplete = value;
                //    RaisePropertyChanged(nameof(PercentComplete));
                //    firstload = !firstload;
                //}

                if (firstload)
                {
                    if (!IsHourly)
                    {
                        if (ContractFee > 0)
                        {
                            InvoicedtoDate = ((value / 100) * ContractFee);
                        }
                    }
                    else
                    {
                        InvoicedtoDate = PreviousInvoiced + ThisPeriodInvoiced;
                    }

                    //BasePercentComplete = value;
                    firstload = !firstload;
                }
                
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));

            }
        }

        private bool _isHourly { get; set; }
        public bool IsHourly
        {
            get
            {
                return _isHourly;
            }
            set
            {
                _isHourly = value;

                //if (ContractFee > 0)
                //{
                //    HourlyStatement = ContractFee > 0 ? $"Hourly not to exceed ${ContractFee}" : "Hourly";
                //}

                RaisePropertyChanged(nameof(IsHourly));
            }
        }

        private string _hourlyStatement { get; set; } = "Hourly";
        public string HourlyStatement
        {
            get
            {
                return _hourlyStatement;
            }
            set
            {
                _hourlyStatement = value;
                RaisePropertyChanged(nameof(HourlyStatement));
            }
        }

        private double _previousInvoiced { get; set; }
        public double PreviousInvoiced
        {
            get
            {
                return _previousInvoiced;
            }
            set
            {
                _previousInvoiced = value;
                RaisePropertyChanged(nameof(PreviousInvoiced));
            }
        }

        private double _invoicedtoDate { get; set; }
        public double InvoicedtoDate
        {
            get
            {
                return _invoicedtoDate;
            }
            set
            {
                _invoicedtoDate = value;
                RaisePropertyChanged(nameof(InvoicedtoDate));
            }
        }

        private double _thisPeriodInvoiced { get; set; }
        public double ThisPeriodInvoiced
        {
            get
            {
                return _thisPeriodInvoiced;
            }
            set
            {
                _thisPeriodInvoiced = value;
                //InvoicedtoDate = _thisPeriodInvoiced + PreviousInvoiced;

                //if (!IsContractVisible)
                //{
                    //viewmodel.SumValues();
                //}
                RaisePropertyChanged(nameof(ThisPeriodInvoiced));
            }
        }

        private string _scopeName { get; set; }
        public string ScopeName
        {
            get
            {
                return _scopeName;
            }
            set
            {
                _scopeName = value;
                RaisePropertyChanged(nameof(ScopeName));
            }
        }

        private string _dateOfInvoice { get; set; }
        public string DateOfInvoice
        {
            get
            {
                return _dateOfInvoice;
            }
            set
            {
                _dateOfInvoice = value;
                RaisePropertyChanged(nameof(DateOfInvoice));
            }
        }

        private double _budgetSpent { get; set; }
        public double BudgetSpent
        {
            get
            {
                return _budgetSpent;
            }
            set
            {
                _budgetSpent = value;
                RaisePropertyChanged(nameof(BudgetSpent));
            }
        }

        private bool _isEditable { get; set; }
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }

        public CreateInvoiceVM viewmodel;

        public InvoicingRows()
        {

        }

        public InvoicingRows(InvoicingRowsDb iDb)
        {
            Id = iDb.Id;
            SubId = iDb.SubId;
            InvoiceId = iDb.InvoiceId;
            PreviousInvoiced = iDb.PreviousInvoiced;

            //if (iDb.PercentComplete == 100)
            //{
            //    CanBeEdited = false;
            //}
            
            //PercentComplete = iDb.PercentComplete;
            ThisPeriodInvoiced = iDb.ThisPeriodInvoiced;
            ScopeName = iDb.ScopeName;
        }
    }
}
