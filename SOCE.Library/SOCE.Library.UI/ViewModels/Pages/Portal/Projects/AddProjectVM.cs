using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;

namespace SOCE.Library.UI.ViewModels
{
    public class AddProjectVM : BaseVM
    {
        private string _projectNameInp;
        public string ProjectNameInp
        {
            get { return _projectNameInp; }
            set
            {
                _projectNameInp = value;
                RaisePropertyChanged("ProjectNameInp");
            }
        }

        private int _projectNumberInp;
        public int ProjectNumberInp
        {
            get { return _projectNumberInp; }
            set
            {
                _projectNumberInp = value;
                RaisePropertyChanged("ProjectNumberInp");
            }
        }

        private ObservableCollection<ClientModel> _clientsAvailable = new ObservableCollection<ClientModel>();
        public ObservableCollection<ClientModel> ClientsAvailable
        {
            get { return _clientsAvailable; }
            set
            {
                _clientsAvailable = value;
                RaisePropertyChanged(nameof(ClientsAvailable));
            }
        }

        private ObservableCollection<MarketModel> _marketsAvailable = new ObservableCollection<MarketModel>();
        public ObservableCollection<MarketModel> MarketsAvailable
        {
            get { return _marketsAvailable; }
            set
            {
                _marketsAvailable = value;
                RaisePropertyChanged(nameof(MarketsAvailable));
            }
        }

        private ObservableCollection<EmployeeModel> _pMsAvailable = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> PMsAvailable
        {
            get { return _pMsAvailable; }
            set
            {
                _pMsAvailable = value;
                RaisePropertyChanged(nameof(PMsAvailable));
            }
        }

        private ClientModel _clientInp;
        public ClientModel ClientInp
        {
            get { return _clientInp; }
            set
            {
                _clientInp = value;
                RaisePropertyChanged("ClientInp");
            }
        }

        private MarketModel _marketInp;
        public MarketModel MarketInp
        {
            get { return _marketInp; }
            set
            {
                _marketInp = value;
                RaisePropertyChanged("MarketInp");
            }
        }

        private EmployeeModel _pMInp;
        public EmployeeModel PMInp
        {
            get { return _pMInp; }
            set
            {
                _pMInp = value;
                RaisePropertyChanged("PMInp");
            }
        }

        private double _totalFeeInp;
        public double TotalFeeInp
        {
            get { return _totalFeeInp; }
            set
            {
                _totalFeeInp = value;
                RaisePropertyChanged("TotalFeeInp");
            }
        }

        public ICommand AddProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddProjectVM()
        {
            MarketsAvailable.Clear();
            ClientsAvailable.Clear();

            this.AddProjectCommand = new RelayCommand(this.AddProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            List<MarketDbModel> Markets = SQLAccess.LoadMarkets();
            List<ClientDbModel> Clients = SQLAccess.LoadClients();
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();
            ObservableCollection<MarketModel> TotalMark = new ObservableCollection<MarketModel>();
            ObservableCollection<ClientModel> TotalClients = new ObservableCollection<ClientModel>();
            ObservableCollection<EmployeeModel> TotalPMs = new ObservableCollection<EmployeeModel>();

            foreach (MarketDbModel mark in Markets)
            {
                TotalMark.Add(new MarketModel(mark));
            }

            foreach (ClientDbModel client in Clients)
            {
                TotalClients.Add(new ClientModel(client));
            }

            foreach (EmployeeDbModel pm in PMs)
            {
                TotalPMs.Add(new EmployeeModel(pm));
            }

            MarketsAvailable = TotalMark;
            ClientsAvailable = TotalClients;
            PMsAvailable = TotalPMs;

        }

        public void AddProject()
        {
            ProjectDbModel project = new ProjectDbModel
            {
                ProjectName = ProjectNameInp,
                ProjectNumber = ProjectNumberInp,
                ClientId = ClientInp.Id,
                MarketId = MarketInp.Id,
                ManagerId = PMInp.Id,
                Fee = TotalFeeInp,
                PercentComplete = 0,
                IsActive = 1,
                Projectfolder = "",
                Drawingsfolder = "",
                Architectfolder = "",
                Plotfolder = "",

            };

            SQLAccess.AddProject(project);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
