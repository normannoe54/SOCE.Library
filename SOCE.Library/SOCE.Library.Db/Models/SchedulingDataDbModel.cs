using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.Db
{
    public class SchedulingDataDbModel
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }
        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }


        //public int RoleId { get; set; }
        public int Date { get; set; }
        public double Hours1 { get; set; }
        public double Hours2 { get; set; }
        public double Hours3 { get; set; }
        public double Hours4 { get; set; }
        public double Hours5 { get; set; }
        public double Hours6 { get; set; }
        public double Hours7 { get; set; }
        public double Hours8 { get; set; }
        public int EmployeeId { get; set; }

        public int ProjectNumber { get; set; }
        public int ManagerId { get; set; }
    }
}
