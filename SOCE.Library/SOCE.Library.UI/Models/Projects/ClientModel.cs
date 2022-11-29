using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class ClientModel : PropertyChangedBase
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

        private bool _editFieldClientsState = true;
        public bool EditFieldClientsState
        {
            get { return _editFieldClientsState; }
            set
            {
                if (!_editFieldClientsState && value)
                {
                    UpdateClient();
                }
                _editFieldClientsState = value;

                RaisePropertyChanged(nameof(EditFieldClientsState));
            }
        }

        public ClientModel()
        { }

        public ClientModel(ClientDbModel cdbm)
        {
            Id = cdbm.Id;
            ClientName = cdbm.ClientName;
            ClientNumber = cdbm.ClientNumber;
        }

        public void UpdateClient()
        {
            ClientDbModel client = new ClientDbModel()
            {
                Id = Id,
                ClientName = ClientName,
                ClientNumber = ClientNumber
            };

            SQLAccess.UpdateClient(client);
        }

        public override bool Equals(object obj)
        {
            ClientModel cm = (ClientModel)obj;

            if (cm== null)
            {
                return false;
            }

            return cm != null && Id == cm.Id && ClientName == cm.ClientName && ClientNumber == cm.ClientNumber;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + ClientName.GetHashCode();
                hash = hash * 23 + ClientNumber.GetHashCode();
                return hash;
            }
        }
    }
}
