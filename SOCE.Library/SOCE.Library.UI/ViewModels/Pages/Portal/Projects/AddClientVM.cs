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


        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }


        private int _clientNumberInp;
        public int ClientNumberInp
        {
            get { return _clientNumberInp; }
            set
            {
                _clientNumberInp = value;
                RaisePropertyChanged("ClientNumberInp");
            }
        }

        public ICommand AddClientCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddClientVM()
        {
            ButtonInAction = true;
            this.AddClientCommand = new RelayCommand(this.AddClient);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddClient()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            ClientDbModel client = new ClientDbModel()
            { ClientName = ClientNameInp, ClientNumber = ClientNumberInp, IsActive = 1};

             int id = SQLAccess.AddClient(client);

            if (id == 0)
            {
                ErrorMessage = $"Client number exists already";
                ButtonInAction = true;
                return;
            }


            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            DialogHost.Close("RootDialog");
        }
    }
}
