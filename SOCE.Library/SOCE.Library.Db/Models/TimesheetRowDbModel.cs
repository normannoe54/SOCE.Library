using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class TimesheetRowDbModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Date { get; set; }
        public int SubProjectId { get; set; }

        public int ProjIdRef { get; set; }
        public double TimeEntry { get; set; }
        public double BudgetSpent { get; set; }
        public int Submitted { get; set; }
        public int Approved { get; set; }
        public int Invoiced { get; set; }
    }
}
