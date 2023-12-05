using SOCE.Library.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TREntryModel : PropertyChangedBase, ICloneable
    {
        private int _id = 0;
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

        private bool _readOnly = true;
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
                RaisePropertyChanged(nameof(ReadOnly));
            }
        }



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

        public TimesheetVM timesheetvm;

        private double _timeentry = 0;
        public double TimeEntry
        {
            get
            {
                return _timeentry;
            }
            set
            {
                if (_timeentry != value)
                {
                    _timeentry = value;

                    if (timesheetvm != null)
                    {
                        timesheetvm.SumTable();

                        foreach (TimesheetRowModel trm in timesheetvm.Rowdata)
                        {
                            trm.Total = trm.Entries.Sum(x => x.TimeEntry);
                        }

                        timesheetvm.AutoSave();
                    }

                    //if (_timeEntryString != _timeentry.ToString())
                    //{
                    //    TimeEntryString = _timeentry.ToString();
                    //}

                    RaisePropertyChanged(nameof(TimeEntry));
                }
            }
        }

        //private string _timeEntryString = "0";
        //public string TimeEntryString
        //{
        //    get
        //    {
        //        return _timeEntryString;
        //    }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            double num = 0;
        //            bool isnumeric = double.TryParse(value, out num);

        //            if (isnumeric)
        //            {
        //                _timeEntryString = value;

        //                if (_timeEntryString != _timeentry.ToString())
        //                {
        //                    _timeEntryString = "0";
        //                    TimeEntry = Convert.ToDouble(_timeEntryString);

        //                }
        //            }
        //            else
        //            {
        //                _timeEntryString = "0";
        //                TimeEntry = 0;
        //                TimeEntryString = "0";
        //            }
        //            RaisePropertyChanged(nameof(TimeEntryString));
        //        }

        //        RaisePropertyChanged(nameof(TimeEntryString));

        //    }
        //}

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

        public object Clone()
        {
            return new TREntryModel() { Date = this.Date, TimeEntry = this.TimeEntry, Id = this.Id };
        }

    }
}
