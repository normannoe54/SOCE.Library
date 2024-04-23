using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class InvoicingRowsDb
    {
        public int Id { get; set; }

        public int SubId { get; set; }
        public int InvoiceId { get; set; }
        public double PercentComplete { get; set; }
        public double PreviousInvoiced { get; set; }
        public double ThisPeriodInvoiced { get; set; }
        public string ScopeName { get; set; }

    }
}
