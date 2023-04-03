using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class ClientVM : BaseVM
    {

        private ObservableCollection<ClientModel> _clients = new ObservableCollection<ClientModel>();
        public ObservableCollection<ClientModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged(nameof(Clients));
            }
        }


        private bool _canAddProject = false;
        public bool CanAddProject
        {
            get
            {
                return _canAddProject;
            }
            set
            {
                _canAddProject = value;
                RaisePropertyChanged(nameof(CanAddProject));
            }
        }


        public ICommand GoToAddClient { get; set; }

        public ICommand GoBackCommand { get; set; }
        
        public ICommand DeleteClient { get; set; }

        private ProjectVM baseproject;
        public ClientVM( ProjectVM projectvm, bool canadd)
        {
            baseproject = projectvm;
            this.GoToAddClient = new RelayCommand<object>(this.ExecuteRunAddClientDialog);
            this.GoBackCommand = new RelayCommand(this.GoBack);
            this.DeleteClient = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            CanAddProject = canadd;
            LoadClients();
        }
        
        private void GoBack()
        {
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;

            portAI.RightDrawerOpen = false;
        }

        private async void ExecuteRunAddClientDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddClientView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            LoadClients();
            baseproject.LoadClients();
        }

        private void LoadClients()
        {
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();

            List<ClientModel> clients = new List<ClientModel>();
            foreach (ClientDbModel cdbm in dbclients)
            {
                clients.Add(new ClientModel(cdbm));
            }

            List<ClientModel> newclients = clients.OrderBy(x => x.ClientNumber).ToList();

            Clients = new ObservableCollection<ClientModel>(newclients);
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            ClientModel cm = (ClientModel)o;

            List<ProjectDbModel> project1 = SQLAccess.LoadProjects();
            List<ProjectDbModel> projclient = project1.Where(x => x.ClientId == cm.Id).ToList();

            if (projclient.Count > 0)
            {
                MessageBoxView mbv = new MessageBoxView();
                MessageBoxVM mbvm = new MessageBoxVM("Cannot delete client");
                mbvm.SubMessage = "client assigned to existing project";
                //donothing = true;

                mbv.DataContext = mbvm;
                var result = await DialogHost.Show(mbv, "RootDialog");
            }
            else
            {

                YesNoView ynv = new YesNoView();
                YesNoVM ynvm = new YesNoVM(cm);
                ynv.DataContext = ynvm;
                var result = await DialogHost.Show(ynv, "RootDialog");
                ynvm = ynv.DataContext as YesNoVM;
                if (ynvm.Result)
                {
                    SQLAccess.DeleteClient(cm.Id);
                    LoadClients();
                }
            }
        }
    }
}
