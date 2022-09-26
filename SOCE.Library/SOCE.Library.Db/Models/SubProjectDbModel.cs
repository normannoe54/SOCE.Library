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
        public int Fee { get; set; }
    }
}
