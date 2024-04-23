using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace SOCE.Library.UI
{
    public class FontWeightConverter : IValueConverter
    {
        private readonly FontWeight _regular = FontWeights.Regular;
        private readonly FontWeight _bold = FontWeights.DemiBold;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == false)
                {
                    return _regular;
                }
                else
                {
                    return _bold;
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
