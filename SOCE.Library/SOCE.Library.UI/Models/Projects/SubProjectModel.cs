using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class SubProjectModel : ICloneable
    {
        public int Id { get; set; }
        public int ProjectNumber { get; set; }
        public int PointNumber { get; set; }
        public string Description { get; set; }
        public double Fee { get; set; }

        public string PointNumStr
        {
            get
            {
                string jobnumstr = "";
                //if (ProjectNumber != null)
                //{
                jobnumstr = $"[.{PointNumber.ToString()}]";
                //}
                return jobnumstr;
            }
        }

        public SubProjectModel()
        { }

        public SubProjectModel(SubProjectDbModel spm)
        {
            Id = spm.Id;
            ProjectNumber = spm.ProjectId;
            PointNumber = spm.PointNumber;
            Description = spm.Description;
            Fee = spm.Fee;
        }

        public object Clone()
        {
            return new SubProjectModel() { Id = this.Id, ProjectNumber = this.ProjectNumber, PointNumber = this.PointNumber, Description = this.Description, Fee = this.Fee };
        }
    }
}
