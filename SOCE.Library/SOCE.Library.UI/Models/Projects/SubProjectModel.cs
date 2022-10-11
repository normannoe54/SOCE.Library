using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class SubProjectModel : PropertyChangedBase, ICloneable
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

        private double _percentBudget { get; set; }
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
                UpdateFee();
            }
        }
        private bool onstartup = true;
        private bool canupdatepercentbudget = false;
        private bool canupdatefee = false;


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
                PercentBudget = Math.Round(Fee / TotalFee * 100, 2);
                RaisePropertyChanged(nameof(TotalFee));
            }
        }

        //private double _estimatedFee { get; set; }
        //public double EstimatedFee
        //{
        //    get
        //    {
        //        return _estimatedFee;
        //    }
        //    set
        //    {
        //        _estimatedFee = value;
        //        RaisePropertyChanged(nameof(EstimatedFee));
        //    }
        //}

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
                UpdatePercentBudget();
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
                ComboSubFieldState = !_editSubFieldState;

                RaisePropertyChanged(nameof(EditSubFieldState));
            }
        }

        private bool _comboSubFieldState;
        public bool ComboSubFieldState
        {
            get { return _comboSubFieldState; }
            set
            {
                _comboSubFieldState = value;
                RaisePropertyChanged(nameof(ComboSubFieldState));
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

        public SubProjectModel()
        {
            onstartup = false;
            canupdatepercentbudget = true;
            canupdatefee = true;
        }

        public SubProjectModel(SubProjectDbModel spm)
        {
            Constructor(spm);
        }

        public SubProjectModel(SubProjectDbModel spm, double totalfee)
        {
            TotalFee = totalfee;
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
            canupdatepercentbudget = true;
            canupdatefee = true;
        }

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
                IsInvoiced = IsActive ? 1 : 0,
                PercentComplete = PercentComplete,
                PercentBudget = PercentBudget,
            };

            SQLAccess.UpdateSubProject(subproject);
        }

        public void UpdatePercentBudget()
        {
            if (!onstartup)
            {
                if (canupdatefee)
                {
                    canupdatefee = false;
                    PercentBudget = (Fee / TotalFee) * 100;
                    canupdatepercentbudget = true;
                }
            }
        }

        public void UpdateFee()
        {
            if (!onstartup)
            {
                if (canupdatepercentbudget)
                {
                    canupdatepercentbudget = false;
                    Fee = (_percentBudget / 100) * TotalFee;
                    canupdatefee = true;
                }
            }
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
