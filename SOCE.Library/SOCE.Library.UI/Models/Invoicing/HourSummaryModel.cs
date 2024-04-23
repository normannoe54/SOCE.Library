using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class HourSummaryModel : PropertyChangedBase
    {
        private string _phaseName { get; set; }
        public string PhaseName
        {
            get
            {
                return _phaseName;
            }
            set
            {
                _phaseName = value;
                RaisePropertyChanged(nameof(PhaseName));
            }
        }

        public int PhaseId { get; set; }

        private ObservableCollection<HourSummaryIndModel> _employeeSummary = new ObservableCollection<HourSummaryIndModel>();
        public ObservableCollection<HourSummaryIndModel> EmployeeSummary
        {
            get
            {
                return _employeeSummary;
            }
            set
            {
                _employeeSummary = value;
                RaisePropertyChanged(nameof(EmployeeSummary));
            }
        }

        private double _outstandingHours { get; set; }
        public double OutstandingHours
        {
            get
            {
                return _outstandingHours;
            }
            set
            {
                _outstandingHours = value;
                RaisePropertyChanged(nameof(OutstandingHours));
            }
        }

        private DateTime? _dateOfPrevious { get; set; }
        public DateTime? DateOfPrevious
        {
            get
            {
                return _dateOfPrevious;
            }
            set
            {
                _dateOfPrevious = value;
                RaisePropertyChanged(nameof(DateOfPrevious));
            }
        }

        private double _selectedHours { get; set; }
        public double SelectedHours
        {
            get
            {
                return _selectedHours;
            }
            set
            {
                _selectedHours = value;
                RaisePropertyChanged(nameof(SelectedHours));
            }
        }

        private bool _hoursSelectableVis { get; set; }
        public bool HoursSelectableVis
        {
            get
            {
                return _hoursSelectableVis;
            }
            set
            {
                _hoursSelectableVis = value;
                RaisePropertyChanged(nameof(HoursSelectableVis));
            }
        }
    }
}
