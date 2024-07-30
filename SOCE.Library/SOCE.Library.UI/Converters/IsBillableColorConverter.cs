using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class IsBillableColorConverter : IValueConverter
    {
        private readonly Color _zerocolor = (Color)ColorConverter.ConvertFromString("#898B8D");
        private readonly Color _nonzerocolor = (Color)ColorConverter.ConvertFromString("#ffffff");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == false)
                {
                    return new SolidColorBrush(_zerocolor);
                }
                else
                {
                    return new SolidColorBrush(_nonzerocolor);
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
