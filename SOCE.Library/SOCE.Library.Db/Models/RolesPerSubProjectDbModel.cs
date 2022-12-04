using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class RolePerSubProjectDbModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int SubProjectId { get; set; }
        public double Rate { get; set; }
        public int Role { get; set; }
        public double BudgetHours { get; set; }

    }
}