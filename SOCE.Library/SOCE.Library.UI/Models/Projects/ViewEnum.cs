using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum ViewEnum
    {
        [Description("Project Summary")]
        ProjectSummary,

        [Description("Add Service")]
        AddService,

        [Description("Timeline")]
        Timeline,
    }
}
