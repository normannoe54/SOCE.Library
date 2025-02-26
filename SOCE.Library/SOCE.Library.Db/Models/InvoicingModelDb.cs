﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class InvoicingModelDb
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int InvoiceNumber { get; set; }
        public int Date { get; set; }
        public string ClientName { get; set; }
        public string ClientCompany { get; set; }
        public string ClientAddress { get; set; }
        public string ClientCity { get; set; }
        public double PreviousSpent { get; set; }

        public double AmountDue { get; set; }
        public int EmployeeSignedId { get; set; }
        public string TimesheetIds { get; set; }

        public int AddServicesDate { get; set; }

        public string ExpenseReportIds { get; set; }

        public string Link { get; set; }

        public int IsLogged { get; set; }
        public int IsRevised { get; set; }
        public double ExpensesPrevious { get; set; }
        public double ExpensesDue { get; set; }

        public int ExpensePreviousDate { get; set; }


    }
}
