using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AddClientVM : BaseVM
    {
        private string _clientNameInp;
        public string ClientNameInp
        {
            get { return _clientNameInp; }
            set
            {
                _clientNameInp = value;
                RaisePropertyChanged("ClientNameInp");
            }
        }

        public ICommand AddClientCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddClientVM()
        {
            this.AddClientCommand = new RelayCommand(this.AddClient);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddClient()
        {
            ClientDbModel client = new ClientDbModel()
            { ClientName = ClientNameInp};

            SQLAccess.AddClient(client);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
