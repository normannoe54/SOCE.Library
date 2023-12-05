using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public class SubProjectAddServiceModel : PropertyChangedBase
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

        private int _numberOrder { get; set; }
        public int NumberOrder
        {
            get
            {
                return _numberOrder;
            }
            set
            {
                _numberOrder = value;
                RaisePropertyChanged(nameof(NumberOrder));
            }
        }

        private string _pointNumber { get; set; }
        public string PointNumber
        {
            get
            {
                return _pointNumber;
            }
            set
            {

                _pointNumber = value;
                RaisePropertyChanged(nameof(PointNumber));
            }
        }

        private string _description { get; set; }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        private double _percentBudget { get; set; } = 0;
        public double PercentBudget
        {
            get
            {
                return _percentBudget;
            }
            set
            {
                _percentBudget = value;
                RaisePropertyChanged(nameof(PercentBudget));
            }
        }

        private double _percentSpent;
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

        private double _totalFee;
        public double TotalFee
        {
            get
            {
                return _totalFee;
            }
            set
            {
                _totalFee = value;
                RaisePropertyChanged(nameof(TotalFee));
            }
        }

        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private double _totalHours { get; set; }
        public double TotalHours
        {
            get
            {
                return _totalHours;
            }
            set
            {
                _totalHours = value;
                RaisePropertyChanged(nameof(TotalHours));
            }
        }

        private double _hoursUsed { get; set; }
        public double HoursUsed
        {
            get
            {
                return _hoursUsed;
            }
            set
            {
                _hoursUsed = value;
                CanDelete = _hoursUsed == 0;
                RaisePropertyChanged(nameof(HoursUsed));
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

        private bool _canEdit { get; set; } = true;
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                _canEdit = value;
                RaisePropertyChanged(nameof(CanEdit));
            }
        }

        private double _hoursLeft { get; set; }
        public double HoursLeft
        {
            get
            {
                return _hoursLeft;
            }
            set
            {
                _hoursLeft = value;
                RaisePropertyChanged(nameof(HoursLeft));
            }
        }

        private double _feeUsed { get; set; }
        public double FeeUsed
        {
            get
            {
                return _feeUsed;
            }
            set
            {
                _feeUsed = value;
                RaisePropertyChanged(nameof(FeeUsed));
            }
        }

        private double _feeLeft { get; set; }
        public double FeeLeft
        {
            get
            {
                return _feeLeft;
            }
            set
            {
                _feeLeft = value;
                RaisePropertyChanged(nameof(FeeLeft));
            }
        }

        private double _regulatedBudget { get; set; }
        public double RegulatedBudget
        {
            get
            {
                return _regulatedBudget;
            }
            set
            {
                _regulatedBudget = value;
                RaisePropertyChanged(nameof(RegulatedBudget));
            }
        }

        private double _percentofInvoicedFee { get; set; }
        public double PercentofInvoicedFee
        {
            get
            {
                return _percentofInvoicedFee;
            }
            set
            {
                _percentofInvoicedFee = value;
                RaisePropertyChanged(nameof(PercentofInvoicedFee));
            }
        }

        private double _fee { get; set; }
        public double Fee
        {
            get
            {
                return _fee;
            }
            set
            {
                _fee = value;
                RaisePropertyChanged(nameof(Fee));
            }
        }

        bool runcheck = true;

        private bool _isScheduleActive { get; set; }
        public bool IsScheduleActive
        {
            get
            {
                return _isScheduleActive;
            }
            set
            {
                if (_isScheduleActive != value)
                {
                    _isScheduleActive = value;
                    RaisePropertyChanged(nameof(IsScheduleActive));
                }
            }
        }

        private bool _isActive { get; set; }
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        private bool _isInvoiced { get; set; }
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

        private bool _editSubFieldState = true;
        public bool EditSubFieldState
        {
            get { return _editSubFieldState; }
            set
            {

                if (!_editSubFieldState && value)
                {
                    UpdateSubProject();
                }

                _editSubFieldState = value;
                RaisePropertyChanged(nameof(EditSubFieldState));
            }
        }

        private bool _isAddService { get; set; }
        public bool IsAddService
        {
            get
            {
                return _isAddService;
            }
            set
            {
                _isAddService = value;
                RaisePropertyChanged(nameof(IsAddService));
            }
        }

        private string _expandedDescription { get; set; }
        public string ExpandedDescription
        {
            get
            {
                return _expandedDescription;
            }
            set
            {
                _expandedDescription = value;
                RaisePropertyChanged(nameof(ExpandedDescription));
            }
        }

        private double _percentAllocated = 0;
        public double PercentAllocated
        {
            get { return _percentAllocated; }
            set
            {
                _percentAllocated = value;
                RaisePropertyChanged("PercentAllocated");
            }
        }

        private double _budgetedFee = 0;
        public double BudgetedFee
        {
            get { return _budgetedFee; }
            set
            {
                _budgetedFee = value;
                RaisePropertyChanged("BudgetedFee");
            }
        }


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

        private DateTime? _dateInitiated;
        public DateTime? DateInitiated
        {
            get { return _dateInitiated; }
            set
            {
                _dateInitiated = value;
                RaisePropertyChanged(nameof(DateInitiated));
            }
        }

        private DateTime? _dateInvoiced;
        public DateTime? DateInvoiced
        {
            get { return _dateInvoiced; }
            set
            {
                _dateInvoiced = value;
                RaisePropertyChanged(nameof(DateInvoiced));
            }
        }

        private string _nameOfClient { get; set; }
        public string NameOfClient
        {
            get
            {
                return _nameOfClient;
            }
            set
            {
                _nameOfClient = value;
                RaisePropertyChanged(nameof(NameOfClient));
            }
        }

        private string _clientAddress { get; set; }
        public string ClientAddress
        {
            get
            {
                return _clientAddress;
            }
            set
            {
                _clientAddress = value;
                RaisePropertyChanged(nameof(ClientAddress));
            }
        }

        private string _clientCity { get; set; }
        public string ClientCity
        {
            get
            {
                return _clientCity;
            }
            set
            {
                _clientCity = value;
                RaisePropertyChanged(nameof(ClientCity));
            }
        }

        private string _clientCompanyName { get; set; }
        public string ClientCompanyName
        {
            get
            {
                return _clientCompanyName;
            }
            set
            {
                _clientCompanyName = value;
                RaisePropertyChanged(nameof(ClientCompanyName));
            }
        }

        private string _personAddressed { get; set; }
        public string PersonAddressed
        {
            get
            {
                return _personAddressed;
            }
            set
            {
                _personAddressed = value;
                RaisePropertyChanged(nameof(PersonAddressed));
            }
        }

        private EmployeeLowResModel _selectedEmployee { get; set; }
        public EmployeeLowResModel SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        private int _employeeIdSigned { get; set; }
        public int EmployeeIdSigned
        {
            get
            {
                return _employeeIdSigned;
            }
            set
            {
                _employeeIdSigned = value;
                RaisePropertyChanged(nameof(EmployeeIdSigned));
            }
        }

        public ProjectViewResModel baseproject;
        public SubProjectAddServiceModel()
        { }

        public SubProjectAddServiceModel(SubProjectDbModel spm, ProjectViewResModel pm, EmployeeLowResModel elrm)
        {
            TotalFee = pm.Fee;
            baseproject = pm;
            Id = spm.Id;
            ProjectNumber = spm.ProjectId;
            PointNumber = spm.PointNumber;
            Description = spm.Description;
            ExpandedDescription = spm.ExpandedDescription;
            Fee = spm.Fee;
            IsActive = Convert.ToBoolean(spm.IsActive);
            IsBillable = Convert.ToBoolean(spm.IsBillable);
            IsInvoiced = Convert.ToBoolean(spm.IsInvoiced);
            NameOfClient = spm.NameOfClient;
            ClientCompanyName = spm.ClientCompanyName;
            ClientAddress = spm.ClientAddress;
            ClientCity = spm.ClientCity;
            EmployeeIdSigned = spm.EmployeeIdSigned;
            PersonAddressed = spm.PersonToAddress;

            SelectedEmployee = elrm;

            if (spm?.SubStart != null && spm?.SubStart != 0)
            {
                DateInitiated = DateTime.ParseExact(spm.SubStart.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            if (spm?.SubEnd != null && spm?.SubEnd != 0)
            {
                DateInvoiced = DateTime.ParseExact(spm.SubEnd.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            PercentComplete = spm.PercentComplete;

            IsAddService = Convert.ToBoolean(spm.IsAdservice);
            NumberOrder = spm.NumberOrder;
            IsScheduleActive = Convert.ToBoolean(spm.IsScheduleActive);
        }

        public void UpdateSubProject()
        {
            if (IsScheduleActive)
            {
                IsActive = true;
            }

            if (SelectedEmployee != null)
            {
                if (EmployeeIdSigned != SelectedEmployee.Id)
                {
                    EmployeeIdSigned = SelectedEmployee.Id;
                }
            }

            //if (!IsActive)
            //{
            //    List<SubProjectDbModel> subs = SQLAccess.LoadAllSubProjectsByProject(baseproject.Id);
            //    bool found = subs.Any(x => x.IsActive == 1);
            //}

            SubProjectDbModel subproject = new SubProjectDbModel()
            {
                Id = Id,
                PointNumber = PointNumber,
                Description = Description,
                Fee = Fee,
                IsActive = IsActive ? 1 : 0,
                PercentComplete = PercentComplete,
                PercentBudget = PercentBudget,
                IsInvoiced = IsInvoiced ? 1 : 0,
                SubStart = DateInitiated != null ? (int)long.Parse(DateInitiated?.ToString("yyyyMMdd")) : (int?)null,
                SubEnd = DateInvoiced != null ? (int)long.Parse(DateInvoiced?.ToString("yyyyMMdd")) : (int?)null,
                NameOfClient = NameOfClient,
                ClientCompanyName = ClientCompanyName,
                ClientAddress = ClientAddress,
                EmployeeIdSigned = EmployeeIdSigned,
                ExpandedDescription = ExpandedDescription,
                IsAdservice = IsAddService ? 1 : 0,
                IsBillable = IsBillable ? 1 : 0,
                NumberOrder = NumberOrder,
                IsScheduleActive = IsScheduleActive ? 1 : 0
            };

            SQLAccess.UpdateSubAddService(subproject);
        }
    }
}
