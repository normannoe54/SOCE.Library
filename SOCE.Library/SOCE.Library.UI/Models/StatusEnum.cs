using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum StatusEnum
    {
        [Description("Incomplete")]
        Incomplete = 0,

        [Description("Submitted")]
        Submitted = 1,

        [Description("Approved")]
        Approved = 2,
    }
}
