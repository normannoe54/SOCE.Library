using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SOCE.Library.UI
{
    public class TimesheetTextBox : TextBox
    {
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            double val;
            bool isnum = Double.TryParse(this.Text, out val);

            if (string.IsNullOrEmpty(this.Text) || !isnum)
            {
                this.Text = "0";
            }

            var control = this as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }
    }
}
