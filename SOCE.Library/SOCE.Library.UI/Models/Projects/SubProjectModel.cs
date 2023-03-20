using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
                    rspm.PercentofRegulatedBudget = ((rspm.BudgetedHours * rspm.Rate) / _regulatedBudget)*100;
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

                if (RolesPerSub.Count>0)
                {
                    UpdatePercentAllocated();
                }

                //PercentBudget = (Fee / TotalFee) * 100;
                RaisePropertyChanged(nameof(Fee));
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
                    //UpdateSubProject();
                    baseproject.UpdateSubProjects(this);
                }
                _editSubFieldState = value;
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
            Fee = spm.Fee;
            IsActive = Convert.ToBoolean(spm.IsActive);
            IsInvoiced = Convert.ToBoolean(spm.IsInvoiced);
            PercentComplete = spm.PercentComplete;
            PercentBudget = spm.PercentBudget;
            onstartup = false;
            isAddProj = false;

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

        public void UpdateSubProject()
        {
            SubProjectDbModel subproject = new SubProjectDbModel()
            {
                Id = Id,
                ProjectId = ProjectNumber,
                PointNumber = PointNumber,
                Description = Description,
                Fee = Fee,
                IsActive = IsActive ? 1 : 0,
                IsInvoiced = IsInvoiced ? 1 : 0,
                PercentComplete = PercentComplete,
                PercentBudget = PercentBudget,
            };

            SQLAccess.UpdateSubProject(subproject);
        }

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
            { Id = this.Id, 
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
