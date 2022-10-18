using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private double _timeentry = 0;
        public double TimeEntry
        {
            get
            {
                return _timeentry;
            }
            set
            {
                _timeentry = value;
                //SetTotal();
                RaisePropertyChanged(nameof(TimeEntry));
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

        public object Clone()
        {
            return new TREntryModel() { Date = this.Date, TimeEntry = this.TimeEntry, Id = this.Id };
        }

    }
}
