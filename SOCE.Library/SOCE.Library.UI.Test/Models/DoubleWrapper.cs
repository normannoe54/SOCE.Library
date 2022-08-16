using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public class DoubleWrapper : PropertyChangedBase
    {
        private double _value = 0;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                //SetTotal();
                RaisePropertyChanged(nameof(Value));
            }
        }


        public DoubleWrapper(double value)
        {
            Value = value;
        }

    }
}
