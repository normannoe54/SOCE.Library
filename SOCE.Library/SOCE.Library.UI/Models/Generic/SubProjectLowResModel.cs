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
    public class SubProjectLowResModel : PropertyChangedBase, ICloneable
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
                RaisePropertyChanged(nameof(IsScheduleActive));
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
                RaisePropertyChanged(nameof(HoursUsed));
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

        //public double BudgetSpent;

        public SubProjectLowResModel()
        {
        }


        public SubProjectLowResModel(SubProjectDbModel spm)
        {
            Constructor(spm);
        }


        public void Constructor(SubProjectDbModel spm)
        {
            Id = spm.Id;
            ProjectNumber = spm.ProjectId;
            PointNumber = spm.PointNumber;
            Description = spm.Description;
            IsActive = Convert.ToBoolean(spm.IsActive);
            IsScheduleActive = Convert.ToBoolean(spm.IsScheduleActive);
            NumberOrder = spm.NumberOrder;
        }

        public object Clone()
        {
            return new SubProjectLowResModel()
            {
                Id = this.Id,
                ProjectNumber = this.ProjectNumber,
                PointNumber = this.PointNumber,
                Description = this.Description,
                IsActive = this.IsActive,
                IsScheduleActive = this.IsScheduleActive,
                NumberOrder = this.NumberOrder
            };
        }

    }
}
