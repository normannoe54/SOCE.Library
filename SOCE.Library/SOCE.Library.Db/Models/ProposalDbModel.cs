using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class ProposalDbModel
    {
        public int Id { get; set; }
        public string ProposalName { get; set; }
        public int Status { get; set; }
        public double Fee { get; set; }
        public int ClientId { get; set; }
        public int MarketId { get; set; }
        public int SenderId { get; set; }
        public int? DateSent { get; set; }
        public double CostMetricValue { get; set; }
        public string Remarks { get; set; }

        public string MiscClient { get; set; }
        public string LinkFolder { get; set; }
        public string CostMetric { get; set; }

    }
}
