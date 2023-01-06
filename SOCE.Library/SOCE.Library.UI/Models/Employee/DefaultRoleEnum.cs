using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public enum DefaultRoleEnum
    {
        [Description("BIM Technician")]
        BT = 0,

        [Description("Project Manager")]
        PM = 1,

        [Description("Project Engineer")]
        PE = 2,

        [Description("QC Reviewer")]
        QC = 3,

        [Description("Administration")]
        AD = 4,
    }
}
