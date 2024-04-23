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
                IsContractVisible = _contractFee == 0 ? false : true; 
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

        private bool _isContractVisible { get; set; } = true;
        public bool IsContractVisible
        {
            get
            {
                return _isContractVisible;
            }
            set
            {
                _isContractVisible = value;
                RaisePropertyChanged(nameof(IsContractVisible));
            }
        }


        bool firstload = true;
        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                if (!firstload)
                {
                    if (value <= 100 && value >= BasePercentComplete && IsContractVisible)
                    {
                        _percentComplete = value;
                        InvoicedtoDate = ((value / 100) * ContractFee) - PreviousInvoiced;
                        ThisPeriodInvoiced = InvoicedtoDate - PreviousInvoiced;
                        RaisePropertyChanged(nameof(PercentComplete));
                        viewmodel.SumValues();
                    }

                }
                else
                {
                    if (IsContractVisible)
                    {
                        InvoicedtoDate = ((value / 100) * ContractFee) - PreviousInvoiced;

                    }
                    _percentComplete = value;
                    RaisePropertyChanged(nameof(PercentComplete));
                    firstload = !firstload;
                }

            }
        }

        public double BasePercentComplete { get; set; } = 0;

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
                InvoicedtoDate = _thisPeriodInvoiced + PreviousInvoiced;

                if (!IsContractVisible)
                {
                    viewmodel.SumValues();
                }
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

            if (iDb.PercentComplete == 100)
            {
                CanBeEdited = false;
            }
            BasePercentComplete = iDb.PercentComplete;
            PercentComplete = iDb.PercentComplete;
            ThisPeriodInvoiced = iDb.ThisPeriodInvoiced;
            ScopeName = iDb.ScopeName;
        }
    }
}
