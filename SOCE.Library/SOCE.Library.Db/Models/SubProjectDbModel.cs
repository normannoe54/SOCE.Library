using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.Db
{
    public class SubProjectDbModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string PointNumber { get; set; }
        public string Description { get; set; }
        public double Fee { get; set; }
        public int IsActive { get; set; }

        public double PercentComplete { get; set; }
        public double PercentBudget { get; set; }
        public int IsInvoiced { get; set; }

        public string ExpandedDescription { get; set; }

        public int IsAdservice { get; set; }
        public int NumberOrder { get; set; }

    }
}
