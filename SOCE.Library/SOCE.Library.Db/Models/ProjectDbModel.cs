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
        public string Client { get; set; }
        public double Fee { get; set; }  
    }
}
