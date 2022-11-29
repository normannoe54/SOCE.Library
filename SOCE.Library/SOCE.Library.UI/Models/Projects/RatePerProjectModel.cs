using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class RatePerProjectModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private ProjectModel _project { get; set; }
        public ProjectModel Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        private EmployeeModel _employee { get; set; }
        public EmployeeModel Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                RaisePropertyChanged(nameof(Employee));
            }
        }

        private double _rate { get; set; }
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                TotalBudget = TotalHours * Rate;
                PercentofBudget = Math.Round((TotalBudget / OverallFee) * 100, 2);
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private double _percentofBudget { get; set; }
        public double PercentofBudget
        {
            get
            {
                return _percentofBudget;
            }
            set
            {
                _percentofBudget = value;
                RaisePropertyChanged(nameof(PercentofBudget));
            }
        }

        private double _overallFee { get; set; }
        public double OverallFee
        {
            get
            {
                return _overallFee;
            }
            set
            {
                _overallFee = value;
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

        private double _totalBudget { get; set; }
        public double TotalBudget
        {
            get
            {
                return _totalBudget;
            }
            set
            {
                _totalBudget = value;
                RaisePropertyChanged(nameof(TotalBudget));
            }
        }


        private bool _editEmployeeRateState = true;
        public bool EditEmployeeRateState
        {
            get { return _editEmployeeRateState; }
            set
            {
                if (!_editEmployeeRateState && value)
                {
                    UpdateRatePerProject();
                    Project.FormatData(false);
                }
                _editEmployeeRateState = value;

                RaisePropertyChanged(nameof(EditEmployeeRateState));
            }
        }
        public RatePerProjectModel()
        { }

        public RatePerProjectModel(int id, double rate, int employeeId, ProjectModel project, double totalhours, double overallFee)
        {
            Id = id;
            Employee = new EmployeeModel(SQLAccess.LoadEmployeeById(employeeId));
            Project = project;
            Rate = rate;
            OverallFee = overallFee;
            TotalHours = totalhours;
            TotalBudget = totalhours * Rate;
            PercentofBudget = Math.Round((TotalBudget / OverallFee) * 100, 2);
        }

        public void UpdateRatePerProject()
        {
            RatesPerProjectDbModel rpp = new RatesPerProjectDbModel()
            {
                Id = Id,
                EmployeeId = Employee.Id,
                ProjectId = Project.Id,
                Rate = Rate
            };

            SQLAccess.UpdateRatesPerProject(rpp);
        }

        //public override bool Equals(object obj)
        //{
        //    MarketModel em = (MarketModel)obj;

        //    if (em == null)
        //    {
        //        return false;
        //    }

        //    return em != null && Id == em.Id && MarketName == em.MarketName;
        //}

        //public override int GetHashCode()
        //{
        //    unchecked // Overflow is fine, just wrap
        //    {
        //        int hash = 17;
        //        // Suitable nullity checks etc, of course :)
        //        hash = hash * 23 + Id.GetHashCode();
        //        hash = hash * 23 + MarketName.GetHashCode();
        //        return hash;
        //    }
        //}

    }
}
