using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class ClientLowResModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private string _clientName { get; set; }
        public string ClientName
        {
            get
            {
                return _clientName;
            }
            set
            {
                _clientName = value;
                RaisePropertyChanged(nameof(ClientName));
            }
        }

        private int _clientNumber { get; set; }
        public int ClientNumber
        {
            get
            {
                return _clientNumber;
            }
            set
            {
                _clientNumber = value;
                RaisePropertyChanged(nameof(ClientNumber));
            }
        }

        public ClientLowResModel()
        { }

        public ClientLowResModel(ClientDbModel cdbm)
        {
            Id = cdbm.Id;
            ClientName = cdbm.ClientName;
            ClientNumber = cdbm.ClientNumber;

        }

    }
}
