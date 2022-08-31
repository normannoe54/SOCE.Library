using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum TimeOptionEnum
    {
        [Description("Yearly")]
        Yearly = 0,

        [Description("Monthly")]
        Monthly = 1,

        [Description("All Time")]
        AllTime = 2,
    }
}
