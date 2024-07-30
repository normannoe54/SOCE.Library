using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class ExpenseReportDbModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public int Date { get; set; }
        public int TypeExpense { get; set; }
        public double TotalCost { get; set; }
        public double Mileage { get; set; }
        public double MileageRate { get; set; }
        public int Invoiced { get; set; }
        public int IsClientBillable { get; set; }
        public int Reimbursable { get; set; }
        public int IsCustom { get; set; }
    }
}
