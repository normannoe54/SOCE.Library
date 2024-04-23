using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOCE.Library.Db;

namespace SOCE.Library.DbBatch
{
    public class MarketFoundInfo
    {
        public MarketDbModel market { get; set; }

        public double Hours { get; set; }

        public double Budget { get; set; }
    }

    public class ClientFoundInfo
    {
        public ClientDbModel client { get; set; }

        public double Hours { get; set; }

        public double Budget { get; set; }
    }
}
