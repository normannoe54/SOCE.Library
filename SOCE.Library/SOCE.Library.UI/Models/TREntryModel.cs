using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public class TREntryModel : PropertyChangedBase
    {
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
    }
}
