using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class ProjectFormatResultDb
    {
        public ProjectDbModel Project { get; set; }

        public SubProjectDbModel SelectedSubProject { get; set; }

        public List<SubProjectDbModel> SubProjects { get; set; }
    }
}
