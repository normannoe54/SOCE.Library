using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class SubProjectModel : PropertyChangedBase, ICloneable
    {
        private ObservableCollection<RolePerSubProjectModel> _rolesPerSub = new ObservableCollection<RolePerSubProjectModel>();
        public ObservableCollection<RolePerSubProjectModel> RolesPerSub
        {
            get { return _rolesPerSub; }
            set
            {
                _rolesPerSub = value;
                RaisePropertyChanged(nameof(RolesPerSub));
            }
        }

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
                //if (!_editSubFieldState && IsAddService)
                //{
                //    double doubleout = 0;
                //    Double.TryParse(value, out doubleout);

                //    if (doubleout < 1 && 0 < doubleout)
                //    {
                //        doubleout.ToString();
                //    }
                //    else { return; }
                //}
                //else
                //{
                _pointNumber = value;
                //}
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
                if (isAddProj)
                {
                    UpdateFee();
                }

                //update others
                foreach (RolePerSubProjectModel rspm in RolesPerSub)
                {
                    rspm.OverallFee = Fee;
                }
                RaisePropertyChanged(nameof(PercentBudget));
            }
        }

        public bool isAddProj = false;


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

        private bool onstartup = true;

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
                if (isAddProj)
                {
                    UpdateFee();
                }
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

                foreach (RolePerSubProjectModel rspm in RolesPerSub)
                {
                    rspm.PercentofRegulatedBudget = ((rspm.BudgetedHours * rspm.Rate) / _regulatedBudget) * 100;
                }

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

                //update others
                foreach (RolePerSubProjectModel rspm in RolesPerSub)
                {
                    rspm.OverallFee = Fee;
                }

                if (RolesPerSub.Count > 0)
                {
                    UpdatePercentAllocated();
                }

                //PercentBudget = (Fee / TotalFee) * 100;
                RaisePropertyChanged(nameof(Fee));
            }
        }

        private bool _isScheduleActive { get; set; }
        public bool IsScheduleActive
        {
            get
            {
                return _isScheduleActive;
            }
            set
            {
                _isScheduleActive = value;

                if (_isScheduleActive)
                {
                    if (baseproject != null)
                    {
                        foreach (SubProjectModel sub in baseproject.SubProjects)
                        {
                            if (sub.Id != Id)
                            {
                                sub.IsScheduleActive = false;
                            }
                        }
                    }
                }
                RaisePropertyChanged(nameof(IsScheduleActive));
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

        private bool _canAdd { get; set; } = true;
        public bool CanAdd
        {
            get
            {
                return _canAdd;
            }
            set
            {
                _canAdd = value;
                RaisePropertyChanged(nameof(CanAdd));
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
        public bool globaleditmode = false;
        private bool _editSubFieldState = true;
        public bool EditSubFieldState
        {
            get { return _editSubFieldState; }
            set
            {
                if (!_editSubFieldState && value && !globaleditmode)
                {
                    _editSubFieldState = value;
                    baseproject.UpdateSubProjects();
                }
                else if (_editSubFieldState && !value && !globaleditmode)
                {
                    if (Id != 0)
                    {
                        foreach (SubProjectModel sub in baseproject.SubProjects)
                        {
                            if (sub.Id != Id)
                            {
                                sub.EditSubFieldState = !value;
                            }
                        }
                    }
                    _editSubFieldState = value;
                }
                else
                {
                    _editSubFieldState = value;
                }
                ComboSubFieldState = !_editSubFieldState;

                RaisePropertyChanged(nameof(EditSubFieldState));
            }
        }

        private bool _comboSubFieldState = false;
        public bool ComboSubFieldState
        {
            get { return _comboSubFieldState; }
            set
            {
                _comboSubFieldState = value;
                RaisePropertyChanged(nameof(ComboSubFieldState));
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

        private bool _canDelete { get; set; } = false;
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

        public string PointNumStr
        {
            get
            {
                //if (ProjectNumber != null)
                //{
                string jobnumstr = $"[.{PointNumber}]";
                //}
                return jobnumstr;
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
        private bool _isVisible { get; set; } = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                RaisePropertyChanged(nameof(IsVisible));
            }
        }

        private double _totalScheduledHours { get; set; }
        public double TotalScheduledHours
        {
            get
            {
                return _totalScheduledHours;
            }
            set
            {
                _totalScheduledHours = value;
                RaisePropertyChanged(nameof(TotalScheduledHours));
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

        private bool? _isBillable { get; set; } = true;
        public bool? IsBillable
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

        public SubProjectModel()
        {
            onstartup = false;
        }

        public SubProjectModel(SubProjectDbModel spm)
        {
            Constructor(spm);
        }

        public SubProjectModel(SubProjectDbModel spm, double totalfee, ProjectModel pm)
        {
            TotalFee = totalfee;
            baseproject = pm;
            Constructor(spm);
        }


        public void Constructor(SubProjectDbModel spm)
        {
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
            EmployeeIdSigned = spm.EmployeeIdSigned;

            if (spm?.SubStart != null && spm?.SubStart != 0)
            {
                DateInitiated = DateTime.ParseExact(spm.SubStart.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            if (spm?.SubEnd != null && spm?.SubEnd != 0)
            {
                DateInvoiced = DateTime.ParseExact(spm.SubEnd.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            PercentComplete = spm.PercentComplete;
            //PercentBudget = spm.PercentBudget;
            IsAddService = Convert.ToBoolean(spm.IsAdservice);
            NumberOrder = spm.NumberOrder;
            IsScheduleActive = Convert.ToBoolean(spm.IsScheduleActive);

            onstartup = false;
            //isAddProj = false;
        }

        public void SetCollectionChanged()
        {
            RolesPerSub.CollectionChanged += RowDataChanged;
        }

        public ProjectModel baseproject;

        public void UpdatePercents()
        {
            PercentBudget = (RegulatedBudget / TotalFee) * 100;
            PercentofInvoicedFee = Math.Round(RegulatedBudget / Fee * 100, 2);
            PercentSpent = Math.Round(FeeUsed / RegulatedBudget * 100, 2);
        }

        private void UpdateFee()
        {
            Fee = PercentBudget * (TotalFee / 100);
        }

        //public void UpdatePercentBudget()
        //{
        //    PercentBudget = (Fee / TotalFee) * 100;
        //}

        //public void UpdateSubProject()
        //{
        //    if (Id != 0)
        //    {
        //        SubProjectDbModel subproject = new SubProjectDbModel()
        //        {
        //            Id = Id,
        //            ProjectId = ProjectNumber,
        //            PointNumber = PointNumber,
        //            Description = Description,

        //            Fee = Fee,
        //            IsActive = IsActive ? 1 : 0,
        //            IsInvoiced = IsInvoiced ? 1 : 0,
        //            PercentComplete = PercentComplete,
        //            PercentBudget = PercentBudget,
        //        };

        //        SQLAccess.UpdateSubProject(subproject);
        //    }       
        //}

        private void RowDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.NewItems)
                {
                    RolePerSubProjectModel spm = (RolePerSubProjectModel)added;
                    spm.PropertyChanged += ItemModificationOnPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.OldItems)
                {
                    RolePerSubProjectModel spm = (RolePerSubProjectModel)added;
                    spm.PropertyChanged -= ItemModificationOnPropertyChanged;
                }
            }
        }

        private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdatePercentAllocated();
        }

        private void UpdatePercentAllocated()
        {
            double totalbudgetspent = 0;

            foreach (RolePerSubProjectModel role in RolesPerSub)
            {
                totalbudgetspent += role.BudgetedHours * role.Rate;
            }

            BudgetedFee = totalbudgetspent;
            PercentAllocated = Math.Round((totalbudgetspent / Fee * 100), 2);
        }


        public object Clone()
        {
            return new SubProjectModel()
            {
                Id = this.Id,
                ProjectNumber = this.ProjectNumber,
                PointNumber = this.PointNumber,
                Description = this.Description,
                IsActive = this.IsActive,
                IsInvoiced = this.IsInvoiced,
                Fee = this.Fee
            };
        }
    }
}
