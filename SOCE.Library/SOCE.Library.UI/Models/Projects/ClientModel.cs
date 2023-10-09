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

        private string _nameOfClient { get; set; }
        public string NameOfClient
        {
            get
            {
                return _nameOfClient;
            }
            set
            {
                _nameOfClient = value;
                RaisePropertyChanged(nameof(NameOfClient));
            }
        }

        private string _clientAddress { get; set; }
        public string ClientAddress
        {
            get
            {
                return _clientAddress;
            }
            set
            {
                _clientAddress = value;
                RaisePropertyChanged(nameof(ClientAddress));
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

        private bool _makingChanges = false;
        public bool MakingChanges
        {
            get { return _makingChanges; }
            set
            {
                if (_makingChanges && !value)
                {
                    UpdateClient();
                }
                _makingChanges = value;

                RaisePropertyChanged(nameof(MakingChanges));
            }
        }

        public ClientModel()
        { }

        public ClientModel(ClientDbModel cdbm)
        {
            Id = cdbm.Id;
            ClientName = cdbm.ClientName;
            ClientNumber = cdbm.ClientNumber;
            ClientAddress = cdbm.ClientAddress;
            NameOfClient = cdbm.NameOfClient;
        }

        public void UpdateClient()
        {
            ClientDbModel client = new ClientDbModel()
            {
                Id = Id,
                ClientName = ClientName,
                ClientNumber = ClientNumber,
                ClientAddress = ClientAddress,
                NameOfClient = NameOfClient
            };

            SQLAccess.UpdateClient(client);
        }

        public override bool Equals(object obj)
        {
            ClientModel cm = (ClientModel)obj;

            if (cm == null)
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
