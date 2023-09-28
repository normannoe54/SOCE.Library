using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public enum ProjectScheduleViewEnum
    {
        [Description("Weekly Schedule")]
        WeeklySchedule,

        //[Description("Add Service")]
        //AddService,

        [Description("PM Report")]
        PMReport,

        [Description("Upcoming Schedule")]
        UpcomingSchedule,

        [Description("Project List")]
        ProjectList,
    }
}
