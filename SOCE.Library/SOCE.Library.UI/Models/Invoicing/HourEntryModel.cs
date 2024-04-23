using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public class HourEntryModel : PropertyChangedBase
    {
        public int SubId;
        public int TimeId;

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                if (_date.DayOfWeek == DayOfWeek.Saturday || _date.DayOfWeek == DayOfWeek.Sunday)
                {
                    CellColor = Brushes.LightGray;
                }
                else
                {
                    CellColor = Brushes.White;
                }
                RaisePropertyChanged(nameof(Date));
            }
        }

        private bool _invoiced = false;
        public bool Invoiced
        {
            get
            {
                return _invoiced;
            }
            set
            {
                _invoiced = value;
                RaisePropertyChanged(nameof(Invoiced));
            }
        }

        private bool _canHit = false;
        public bool CanHit
        {
            get
            {
                return _canHit;
            }
            set
            {
                _canHit = value;
                RaisePropertyChanged(nameof(CanHit));
            }
        }

        private bool _isSelectedForInvoicing = false;
        public bool IsSelectedForInvoicing
        {
            get
            {
                return _isSelectedForInvoicing;
            }
            set
            {
                _isSelectedForInvoicing = value;
                BackgroundColor = _isSelectedForInvoicing ? Brushes.PaleVioletRed : Brushes.Transparent;
                RaisePropertyChanged(nameof(IsSelectedForInvoicing));
            }
        }

        private double _timeentry = 0;
        public double TimeEntry
        {
            get
            {
                return _timeentry;
            }
            set
            {
                //if (_timeentry != value)
                //{
                _timeentry = value;

                if (_timeentry > 0)
                {
                    CanHit = true;
                }
                //if (basevm != null)
                //{
                //    basevm.UpdateTotals();
                //}
                //if (_timeEntryString != _timeentry.ToString())
                //{
                //    TimeEntryString = _timeentry.ToString();
                //}

                RaisePropertyChanged(nameof(TimeEntry));
                //}
            }
        }

        private double _budgetSpent = 0;
        public double BudgetSpent
        {
            get
            {
                return _budgetSpent;
            }
            set
            {
                //if (_timeentry != value)
                //{
                _budgetSpent = value;
                RaisePropertyChanged(nameof(BudgetSpent));
                //}
            }
        }

        //public ScheduleWeekControlVM basevm;

        private Brush _backgroundColor;
        public Brush BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        private Brush _cellColor;
        public Brush CellColor
        {
            get
            {
                return _cellColor;
            }
            set
            {
                _cellColor = value;
                //SetTotal();
                RaisePropertyChanged(nameof(CellColor));
            }
        }

        //private Brush _cellColor;
        //public Brush CellColor
        //{
        //    get
        //    {
        //        return _cellColor;
        //    }
        //    set
        //    {
        //        _cellColor = value;
        //        //SetTotal();
        //        RaisePropertyChanged(nameof(CellColor));
        //    }
        //}

    }
}
