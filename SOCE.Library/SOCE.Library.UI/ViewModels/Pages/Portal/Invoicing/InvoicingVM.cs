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
    public class InvoicingVM : BaseVM
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

        public ICommand ClearComboBox { get; set; }
        public ICommand ClearSearchParamters { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand GoToOpenInvoicingSummary { get; set; }
        public ICommand AddYearCommand { get; set; }
        public ICommand SubtractYearCommand { get; set; }
        public ICommand GoToOpenProjectSummary { get; set; }

        private ObservableCollection<ProjectInvoicingModel> _projects = new ObservableCollection<ProjectInvoicingModel>();
        public ObservableCollection<ProjectInvoicingModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                RaisePropertyChanged(nameof(Projects));
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

        private List<ProjectInvoicingModel> AllProjects = new List<ProjectInvoicingModel>();

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

        private bool _showFeeNotMetOnly { get; set; } = false;
        public bool ShowFeeNotMetOnly
        {
            get { return _showFeeNotMetOnly; }
            set
            {
                _showFeeNotMetOnly = value;
                RunSearch();
                //Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects);
                RaisePropertyChanged(nameof(ShowFeeNotMetOnly));

            }
        }

        private bool _showUninvoicedHours { get; set; } = false;
        public bool ShowUninvoicedHours
        {
            get { return _showUninvoicedHours; }
            set
            {
                _showUninvoicedHours = value;
                RunSearch();
                //Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects);
                RaisePropertyChanged(nameof(ShowUninvoicedHours));

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
                Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects);
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

        public InvoicingVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.AddYearCommand = new RelayCommand(this.AddToYear);
            this.SubtractYearCommand = new RelayCommand(this.SubtractToYear);
            this.GoToOpenProjectSummary = new RelayCommand<object>(this.ExecuteOpenSubDialog);
            LoadClients();
            LoadProjectManagers();
            LoadProjects();

            ShowActiveProjects = true;
        }

        private async void ExecuteOpenSubDialog(object o)
        {
            ProjectInvoicingModel pm = (ProjectInvoicingModel)o;
            ProjectDbModel pdb = SQLAccess.LoadProjectsById(pm.Id);
            ProjectViewResModel pvrm = new ProjectViewResModel(pdb);
            pvrm.ProjectManager = pm.ProjectManager;
            pvrm.Client = pm.Client;
            var view = new BaseProjectSummaryView();
            BaseProjectSummaryVM vm = new BaseProjectSummaryVM(CurrentEmployee, pvrm, ViewEnum.Invoicing, this);
            view.DataContext = vm;
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
        }

        private async void ClearInputsandReload()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchableText = "";
                SelectedClient = null;
                SelectedPM = null;

                if (!ShowActiveProjects)
                {
                    if (YearInp != null)
                    {
                        string yearinpstr = YearInp.ToString();
                        Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects.Where(x => x.ProjectNumber.ToString().Substring(0, 2) == yearinpstr.Substring(yearinpstr.Length - 2)).ToList());
                    }
                    else
                    {
                        Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects);
                    }
                }
                else
                {
                    Projects = new ObservableCollection<ProjectInvoicingModel>(AllProjects.Where(x => x.IsActive == true).ToList());
                }
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private async void RunSearch()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                List<ProjectInvoicingModel> pmnew = AllProjects;

                if (SelectedPM != null)
                {
                    pmnew = pmnew.Where(x => x.ProjectManager?.Id == SelectedPM.Id).ToList();
                }

                if (SelectedClient != null)
                {
                    pmnew = pmnew.Where(x => x.Client.Id == SelectedClient.Id).ToList();
                }

                if (!String.IsNullOrEmpty(SearchableText))
                {
                    pmnew = pmnew.Where(x => x.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || x.ProjectNumber.ToString().Contains(_searchableText)).ToList();
                }

                if (!ShowActiveProjects && YearInp != null)
                {
                    string yearinpstr = YearInp.ToString();
                    pmnew = pmnew.Where(x => x.ProjectNumber.ToString().Substring(0, 2) == yearinpstr.Substring(yearinpstr.Length - 2)).ToList();
                }

                if (ShowFeeNotMetOnly)
                {
                    pmnew = pmnew.Where(x => x.PercentOfTotalFeeInvoiced <= 100).ToList();
                }

                if (ShowUninvoicedHours)
                {
                    pmnew = pmnew.Where(x => x.HoursSpentSinceLastInvoice > 0).ToList();
                }

                Projects = new ObservableCollection<ProjectInvoicingModel>(pmnew);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        public void LoadProjects()
        {
            SQLAccess sqldb = new SQLAccess();
            sqldb.Open();

            List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveProjects(ShowActiveProjects);

            ObservableCollection<ProjectInvoicingModel> members = new ObservableCollection<ProjectInvoicingModel>();

            ProjectInvoicingModel[] ProjectArray = new ProjectInvoicingModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];
                ProjectInvoicingModel pm = new ProjectInvoicingModel(pdb);

                EmployeeLowResModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();
                ClientModel cm = Clients.Where(x => x.Id == pdb.ClientId).FirstOrDefault();

                pm.ProjectManager = em;
                pm.Client = cm;

                ProjectArray[i] = pm;
            }
            );
            sqldb.Close();
            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            AllProjects = ProjectArray.ToList();
            RunSearch();
        }


        public void LoadClients()
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

        private void AddToYear()
        {
            YearInp++;
        }

        private void SubtractToYear()
        {
            YearInp--;
        }

        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeLowResModel> members = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeLowResModel(edbm));
            }

            ProjectManagers = members;
        }
    }
}
