using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace SOCE.Library.UI.ViewModels
{
    public class AddProposalsVM : BaseVM
    {
        public bool result = false;

        private string _proposalsNameInp;
        public string ProposalsNameInp
        {
            get { return _proposalsNameInp; }
            set
            {
                _proposalsNameInp = value;
                RaisePropertyChanged(nameof(ProposalsNameInp));
            }
        }

        private DateTime? _dateSentInp;
        public DateTime? DateSentInp
        {
            get { return _dateSentInp; }
            set
            {
                _dateSentInp = value;
                RaisePropertyChanged(nameof(DateSentInp));
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

        private ObservableCollection<EmployeeLowResModel> _pMsAvailable = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> PMsAvailable
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

                if (_clientInp.ClientName.ToUpper() == "MISCELLANEOUS")
                {
                    IsClientInputVisible = Visibility.Visible;
                }
                else
                {
                    IsClientInputVisible = Visibility.Collapsed;
                }

                //collect all and find next iteration
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
                CostMetricInp = (_marketInp.MarketName.ToUpper() == "MULTI-FAMILY") ? "unit" : "sq.ft.";
                RaisePropertyChanged("MarketInp");
            }
        }

        private EmployeeLowResModel _pMInp;
        public EmployeeLowResModel PMInp
        {
            get { return _pMInp; }
            set
            {
                _pMInp = value;
                RaisePropertyChanged("PMInp");
            }
        }

        private ProposalStatusEnum _proposalStatusInp = ProposalStatusEnum.Pending;
        public ProposalStatusEnum ProposalStatusInp
        {
            get { return _proposalStatusInp; }
            set
            {
                _proposalStatusInp = value;
                RaisePropertyChanged("ProposalStatusInp");
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

        private double _costMetricValueInp = 0;
        public double CostMetricValueInp
        {
            get { return _costMetricValueInp; }
            set
            {
                _costMetricValueInp = value;
                RaisePropertyChanged("CostMetricValueInp");
            }
        }

        private string _costMetricInp;
        public string CostMetricInp
        {
            get { return _costMetricInp; }
            set
            {
                _costMetricInp = value;
                RaisePropertyChanged("CostMetricInp");
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

        private string _remarksInp = "";
        public string RemarksInp
        {
            get { return _remarksInp; }
            set
            {
                _remarksInp = value;
                RaisePropertyChanged("RemarksInp");
            }
        }

        private string _folderLinkInp = "";
        public string FolderLinkInp
        {
            get { return _folderLinkInp; }
            set
            {
                _folderLinkInp = value;
                RaisePropertyChanged("FolderLinkInp");
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

        public ICommand AddProposalCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        ProposalsVM propsumvm;
        public AddProposalsVM(EmployeeModel employee, ProposalsVM propvm)
        {
            propsumvm = propvm;
            MarketsAvailable.Clear();
            ClientsAvailable.Clear();
            this.OpenFolderCommand = new RelayCommand(this.SelectLinkFolder); 
            this.AddProposalCommand = new RelayCommand(this.AddProposal);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            List<MarketDbModel> Markets = SQLAccess.LoadMarkets();
            List<ClientDbModel> Clients = SQLAccess.LoadClients();
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();
            ObservableCollection<MarketModel> TotalMark = new ObservableCollection<MarketModel>();
            ObservableCollection<ClientModel> TotalClients = new ObservableCollection<ClientModel>();
            ObservableCollection<EmployeeLowResModel> TotalPMs = new ObservableCollection<EmployeeLowResModel>();

            foreach (MarketDbModel mark in Markets)
            {
                TotalMark.Add(new MarketModel(mark));
            }
            List<MarketModel> newmarkets = TotalMark.OrderBy(x => x.MarketName).ToList();
            MarketsAvailable = new ObservableCollection<MarketModel>(newmarkets);

            foreach (ClientDbModel client in Clients)
            {
                TotalClients.Add(new ClientModel(client));
            }
            List<ClientModel> newclients = TotalClients.OrderBy(x => x.ClientName).ToList();
            ClientsAvailable = new ObservableCollection<ClientModel>(newclients);

            foreach (EmployeeDbModel pm in PMs)
            {
                TotalPMs.Add(new EmployeeLowResModel(pm));
            }
            List<EmployeeLowResModel> newpms = TotalPMs.OrderBy(x => x.FullName).ToList();
            PMsAvailable = new ObservableCollection<EmployeeLowResModel>(newpms);

            CostMetricInp = "sq.ft.";
            EmployeeLowResModel selectem = PMsAvailable.Where(x => x.Id == employee.Id).FirstOrDefault();
            
            if (selectem != null)
            {
                PMInp = selectem;
            }
            ProposalStatusInp = ProposalStatusEnum.Pending;
            DateSentInp = DateTime.Now;
        }


        public void AddProposal()
        {
            if (String.IsNullOrEmpty(ProposalsNameInp) || ClientInp == null || MarketInp == null || PMInp == null)
            {
                ErrorMessage = $"Double check that all inputs have been {Environment.NewLine}filled out correctly and try again.";
                return;
            }

            int? duedatevar = null;

            if (DateSentInp != null)
            {
                duedatevar = (int)long.Parse(DateSentInp?.ToString("yyyyMMdd"));
            }

            ProposalDbModel proposal = new ProposalDbModel
            {
                ProposalName = ProposalsNameInp,
                ClientId = ClientInp.Id,
                MarketId = MarketInp.Id,
                SenderId = PMInp.Id,
                Fee = TotalFeeInp,
                DateSent = duedatevar,
                Status = (int)ProposalStatusInp,
                CostMetricValue = CostMetricValueInp,
                CostMetric = CostMetricInp,
                MiscClient = MiscClientInput,
                Remarks = RemarksInp,
                LinkFolder = FolderLinkInp
            };

            int id = SQLAccess.AddProposal(proposal);

            //if (id == 0)
            //{
            //    ErrorMessage = $"Project number exists{Environment.NewLine}check inactive projects.";
            //    return;
            //}
            propsumvm.Reload();
            //update view models?
            //BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            //PortalAI portAI = (PortalAI)CurrentPage;
            //await portAI.RefreshViews();

            CloseWindow();
        }

        public void SelectLinkFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                FolderLinkInp = dialog.FileName;
            }
        }


        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
