using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TimesheetColorAlertConverter : IValueConverter
    {
        private readonly Color _deletedColor = (Color)ColorConverter.ConvertFromString("#FFD7B5");
        private readonly Color _inactivecolor = (Color)ColorConverter.ConvertFromString("#F9AEAE");
        private readonly Color _activecolor = (Color)ColorConverter.ConvertFromString("#00FFFFFF");


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimesheetRowAlertStatus tras = (TimesheetRowAlertStatus)value;

            switch (tras)
            {
                case TimesheetRowAlertStatus.Deleted:
                    {
                        return new SolidColorBrush(_deletedColor);
                    }
                case TimesheetRowAlertStatus.Inactive:
                    {
                        return new SolidColorBrush(_inactivecolor);
                    }
                case TimesheetRowAlertStatus.Active:
                    {
                        return new SolidColorBrush(_activecolor);
                    }
                default:
                    {
                        return new SolidColorBrush(_activecolor);
                    }
            }

            //return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
