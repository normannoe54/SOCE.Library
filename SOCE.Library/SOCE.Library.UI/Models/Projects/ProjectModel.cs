using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class ProjectModel: PropertyChangedBase
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int ProjectNumber { get; set; }
        public string Client { get; set; }
        public double Fee { get; set; }



        public string JobNumStr
        {
            get
            {
                string jobnumstr = $"[{ProjectNumber.ToString()}]";
                return jobnumstr;
            }
        }

        public ProjectModel()
        { }

        public ProjectModel(ProjectDbModel pm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            Client = pm.Client;
            Fee = pm.Fee;
        }

    }
}
