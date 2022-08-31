using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum DataTypeEnum
    {
        [Description("Total Hours")]
        TotalHours = 0,

        [Description("Total Budget")]
        TotalBudget = 1,

        [Description("Incremental Hours")]
        IncHours = 2,

        [Description("Incremental Budget")]
        IncBudget = 3,
    }
}
