using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public enum PhaseEnum
    {
        [Description("DNA")]
        DNA = 0,

        [Description("Adservice")]
        AD = 1,

        [Description("PreSD")]
        PSD = 2,

        [Description("SD")]
        SD = 3,

        [Description("DD")]
        DD = 4,

        [Description("CD")]
        CD = 5,

        [Description("CA")]
        CA = 6,
    }
}
