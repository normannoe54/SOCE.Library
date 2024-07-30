using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class TimesheetSubmissionDbModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Date { get; set; }
        public double TotalHours { get; set; }
        public double PTOHours { get; set; }
        public double OTHours { get; set; }
        public double SickHours { get; set; }
        public double HolidayHours { get; set; }
        public int Approved { get; set; }

        public double ExpensesCost { get; set; }
    }
}
