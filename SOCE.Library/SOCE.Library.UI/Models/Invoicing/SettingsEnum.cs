using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum SettingsEnum
    {
        [Description("Show All")]
        All,

        [Description("Uninvoiced hours and fee not met")]
        UninvoicedHoursAndFeeNotMet,

        [Description("Uninvoiced hours")]
        UnInvoicedHours,
    }
}
