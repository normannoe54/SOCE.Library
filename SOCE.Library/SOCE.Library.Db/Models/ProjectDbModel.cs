using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class ProjectDbModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int ProjectNumber { get; set; }
        public int ClientId { get; set; }
        public double Fee { get; set; }

        public int MarketId { get; set; }

        public int ManagerId { get; set; }

        public int IsActive { get; set; }

        public double PercentComplete { get; set; }

        public string Projectfolder { get; set; }
        public string Drawingsfolder { get; set; }
        public string Architectfolder { get; set; }
        public string Plotfolder { get; set; }
    }
}
