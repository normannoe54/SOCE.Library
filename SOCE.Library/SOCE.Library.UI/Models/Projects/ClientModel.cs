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

        private bool _editFieldClientState = true;
        public bool EditFieldClientState
        {
            get { return _editFieldClientState; }
            set
            {
                if (!_editFieldClientState && value)
                {
                    UpdateClient();
                }
                _editFieldClientState = value;

                RaisePropertyChanged(nameof(EditFieldClientState));
            }
        }

        public ClientModel()
        { }

        public ClientModel(ClientDbModel cdbm)
        {
            Id = cdbm.Id;
            ClientName = cdbm.ClientName;

        }

        public void UpdateClient()
        {
            ClientDbModel client = new ClientDbModel()
            {
                Id = Id,
                ClientName = ClientName,
            };

            SQLAccess.UpdateClient(client);
        }
    }
}
