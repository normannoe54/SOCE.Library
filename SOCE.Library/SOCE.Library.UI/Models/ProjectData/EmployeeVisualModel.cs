using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeVisualModel : PropertyChangedBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string _truncatedName;
        public string TruncatedName
        {
            get { return _truncatedName; }
            set
            {
                _truncatedName = value;
                RaisePropertyChanged(nameof(TruncatedName));
            }
        }

        private SolidColorBrush _visualColor;
        public SolidColorBrush VisualColor
        {
            get { return _visualColor; }
            set
            {
                _visualColor = value;
                RaisePropertyChanged(nameof(VisualColor));
            }
        }

        private double _sumHours;
        public double SumHours
        {
            get { return _sumHours; }
            set
            {
                _sumHours = value;
                RaisePropertyChanged(nameof(SumHours));
            }
        }

        private double _rate;
        public double Rate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private bool _selectedCurr = true;
        public bool SelectedCurr
        {
            get { return _selectedCurr; }
            set
            {
                _selectedCurr = value;
                RaisePropertyChanged(nameof(SelectedCurr));
            }
        }

        public EmployeeVisualModel()
        { }

        public EmployeeVisualModel(EmployeeDbModel employeedbmodel)
        {
            Name = employeedbmodel.FullName;
            TruncatedName = employeedbmodel.FirstName;
            Rate = employeedbmodel.Rate;
        }

    }
}
