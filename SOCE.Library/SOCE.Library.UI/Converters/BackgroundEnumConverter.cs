using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class BackgroundEnumConverter : IValueConverter
    {
        private readonly Color _deletedColor = (Color)ColorConverter.ConvertFromString("#6B0000");
        private readonly Color _approvedColor = (Color)ColorConverter.ConvertFromString("#015F15");
        private readonly Color _pendingColor = (Color)ColorConverter.ConvertFromString("#000F6B");


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProposalStatusEnum tras = (ProposalStatusEnum)value;

            switch (tras)
            {
                case ProposalStatusEnum.Pending:
                    {
                        return new SolidColorBrush(_pendingColor);
                    }
                case ProposalStatusEnum.Approved:
                    {
                        return new SolidColorBrush(_approvedColor);
                    }
                case ProposalStatusEnum.Denied:
                    {
                        return new SolidColorBrush(_deletedColor);
                    }
                default:
                    {
                        return new SolidColorBrush(_pendingColor);
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
