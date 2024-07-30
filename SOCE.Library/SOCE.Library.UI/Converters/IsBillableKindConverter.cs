using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class IsBillableKindConverter : IValueConverter
    {
        private readonly PackIconKind _notbillable = PackIconKind.CurrencyUsdOff;
        private readonly PackIconKind _isbillable = PackIconKind.CurrencyUsd;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == false)
                {
                    return _notbillable;
                }
                else
                {
                    return _isbillable;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
