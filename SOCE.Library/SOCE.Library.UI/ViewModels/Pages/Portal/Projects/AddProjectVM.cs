using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;

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

        private ObservableCollection<BudgetEstimateView> _roles = new ObservableCollection<BudgetEstimateView>();
        public ObservableCollection<BudgetEstimateView> Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                RaisePropertyChanged(nameof(Roles));
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

        private bool _cLDevPhase;
        public bool CLDevPhase
        {
            get { return _cLDevPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || CDPhase || PPhase || COPhase || InvPhase)))
                {
                    _cLDevPhase = value;

                    if (_cLDevPhase)
                    {
                        Roles.Add(CLDevView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(CLDevView);
                    }

                    RaisePropertyChanged("CLDevView");
                }
            }
        }


        private bool _cDPhase;
        public bool CDPhase
        {
            get { return _cDPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || InvPhase || PPhase || COPhase || CLDevPhase)))
                {
                    _cDPhase = value;

                    if (_cDPhase)
                    {
                        Roles.Add(CDView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(CDView);
                    }

                    RaisePropertyChanged("CDPhase");
                }
            }
        }

        private bool _sDPhase;
        public bool SDPhase
        {
            get { return _sDPhase; }
            set
            {
                if (value || (!value && (CAPhase || InvPhase || DDPhase || CDPhase || PPhase || COPhase || CLDevPhase)))
                {
                    _sDPhase = value;

                    if (_sDPhase)
                    {
                        Roles.Add(SDView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(SDView);
                    }

                    RaisePropertyChanged("SDPhase");
                }
            }
        }

        private bool _dDPhase;
        public bool DDPhase
        {
            get { return _dDPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || InvPhase || CDPhase || PPhase || COPhase || CLDevPhase)))
                {
                    _dDPhase = value;

                    if (_dDPhase)
                    {
                        Roles.Add(DDView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(DDView);
                    }

                    RaisePropertyChanged("DDPhase");
                }
            }
        }

        private bool _cAPhase;
        public bool CAPhase
        {
            get { return _cAPhase; }
            set
            {
                if (value || (!value && (InvPhase || SDPhase || DDPhase || CDPhase || PPhase || COPhase || CLDevPhase)))
                {
                    _cAPhase = value;
                    //Add CA Phase
                    if (_cAPhase)
                    {
                        Roles.Add(CAView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(CAView);
                    }

                    //UpdateFees();
                    RaisePropertyChanged("CAPhase");
                }
            }
        }

        private bool _pPhase;
        public bool PPhase
        {
            get { return _pPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || CDPhase || InvPhase || COPhase || CLDevPhase)))
                {
                    _pPhase = value;
                    if (_pPhase)
                    {
                        Roles.Add(PropView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(PropView);
                    }


                    RaisePropertyChanged("PPhase");
                }
            }
        }

        private bool _invPhase;
        public bool InvPhase
        {
            get { return _invPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || CDPhase || PPhase || COPhase || CLDevPhase)))
                {
                    _invPhase = value;
                    if (_invPhase)
                    {
                        Roles.Add(InvestigationView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(InvestigationView);
                    }


                    RaisePropertyChanged("InvPhase");
                }
            }
        }

        private bool _cOPhase;
        public bool COPhase
        {
            get { return _cOPhase; }
            set
            {
                if (value || (!value && (CAPhase || SDPhase || DDPhase || CDPhase || PPhase || InvPhase || CLDevPhase)))
                {
                    _cOPhase = value;
                    if (_cOPhase)
                    {
                        Roles.Add(ConstrObvView);
                    }
                    else
                    {
                        //remove from list
                        Roles.Remove(ConstrObvView);
                    }


                    RaisePropertyChanged("COPhase");
                }
            }
        }

        public ICommand AddProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private BudgetEstimateView CLDevView;
        private BudgetEstimateView PropView;
        private BudgetEstimateView SDView;
        private BudgetEstimateView DDView;
        private BudgetEstimateView CDView;
        private BudgetEstimateView CAView;
        private BudgetEstimateView InvestigationView;
        private BudgetEstimateView ConstrObvView;

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

            SetSubs();
            CAPhase = true;
            CDPhase = true;
            MarketsAvailable = TotalMark;
            ClientsAvailable = TotalClients;
            PMsAvailable = TotalPMs;

        }
        private void UpdateFee()
        {

        }


        private void SetSubs()
        {
            CLDevView = new BudgetEstimateView();
            SubProjectModel clientdevproj = new SubProjectModel
            {
                PointNumber = "CLDev",
                Description = "Client Developement",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM CLDevVM = new BudgetEstimateVM(clientdevproj, 0);
            CLDevView.DataContext = CLDevVM;

            PropView = new BudgetEstimateView();
            SubProjectModel proposalproj = new SubProjectModel
            {
                PointNumber = "Prop",
                Description = "Proposal",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM PropVM = new BudgetEstimateVM(proposalproj, 0);
            PropView.DataContext = PropVM;

            SDView = new BudgetEstimateView();
            SubProjectModel sdproj = new SubProjectModel
            {
                PointNumber = "SD",
                Description = "Schematic Design",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM SDVM = new BudgetEstimateVM(sdproj, 0);
            SDView.DataContext = SDVM;

            DDView = new BudgetEstimateView();
            SubProjectModel ddproj = new SubProjectModel
            {
                PointNumber = "DD",
                Description = "Design Developement",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM DDVM = new BudgetEstimateVM(ddproj, 0);
            DDView.DataContext = DDVM;

            CDView = new BudgetEstimateView();
            SubProjectModel cdproj = new SubProjectModel
            {
                PointNumber = "CD",
                Description = "Construction Document",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM CDVM = new BudgetEstimateVM(cdproj, 0);
            CDView.DataContext = CDVM;

            CAView = new BudgetEstimateView();
            SubProjectModel caproj = new SubProjectModel
            {
                PointNumber = "CA",
                Description = "Construction Administration",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM CAVM = new BudgetEstimateVM(caproj, 0);
            CAView.DataContext = CAVM;

            InvestigationView = new BudgetEstimateView();
            SubProjectModel investigationproj = new SubProjectModel
            {
                PointNumber = "I",
                Description = "Investigation",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM InvestigationVM = new BudgetEstimateVM(investigationproj, 0);
            InvestigationView.DataContext = InvestigationVM;

            ConstrObvView = new BudgetEstimateView();
            SubProjectModel constrobsproj = new SubProjectModel
            {
                PointNumber = "CO",
                Description = "Construction Observation",
                IsActive = true,
                IsInvoiced = false,
                PercentComplete = 0,
                PercentBudget = 0,
            };
            BudgetEstimateVM COVM = new BudgetEstimateVM(constrobsproj, 0);
            ConstrObvView.DataContext = COVM;
        }


        public void AddProject()
        {
            if (String.IsNullOrEmpty(ProjectNameInp) || ProjectNumberInp == 0 || ClientInp == null || MarketInp == null || PMInp == null)
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

            //count += CDPhase ? 1 : 0;
            //count += CAPhase ? 1 : 0;
            //count += PPhase ? 1 : 0;
            //count += SDPhase ? 1 : 0;
            //count += DDPhase ? 1 : 0;


            //if (CDPhase && id !=0)
            //{
            //    SubProjectDbModel cdproj = new SubProjectDbModel
            //    {
            //        ProjectId = id,
            //        PointNumber = "CD",
            //        Description = "Construction Document Phase",
            //        Fee = (1/count) * TotalFeeInp,
            //        IsActive = 1,
            //        IsCurrActive = 1,
            //        IsInvoiced = 0,
            //        PercentComplete = 0,
            //        PercentBudget = (1 / count)*100,
            //    };
            //    SQLAccess.AddSubProject(cdproj);
            //}

            //if (CAPhase && id != 0)
            //{
            //    SubProjectDbModel caproj = new SubProjectDbModel
            //    {
            //        ProjectId = id,
            //        PointNumber = "CA",
            //        Description = "Construction Administration Phase",
            //        Fee = (1 / count) * TotalFeeInp,
            //        IsActive = 1,
            //        IsCurrActive = 1,
            //        IsInvoiced = 0,
            //        PercentComplete = 0,
            //        PercentBudget = (1 / count) * 100,
            //    };
            //    SQLAccess.AddSubProject(caproj);
            //}

            //if (PPhase && id != 0)
            //{
            //    SubProjectDbModel pproj = new SubProjectDbModel
            //    {
            //        ProjectId = id,
            //        PointNumber = "Pre",
            //        Description = "Proposal Phase",
            //        Fee = (1 / count) * TotalFeeInp,
            //        IsActive = 1,
            //        IsCurrActive = 1,
            //        IsInvoiced = 0,
            //        PercentComplete = 0,
            //        PercentBudget = (1 / count) * 100,
            //    };
            //    SQLAccess.AddSubProject(pproj);
            //}

            //if (SDPhase && id != 0)
            //{
            //    SubProjectDbModel pproj = new SubProjectDbModel
            //    {
            //        ProjectId = id,
            //        PointNumber = "SD",
            //        Description = "Schematic Design",
            //        Fee = (1 / count) * TotalFeeInp,
            //        IsActive = 1,
            //        IsCurrActive = 1,
            //        IsInvoiced = 0,
            //        PercentComplete = 0,
            //        PercentBudget = (1 / count) * 100,
            //    };
            //    SQLAccess.AddSubProject(pproj);
            //}

            //if (DDPhase && id != 0)
            //{
            //    SubProjectDbModel pproj = new SubProjectDbModel
            //    {
            //        ProjectId = id,
            //        PointNumber = "DD",
            //        Description = "Design Developement",
            //        Fee = (1 / count) * TotalFeeInp,
            //        IsActive = 1,
            //        IsCurrActive = 1,
            //        IsInvoiced = 0,
            //        PercentComplete = 0,
            //        PercentBudget = (1 / count) * 100,
            //    };
            //    SQLAccess.AddSubProject(pproj);
            //}

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
