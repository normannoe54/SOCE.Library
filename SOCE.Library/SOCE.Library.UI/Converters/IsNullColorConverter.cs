using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class IsNullColorConverter : IValueConverter
    {
        private readonly Color _nullcolor = (Color)ColorConverter.ConvertFromString("#C92127");
        private readonly Color _nonnullcolor = (Color)ColorConverter.ConvertFromString("#1D9719");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return new SolidColorBrush(_nullcolor);
            }
            else
            {
                return new SolidColorBrush(_nonnullcolor);
            }

            //return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
