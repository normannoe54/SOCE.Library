using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public enum AddServiceLogStatusEnum
    {
        [Description("Complete")]
        Complete = 0,

        [Description("Incomplete")]
        Incomplete = 1,

        [Description("Missing")]
        Missing = 2,
    }
}
