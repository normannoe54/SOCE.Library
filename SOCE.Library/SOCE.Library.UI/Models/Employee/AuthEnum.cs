using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public enum AuthEnum
    {
        [Description("Other")]
        Other = 0,

        [Description("Administrator")]
        Admin = 1,

        [Description("Principal")]
        Principal = 2,

        [Description("Project Manager")]
        PM = 3,

        
    }
}
