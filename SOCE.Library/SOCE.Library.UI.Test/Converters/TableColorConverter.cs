using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TableColorConverter : IValueConverter
    {
        private readonly Color _zerocolor = Colors.LightGray;
        private readonly Color _nonzerocolor = Colors.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is double)
            {
                if ((double)value == 0 )
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
