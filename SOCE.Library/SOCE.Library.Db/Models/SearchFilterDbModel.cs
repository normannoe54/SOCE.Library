using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class SearchFilterDbModel
    {
        public int Id { get; set; }

        public string Header { get; set; }
        public string FolderPath { get; set; }
        public int NumberOrder { get; set; }

        public int Active { get; set; }

        public int SearchFileType { get; set; }
        public int EmployeeId { get; set; }
        public int SubLayer { get; set; }
    }
}
