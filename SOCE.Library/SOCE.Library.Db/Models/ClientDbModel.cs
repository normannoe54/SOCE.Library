﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.Db
{
    public class ClientDbModel
    {
        public int Id { get; set; }

        public int ClientNumber { get; set; }
        public string ClientName { get; set; }
        public string NameOfClient { get; set; }

        public string ClientAddress { get; set; }

        public string ClientCity { get; set; }
        public int IsActive { get; set; }
    }
}
