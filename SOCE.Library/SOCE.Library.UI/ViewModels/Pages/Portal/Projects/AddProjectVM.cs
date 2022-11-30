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
        public bool result = false;
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

                if (_clientInp.ClientName.ToUpper() == "MISC")
                {
                    IsClientInputVisible = Visibility.Visible;
                }
                else
                {
                    IsClientInputVisible = Visibility.Collapsed;
                }
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

        private string _miscClientInput = "";
        public string MiscClientInput
        {
            get { return _miscClientInput; }
            set
            {
                _miscClientInput = value;
                RaisePropertyChanged("MiscClientInput");
            }
        }

        private Visibility _isClientInputVisible = Visibility.Collapsed;
        public Visibility IsClientInputVisible
        {
            get { return _isClientInputVisible; }
            set
            {
                _isClientInputVisible = value;
                RaisePropertyChanged("IsClientInputVisible");
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

        private bool _cDPhase = true;
        public bool CDPhase
        {
            get { return _cDPhase; }
            set
            {
                if (value || (!value && (CAPhase || PPhase || DDPhase || SDPhase)))
                {
                    _cDPhase = value;
                    RaisePropertyChanged("CDPhase");
                }
            }
        }

        private bool _sDPhase = true;
        public bool SDPhase
        {
            get { return _sDPhase; }
            set
            {
                if (value || (!value && (CAPhase || PPhase || DDPhase || CDPhase)))
                {
                    _sDPhase = value;
                    RaisePropertyChanged("SDPhase");
                }
            }
        }

        private bool _dDPhase = true;
        public bool DDPhase
        {
            get { return _dDPhase; }
            set
            {
                if (value || (!value && (CAPhase || PPhase || SDPhase || CDPhase)))
                {
                    _dDPhase = value;
                    RaisePropertyChanged("DDPhase");
                }
            }
        }

        private bool _cAPhase = true;
        public bool CAPhase
        {
            get { return _cAPhase; }
            set
            {
                if (value || (!value && (SDPhase || PPhase || DDPhase || CDPhase)))
                {
                    _cAPhase = value;
                    RaisePropertyChanged("CAPhase");
                }
            }
        }

        private bool _pPhase = false;
        public bool PPhase
        {
            get { return _pPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || CDPhase)))
                {
                    _pPhase = value;
                    RaisePropertyChanged("PPhase");
                }
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
            if(String.IsNullOrEmpty(ProjectNameInp) || ProjectNumberInp == 0 || ClientInp == null || MarketInp ==null || PMInp == null )
            {
                ErrorMessage = $"Double check that all inputs have been {Environment.NewLine}filled out correctly and try again.";
                return;
            }

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
                IsCurrActive = 1,
                Projectfolder = "",
                Drawingsfolder = "",
                Architectfolder = "",
                Plotfolder = "",
                ProjectStart = (int)long.Parse(DateTime.Now.ToString("yyyyMMdd")),
                MiscName = MiscClientInput
            };

            int id = SQLAccess.AddProject(project);

            double count = 0;

            count += CDPhase ? 1 : 0;
            count += CAPhase ? 1 : 0;
            count += PPhase ? 1 : 0;
            count += SDPhase ? 1 : 0;
            count += DDPhase ? 1 : 0;


            if (CDPhase && id !=0)
            {
                SubProjectDbModel cdproj = new SubProjectDbModel
                {
                    ProjectId = id,
                    PointNumber = "CD",
                    Description = "Construction Document Phase",
                    Fee = (1/count) * TotalFeeInp,
                    IsActive = 1,
                    IsCurrActive = 1,
                    IsInvoiced = 0,
                    PercentComplete = 0,
                    PercentBudget = (1 / count)*100,
                };
                SQLAccess.AddSubProject(cdproj);
            }

            if (CAPhase && id != 0)
            {
                SubProjectDbModel caproj = new SubProjectDbModel
                {
                    ProjectId = id,
                    PointNumber = "CA",
                    Description = "Construction Administration Phase",
                    Fee = (1 / count) * TotalFeeInp,
                    IsActive = 1,
                    IsCurrActive = 1,
                    IsInvoiced = 0,
                    PercentComplete = 0,
                    PercentBudget = (1 / count) * 100,
                };
                SQLAccess.AddSubProject(caproj);
            }

            if (PPhase && id != 0)
            {
                SubProjectDbModel pproj = new SubProjectDbModel
                {
                    ProjectId = id,
                    PointNumber = "Pre",
                    Description = "Proposal Phase",
                    Fee = (1 / count) * TotalFeeInp,
                    IsActive = 1,
                    IsCurrActive = 1,
                    IsInvoiced = 0,
                    PercentComplete = 0,
                    PercentBudget = (1 / count) * 100,
                };
                SQLAccess.AddSubProject(pproj);
            }

            if (SDPhase && id != 0)
            {
                SubProjectDbModel pproj = new SubProjectDbModel
                {
                    ProjectId = id,
                    PointNumber = "SD",
                    Description = "Schematic Design",
                    Fee = (1 / count) * TotalFeeInp,
                    IsActive = 1,
                    IsCurrActive = 1,
                    IsInvoiced = 0,
                    PercentComplete = 0,
                    PercentBudget = (1 / count) * 100,
                };
                SQLAccess.AddSubProject(pproj);
            }

            if (DDPhase && id != 0)
            {
                SubProjectDbModel pproj = new SubProjectDbModel
                {
                    ProjectId = id,
                    PointNumber = "DD",
                    Description = "Design Developement",
                    Fee = (1 / count) * TotalFeeInp,
                    IsActive = 1,
                    IsCurrActive = 1,
                    IsInvoiced = 0,
                    PercentComplete = 0,
                    PercentBudget = (1 / count) * 100,
                };
                SQLAccess.AddSubProject(pproj);
            }

            result = true;

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
