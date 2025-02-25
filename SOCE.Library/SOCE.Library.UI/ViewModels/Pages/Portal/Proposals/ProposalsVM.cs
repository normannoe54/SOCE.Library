using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class ProposalsVM : BaseVM
    {
        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }
        public ICommand GoToAddProposal { get; set; }
        public ICommand ArchiveProposal { get; set; }
        public ICommand GoToAddMarket { get; set; }
        public ICommand ClearComboBox { get; set; }
        public ICommand ClearSearchParamters { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportDataCommand { get; set; }
        public ICommand GoToOpenProposalSummary { get; set; }
        //public ICommand OpenClientCommand { get; set; }
        //public ICommand OpenMarketCommand { get; set; }
        //public ICommand AddYearCommand { get; set; }

        private bool _sortFilter = false;
        public bool SortFilter
        {
            get
            {
                return _sortFilter;
            }
            set
            {
                _sortFilter = value;
                SortProposals(_sortFilter);
                RaisePropertyChanged(nameof(SortFilter));
            }
        }

        private ObservableCollection<ProposalViewResModel> _proposals = new ObservableCollection<ProposalViewResModel>();
        public ObservableCollection<ProposalViewResModel> Proposals
        {
            get { return _proposals; }
            set
            {
                _proposals = value;
                RaisePropertyChanged(nameof(Proposals));
            }
        }

        private DateTime _startDate { get; set; }
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                if (!onstart)
                {
                    Reload();
                }
                RaisePropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _endDate { get; set; }
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                if (!onstart)
                {
                    Reload();
                }
                RaisePropertyChanged(nameof(EndDate));
            }
        }

        private ProposalStatusEnum? _selectedStatus;
        public ProposalStatusEnum? SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                RaisePropertyChanged(nameof(SelectedStatus));
            }
        }

        private EmployeeLowResModel _selectedPM;
        public EmployeeLowResModel SelectedPM
        {
            get { return _selectedPM; }
            set
            {
                _selectedPM = value;
                RaisePropertyChanged(nameof(SelectedPM));
            }
        }

        private MarketModel _selectedMarket;
        public MarketModel SelectedMarket
        {
            get { return _selectedMarket; }
            set
            {
                _selectedMarket = value;
                RaisePropertyChanged(nameof(SelectedMarket));
            }
        }

        private ClientModel _selectedClient;
        public ClientModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                RaisePropertyChanged(nameof(SelectedClient));
            }
        }

        private List<ProposalViewResModel> AllProposals = new List<ProposalViewResModel>();

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

        private ObservableCollection<MarketModel> _markets = new ObservableCollection<MarketModel>();
        public ObservableCollection<MarketModel> Markets
        {
            get { return _markets; }
            set
            {
                _markets = value;
                RaisePropertyChanged(nameof(Markets));
            }
        }

        private ObservableCollection<EmployeeLowResModel> _projectManagers = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> ProjectManagers
        {
            get { return _projectManagers; }
            set
            {
                _projectManagers = value;
                RaisePropertyChanged(nameof(ProjectManagers));
            }
        }

        private int? _yearInp = 2022;
        public int? YearInp
        {
            get { return _yearInp; }
            set
            {
                _yearInp = value;
                RaisePropertyChanged(nameof(YearInp));
            }
        }

        private bool _isEditable = false;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }

        private int _dateTimeSelection { get; set; }
        public int DateTimeSelection
        {
            get { return _dateTimeSelection; }
            set
            {
                _dateTimeSelection = value;
                RaisePropertyChanged(nameof(DateTimeSelection));
            }
        }

        private string _searchableText { get; set; }
        public string SearchableText
        {
            get { return _searchableText; }
            set
            {
                _searchableText = value;
                RaisePropertyChanged(nameof(SearchableText));
            }
        }

        public bool onstart;

        public ProposalsVM(EmployeeModel loggedinEmployee)
        {
            onstart = true;
            CurrentEmployee = loggedinEmployee;
            this.GoToOpenProposalSummary = new RelayCommand<object>(this.ExecuteOpenSubDialog);
            this.GoToAddProposal = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.ArchiveProposal = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.ExportDataCommand = new RelayCommand(this.RunExport);
            //this.AddYearCommand = new RelayCommand(this.AddToYear);
            //this.SubtractYearCommand = new RelayCommand(this.SubtractToYear);
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            //this.ExportDataCommand = new RelayCommand(this.RunExport);

            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            StartDate = firstDay;
            EndDate = lastDay;

            LoadClients();
            LoadMarkets();
            LoadProjectManagers();
            LoadProposals();

            onstart = false;
        }

        public void Reload()
        {
            LoadProjectManagers();
            LoadProposals();
        }

        private async void ClearInputsandReload()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchableText = "";
                SelectedMarket = null;
                SelectedClient = null;
                SelectedPM = null;

                Proposals = new ObservableCollection<ProposalViewResModel>(AllProposals);
                SortProposals(SortFilter);

            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }


        private async void RunExport()
        {
            TimeSpan time = TimeSpan.FromSeconds(Proposals.Count * 0.1);
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();

            aysvm.Message = $"Are you sure you want to export {Proposals.Count} proposals?";
            aysvm.SubMessage = $"Estimated time to complete: {time.Minutes} min. {time.Seconds} sec.";
            view.DataContext = aysvm;

            //show the dialog
            var Result = await DialogHost.Show(view, "RootDialog");

            YesNoVM vm = view.DataContext as YesNoVM;
            bool resultvm = vm.Result;

            if (resultvm)
            {
                ExportConfirmProposalsView ecv = new ExportConfirmProposalsView();
                ExportConfirmProposalVM ecvm = new ExportConfirmProposalVM(Proposals.ToList(), StartDate, EndDate);
                //show progress bar and do stuff
                ecv.DataContext = ecvm;
                var newres = await DialogHost.Show(ecv, "RootDialog");
            }
        }

        private async void RunSearch()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                List<ProposalViewResModel> pmnew = AllProposals;

                if (SelectedPM != null)
                {
                    pmnew = pmnew.Where(x => x.Sender?.Id == SelectedPM.Id).ToList();
                }

                if (SelectedClient != null)
                {
                    pmnew = pmnew.Where(x => x.Client.Id == SelectedClient.Id).ToList();
                }

                if (SelectedMarket != null)
                {
                    pmnew = pmnew.Where(x => x.Market.Id == SelectedMarket.Id).ToList();
                }

                if (SelectedStatus != null)
                {
                    pmnew = pmnew.Where(x => x.Status == SelectedStatus).ToList();
                }

                
                if (!String.IsNullOrEmpty(SearchableText))
                {
                    pmnew = pmnew.Where(x => x.ProposalName.ToUpper().Contains(_searchableText.ToUpper())).ToList();
                }

                //if (!ShowActiveProjects && YearInp != null)
                //{
                //    string yearinpstr = YearInp.ToString();
                //    pmnew = pmnew.Where(x => x.ProjectNumber.ToString().Substring(0, 2) == yearinpstr.Substring(yearinpstr.Length - 2)).ToList();
                //}

                Proposals = new ObservableCollection<ProposalViewResModel>(pmnew);
                SortProposals(SortFilter);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private async void ExecuteOpenSubDialog(object o)
        {
            ProposalViewResModel pm = (ProposalViewResModel)o;
            var view = new ProposalSummaryView();
            ProposalSummaryVM vm = new ProposalSummaryVM(CurrentEmployee, pm);
            view.DataContext = vm;
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
        }


        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddProposalsView();
            view.DataContext = new AddProposalsVM(CurrentEmployee, this);
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            AddProposalsVM vm = view.DataContext as AddProposalsVM;
            bool resultvm = vm.result;

            if (resultvm)
            {
                LoadProposals();
            }
        }

        private void SortProposals(bool sort)
        {
            if (sort)
            {
                Proposals = new ObservableCollection<ProposalViewResModel>(Proposals.OrderBy(x => x.DateSent).ToList());
            }
            else
            {
                Proposals = new ObservableCollection<ProposalViewResModel>(Proposals.OrderByDescending(x => x.DateSent).ToList());
            }

        }
        //private async void ExecuteRunAddClientDialog(object o)
        //{
        //    //let's set up a little MVVM, cos that's what the cool kids are doing:
        //    var view = new AddClientView();

        //    //show the dialog
        //    var result = await DialogHost.Show(view, "RootDialog");
        //    //if (result != null)
        //    //{
        //        LoadClients();
        //    //}
        //}


        private async void ExecuteRunDeleteDialog(object o)
        {
            ProposalViewResModel pm = (ProposalViewResModel)o;

            YesNoView ynv = new YesNoView();
            YesNoVM ynvm = new YesNoVM(pm);
            ynv.DataContext = ynvm;

            var result2 = await DialogHost.Show(ynv, "RootDialog");
            ynvm = ynv.DataContext as YesNoVM;

            if (ynvm.Result)
            {
                SQLAccess.DeleteProposal(pm.Id);

                LoadProposals();
            }
        }

        public void LoadProposals()
        {
            DateTime mindate = (StartDate < EndDate ? StartDate : EndDate);
            DateTime maxdate = (StartDate > EndDate ? StartDate : EndDate);

            List<ProposalDbModel> dbprojects = SQLAccess.LoadProposalsBydates(mindate, maxdate);

            ObservableCollection<ProposalViewResModel> members = new ObservableCollection<ProposalViewResModel>();

            ProposalViewResModel[] ProjectArray = new ProposalViewResModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProposalDbModel pdb = dbprojects[i];

                //if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
                //{
                ProposalViewResModel pm = new ProposalViewResModel(pdb);
                EmployeeLowResModel em = ProjectManagers.Where(x => x.Id == pdb.SenderId).FirstOrDefault();
                ClientModel cm = Clients.Where(x => x.Id == pdb.ClientId).FirstOrDefault();
                MarketModel mm = Markets.Where(x => x.Id == pdb.MarketId).FirstOrDefault();

                pm.Sender = em;
                pm.Client = cm;
                pm.Market = mm;


                ProjectArray[i] = pm;
                //}
            }
            );

            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            AllProposals = ProjectArray.ToList();
            Proposals = new ObservableCollection<ProposalViewResModel>(ProjectArray.ToList());
        }

        public void LoadClients()
        {
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();

            List<ClientModel> clients = new List<ClientModel>();
            foreach (ClientDbModel cdbm in dbclients)
            {
                clients.Add(new ClientModel(cdbm));
            }

            List<ClientModel> newclients = clients.OrderBy(x => x.ClientName).ToList();
            Clients = new ObservableCollection<ClientModel>(newclients);
        }

        private void AddToYear()
        {
            YearInp++;
        }

        private void SubtractToYear()
        {
            YearInp--;
        }

        public void LoadMarkets()
        {
            List<MarketDbModel> dbmarkets = SQLAccess.LoadMarkets();

            ObservableCollection<MarketModel> members = new ObservableCollection<MarketModel>();

            foreach (MarketDbModel mdbm in dbmarkets)
            {
                members.Add(new MarketModel(mdbm));
            }

            List<MarketModel> newmarkets = members.OrderBy(x => x.MarketName).ToList();
            Markets = new ObservableCollection<MarketModel>(newmarkets);
        }


        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeLowResModel> members = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeLowResModel(edbm));
            }

            List<EmployeeLowResModel> newemployees = members.OrderBy(x => x.FullName).ToList();
            ProjectManagers = new ObservableCollection<EmployeeLowResModel>(newemployees);
        }
    }
}
