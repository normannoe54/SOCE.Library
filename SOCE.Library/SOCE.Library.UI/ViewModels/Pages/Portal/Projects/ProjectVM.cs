using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectVM : BaseVM
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

                if (_currentEmployee.Status == AuthEnum.Admin || _currentEmployee.Status == AuthEnum.Principal || _currentEmployee.Status == AuthEnum.PM)
                {
                    CanAddProject = true;
                }

                if (!(_currentEmployee.Status == AuthEnum.Standard))
                {
                    IsEditable = true;
                }

                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }
        public ICommand GoToAddProject { get; set; }
        public ICommand GoToAddClient { get; set; }
        public ICommand GoToAddMarket { get; set; }
        //public ICommand GoToAddSubProject { get; set; }
        public ICommand ArchiveProject { get; set; }
        public ICommand ArchiveMarket { get; set; }
        public ICommand ArchiveClient { get; set; }
        public ICommand DeleteSubProject { get; set; }
        public ICommand ClearComboBox { get; set; }
        public ICommand ClearSearchParamters { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportDataCommand { get; set; }
        public ICommand GoToOpenProjectSummary { get; set; }

        public ICommand AddYearCommand { get; set; }

        public ICommand SubtractYearCommand { get; set; }

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
                SortProjects(_sortFilter);
                RaisePropertyChanged(nameof(SortFilter));
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

        private int _numProjects = 0;
        public int NumProjects
        {
            get { return _numProjects; }
            set
            {
                _numProjects = value;
                RaisePropertyChanged("NumProjects");
            }
        }

        private ObservableCollection<ProjectModel> _projects = new ObservableCollection<ProjectModel>();
        public ObservableCollection<ProjectModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                TextProjects = Projects.Count + " Total";
                RaisePropertyChanged(nameof(Projects));
            }
        }

        private EmployeeModel _selectedPM;
        public EmployeeModel SelectedPM
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

        private List<ProjectModel> AllProjects = new List<ProjectModel>();

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

        private ObservableCollection<EmployeeModel> _projectManagers = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> ProjectManagers
        {
            get { return _projectManagers; }
            set
            {
                _projectManagers = value;
                RaisePropertyChanged(nameof(ProjectManagers));
            }
        }

        private string _textProjects;
        public string TextProjects
        {
            get { return _textProjects; }
            set
            {
                _textProjects = value;
                RaisePropertyChanged(nameof(TextProjects));
            }
        }

        private int? _yearInp=2022;
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

        private bool _closeButtonVisible { get; set; } = false;
        public bool CloseButtonVisible
        {
            get { return _closeButtonVisible; }
            set
            {
                _closeButtonVisible = value;
                RaisePropertyChanged(nameof(CloseButtonVisible));
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
        

        private bool _showActiveProjects { get; set; } = true;
        public bool ShowActiveProjects
        {
            get { return _showActiveProjects; }
            set
            {
                _showActiveProjects = value;
                LoadProjects();
                Projects = new ObservableCollection<ProjectModel>(AllProjects);
                SortProjects(SortFilter);
                //if (_showActiveProjects)
                //{
                //    AllProjects = LoadProjects();
                //    Projects = new ObservableCollection<ProjectModel>(AllProjects.Where(x => x.IsActive == _showActiveProjects).ToList());
                //}
                //else
                //{
                //    AllProjects = 
                //    Projects = new ObservableCollection<ProjectModel>(AllProjects);
                //}

                RaisePropertyChanged(nameof(ShowActiveProjects));

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

        public ProjectVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;

            this.GoToAddProject = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.GoToAddClient = new RelayCommand<object>(this.ExecuteRunAddClientDialog);
            this.GoToAddMarket = new RelayCommand<object>(this.ExecuteRunAddMarketDialog);
            //this.GoToAddSubProject = new RelayCommand<object>(this.ExecuteRunAddSubProjectDialog);
            this.GoToOpenProjectSummary = new RelayCommand<object>(this.ExecuteOpenSubDialog);

            this.ArchiveProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.ArchiveClient = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.ArchiveMarket = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.DeleteSubProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);

            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.ExportDataCommand = new RelayCommand(this.RunExport);
            this.AddYearCommand = new RelayCommand(this.AddToYear);
            this.SubtractYearCommand = new RelayCommand(this.SubtractToYear);

            LoadClients();
            LoadMarkets();
            LoadProjectManagers();
            LoadProjects();


            ShowActiveProjects = true;
        }

        private void ClearInputsandReload()
        {
            SearchableText = "";
            SelectedMarket = null;
            SelectedClient = null;
            SelectedPM = null;

            if (!ShowActiveProjects)
            {
                if (YearInp != null)
                {
                    string yearinpstr = YearInp.ToString();
                    Projects = new ObservableCollection<ProjectModel>(AllProjects.Where(x => x.ProjectNumber.ToString().Substring(0, 2) == yearinpstr.Substring(yearinpstr.Length - 2)).ToList());
                }
                else
                {
                    Projects = new ObservableCollection<ProjectModel>(AllProjects);
                }
            }
            else
            {
                Projects = new ObservableCollection<ProjectModel>(AllProjects.Where(x=>x.IsActive == true).ToList());
            }
            SortProjects(SortFilter);
        }

        private void SortProjects(bool sort)
        {
            if (sort)
            {
                Projects = new ObservableCollection<ProjectModel>(Projects.OrderBy(x => x.ProjectNumber).ToList());
            }
            else
            {
                Projects = new ObservableCollection<ProjectModel>(Projects.OrderByDescending(x => x.ProjectNumber).ToList());
            }

        }

        private async void RunExport()
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM();

            aysvm.TopLine = $"Are you sure you want to export";
            aysvm.BottomLine = $"{Projects.Count} projects";
            view.DataContext = aysvm;

            //show the dialog
            var Result = await DialogHost.Show(view, "RootDialog");

            AreYouSureVM vm = view.DataContext as AreYouSureVM;
            bool resultvm = vm.Result;

            if (resultvm)
            {
                ExportConfirmView ecv = new ExportConfirmView();
                ExportConfirmVM ecvm = new ExportConfirmVM(Projects.ToList());
                //show progress bar and do stuff
                ecv.DataContext = ecvm;
                var newres = await DialogHost.Show(ecv, "RootDialog");
            }
        }

        private void RunSearch()
        {
            List<ProjectModel> pmnew = AllProjects;

            if (SelectedPM != null)
            {
                pmnew = pmnew.Where(x => x.ProjectManager.Id == SelectedPM.Id).ToList();
            }

            if (SelectedClient != null)
            {
                pmnew = pmnew.Where(x => x.Client.Id == SelectedClient.Id).ToList();
            }

            if (SelectedMarket != null)
            {
                pmnew = pmnew.Where(x => x.Market.Id == SelectedMarket.Id).ToList();
            }

            if (!String.IsNullOrEmpty(SearchableText))
            {
                pmnew = pmnew.Where(x => x.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || x.ProjectNumber.ToString().Contains(_searchableText)).ToList();
            }

            if (!ShowActiveProjects  && YearInp != null)
            {
                string yearinpstr = YearInp.ToString();
                pmnew = pmnew.Where(x => x.ProjectNumber.ToString().Substring(0, 2) == yearinpstr.Substring(yearinpstr.Length - 2)).ToList();
            }

            Projects = new ObservableCollection<ProjectModel>(pmnew);
            SortProjects(SortFilter);
        }

        private async void ExecuteOpenSubDialog(object o)
        {
            ProjectModel pm = (ProjectModel)o;

            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new ProjectSummaryView();
            ProjectSummaryVM vm = new ProjectSummaryVM(pm, CurrentEmployee);
            view.DataContext = vm;
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

        }


        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddProjectView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            AddProjectVM vm = view.DataContext as AddProjectVM;
            bool resultvm = vm.result;

            if (resultvm)
            {
                LoadProjects();
            }
        }

        private async void ExecuteRunAddClientDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddClientView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
            //if (result != null)
            //{
                LoadClients();
            //}
        }

        private async void ExecuteRunAddMarketDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddMarketView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
            //if (result != null)
            //{
                LoadMarkets();
            //}
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM();
            
            bool donothing = false;
            switch (o)
            {
                case ProjectModel pm:
                    List<SubProjectDbModel> subs =  SQLAccess.LoadSubProjectsByProject(pm.Id);
                    aysvm = new AreYouSureVM(pm);
                    foreach (SubProjectDbModel spm in subs)
                    {

                        List<TimesheetRowDbModel> val = SQLAccess.LoadTimeSheetDatabySubId(spm.Id);

                        if (val.Count > 0)
                        {
                            aysvm.TopLine = "Cannot delete project,";
                            aysvm.BottomLine = "Hours already saved to project.";
                            donothing = true;
                        }
                    }
                    
                    break;
                case ClientModel cm:
                    aysvm = new AreYouSureVM(cm);
                    break;
                case MarketModel mm:
                    aysvm = new AreYouSureVM(mm);
                    break;
                default:
                    return;
                    // code block
            }

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");

            //show the dialog

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result && !donothing)
            {
                switch (o)
                {
                    case ProjectModel pm:
                        SQLAccess.ArchiveProject(pm.Id);
                        LoadProjects();
                        break;
                    case ClientModel cm:
                        SQLAccess.ArchiveClient(cm.Id);
                        LoadClients();
                        break;
                    case MarketModel mm:
                        SQLAccess.ArchiveMarket(mm.Id);
                        LoadMarkets();
                        break;
                    default:
                        // code block
                        break;
                }
            }
        }

        //private void ClosingEventHandlerProjects(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    //load list here
        //    LoadProjects();
        //}
        //private void ClosingEventHandlerClients(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    //load list here
        //    LoadClients();
        //}

        //private void ClosingEventHandlerMarkets(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    //load list here
        //    LoadMarkets();
        //}

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveProjects(ShowActiveProjects);

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            ProjectModel[] ProjectArray = new ProjectModel[dbprojects.Count];


            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];

            //if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
            //{
                ProjectModel pm = new ProjectModel(pdb, IsEditable);
                    EmployeeModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();
                    ClientModel cm = Clients.Where(x => x.Id == pdb.ClientId).FirstOrDefault();
                    MarketModel mm = Markets.Where(x => x.Id == pdb.MarketId).FirstOrDefault();

                    pm.ProjectManager = em;
                    pm.Client = cm;
                    pm.Market = mm;
                    //EmployeeDbModel emdbm = SQLAccess.LoadEmployeeById(pm.ManagerId);
                    //EmployeeModel em = new EmployeeModel(emdbm);
                    //ProjectManager = em;

                    //ClientDbModel cdbm = SQLAccess.LoadClientById(pm.ClientId);
                    //ClientModel cm = new ClientModel(cdbm);
                    //Client = cm;

                    //MarketDbModel mdbm = SQLAccess.LoadMarketeById(pm.MarketId);
                    //MarketModel mm = new MarketModel(mdbm);
                    //Market = mm;

                    ProjectArray[i] = pm;
                //}
            }
            );

            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            AllProjects = ProjectArray.ToList();
            Projects = new ObservableCollection<ProjectModel>(ProjectArray.ToList());
            
            //List<ProjectModel> activeprojects = Projects.Where(x => x.IsActive == true).ToList();
            NumProjects = Projects.Count;
        }

        private void LoadClients()
        {
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();

            ObservableCollection<ClientModel> members = new ObservableCollection<ClientModel>();

            foreach (ClientDbModel cdbm in dbclients)
            {
                members.Add(new ClientModel(cdbm));
            }

            Clients = members;
        }

        private void AddToYear()
        {
            YearInp++;
        }

        private void SubtractToYear()
        {
            YearInp--;
        }

        private void LoadMarkets()
        {
            List<MarketDbModel> dbmarkets = SQLAccess.LoadMarkets();

            ObservableCollection<MarketModel> members = new ObservableCollection<MarketModel>();

            foreach (MarketDbModel mdbm in dbmarkets)
            {
                members.Add(new MarketModel(mdbm));
            }

            Markets = members;
        }


        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeModel> members = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeModel(edbm));
            }

            ProjectManagers = members;
        }
    }
}
