using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public class RegisteredTimesheetDataModel
    {
        public List<TimesheetRowModel> ProjectData;

        public bool Approved;

        public DateTime DateSubmitted;

        public DateTime PayPeriodDate;

        public DateEnum PayPeriodLocation;

    }
}
