using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public class DateWrapper : PropertyChangedBase
    {
        private DateTime _value;
        public DateTime Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public DateWrapper(DateTime value)
        {
            Value = value;
        }

    }
}
