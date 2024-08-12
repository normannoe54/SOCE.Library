using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using SOCE.Library.UI.Views;
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

namespace SOCE.Library.UI
{
    public class ProjectInvoicingModel : PropertyChangedBase
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

        private string _projectName { get; set; }
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                RaisePropertyChanged(nameof(ProjectName));
            }
        }

        private int _projectNumber { get; set; }
        public int ProjectNumber
        {
            get
            {
                return _projectNumber;
            }
            set
            {
                _projectNumber = value;
                RaisePropertyChanged(nameof(ProjectNumber));
            }
        }

        private EmployeeLowResModel _projectManager { get; set; }
        public EmployeeLowResModel ProjectManager
        {
            get
            {
                return _projectManager;
            }
            set
            {
                _projectManager = value;
                RaisePropertyChanged(nameof(ProjectManager));
            }
        }

        private ClientModel _client { get; set; }
        public ClientModel Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                RaisePropertyChanged(nameof(Client));
            }
        }

        private DateTime? _dateOfLastInvoice { get; set; }
        public DateTime? DateOfLastInvoice
        {
            get
            {
                return _dateOfLastInvoice;
            }
            set
            {
                _dateOfLastInvoice = value;
                RaisePropertyChanged(nameof(DateOfLastInvoice));
            }
        }

        private double _totalFeeInvoiced { get; set; }
        public double TotalFeeInvoiced
        {
            get
            {
                return _totalFeeInvoiced;
            }
            set
            {
                _totalFeeInvoiced = value;
                RaisePropertyChanged(nameof(TotalFeeInvoiced));
            }
        }

        private double _percentOfTotalFeeInvoiced { get; set; }
        public double PercentOfTotalFeeInvoiced
        {
            get
            {
                return _percentOfTotalFeeInvoiced;
            }
            set
            {
                _percentOfTotalFeeInvoiced = value;
                RaisePropertyChanged(nameof(PercentOfTotalFeeInvoiced));
            }
        }

        private double _hoursSpentSinceLastInvoice { get; set; }
        public double HoursSpentSinceLastInvoice
        {
            get
            {
                return _hoursSpentSinceLastInvoice;
            }
            set
            {
                _hoursSpentSinceLastInvoice = value;
                RaisePropertyChanged(nameof(HoursSpentSinceLastInvoice));
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        private double _fee = 0;
        public double Fee
        {
            get { return _fee; }
            set
            {
                _fee = value;
                RaisePropertyChanged(nameof(Fee));
            }
        }


        public ProjectInvoicingModel()
        {

        }

        public ProjectInvoicingModel(ProjectDbModel pm, List<InvoicingModelDb> invoices, List<TimesheetRowDbModel> time)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            IsActive = Convert.ToBoolean(pm.IsActive);
            Fee = pm.Fee;

            //List<InvoicingModelDb> invoices = SQLAccess.LoadInvoices(pm.Id);

            if (invoices.Count>0)
            {
                InvoicingModelDb closest = invoices.OrderBy(x => x.Date).LastOrDefault();
                DateOfLastInvoice = DateTime.ParseExact(closest.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                TotalFeeInvoiced = Math.Max(closest.PreviousSpent + closest.AmountDue,0);
                PercentOfTotalFeeInvoiced = (TotalFeeInvoiced / (Fee + closest.ExpensesDue)) * 100;
            }
            else
            {
                DateOfLastInvoice = null;
                TotalFeeInvoiced = 0;
                PercentOfTotalFeeInvoiced = 0;
            }

            //List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByUninvoiced(pm.Id);
            HoursSpentSinceLastInvoice = time.Sum(x => x.TimeEntry);
        }

        public ProjectInvoicingModel(ProjectDbModel pm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            IsActive = Convert.ToBoolean(pm.IsActive);
            Fee = pm.Fee;

            List<InvoicingModelDb> invoices = SQLAccess.LoadInvoices(pm.Id);

            if (invoices.Count > 0)
            {
                InvoicingModelDb closest = invoices.OrderBy(x => x.Date).LastOrDefault();
                DateOfLastInvoice = DateTime.ParseExact(closest.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                TotalFeeInvoiced = Math.Max(closest.PreviousSpent + closest.AmountDue, 0);
                PercentOfTotalFeeInvoiced = (TotalFeeInvoiced / (Fee + closest.ExpensesDue)) * 100;
            }
            else
            {
                DateOfLastInvoice = null;
                TotalFeeInvoiced = 0;
                PercentOfTotalFeeInvoiced = 0;
            }

            List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByUninvoiced(pm.Id);
            HoursSpentSinceLastInvoice = time.Sum(x => x.TimeEntry);
        }
    }
}
