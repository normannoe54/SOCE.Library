using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum ProposalStatusEnum
    {
        [Description("Approved")]
        Approved = 0,

        [Description("Denied")]
        Denied = 1,

        [Description("Pending")]
        Pending = 2,
    }
}
