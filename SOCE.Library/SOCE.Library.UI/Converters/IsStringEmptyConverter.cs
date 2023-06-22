using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class IsStringEmptyConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        private readonly Color _zerocolor = (Color)ColorConverter.ConvertFromString("#147975");
        private readonly Color _nonzerocolor = (Color)ColorConverter.ConvertFromString("#000000");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (String.IsNullOrEmpty((string)value))
                {
                    return new SolidColorBrush(_nonzerocolor);
                }
                else
                {
                    return new SolidColorBrush(_zerocolor);
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
