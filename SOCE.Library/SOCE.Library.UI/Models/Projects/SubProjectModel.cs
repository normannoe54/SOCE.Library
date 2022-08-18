using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class SubProjectDbModel
    {
        public int Id { get; set; }
        public double ProjectNumber { get; set; }
        public string Description { get; set; }
        public double Fee { get; set; }


        public SubProjectDbModel()
        { }

        //public SubProjectDbModel(SubProjectDbModel spm)
        //{
        //    Id = spm.Id;

            //ProjectNumber = pm.ProjectNumber;
            //Client = pm.Client;
            //TotalFee = pm.TotalFee;
        //}

    }
}
