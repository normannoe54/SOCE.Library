﻿using System;
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
        public ICommand GoToAddSubProject { get; set; }


        public ICommand ArchiveProject { get; set; }
        public ICommand ArchiveMarket { get; set; }
        public ICommand ArchiveClient { get; set; }

        public ICommand DeleteSubProject { get; set; }

        public ICommand ClearComboBox { get; set; }

        public ICommand ClearSearchParamters { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand ExportDataCommand { get; set; }

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

        private ObservableCollection<object> _objectsToSelect = new ObservableCollection<object>();
        public ObservableCollection<object> ObjectsToSelect
        {
            get { return _objectsToSelect; }
            set
            {
                _objectsToSelect = value;
                RaisePropertyChanged(nameof(ObjectsToSelect));
            }
        }

        private object _objectofInterest { get; set; }
        public object ObjectofInterest
        {
            get { return _objectofInterest; }
            set
            {
                _objectofInterest = value;
                RaisePropertyChanged(nameof(ObjectofInterest));

                if (_objectofInterest != null)
                {
                    CloseButtonVisible = true;

                }
                else
                {
                    CloseButtonVisible = false;
                    SearchableText = "";
                }
            }
        }

        private int _selectedIndexofSearch { get; set; }
        public int SelectedIndexofSearch
        {
            get { return _selectedIndexofSearch; }
            set
            {
                ObjectofInterest = null;
                _selectedIndexofSearch = value;

                switch (_selectedIndexofSearch)
                {
                    case (0):
                        {
                            ObjectsToSelect = new ObservableCollection<object>(ProjectManagers);
                            TextSearchPath = "FullName";
                            break;
                        }
                    case (1):
                        {
                            ObjectsToSelect = new ObservableCollection<object>(Clients);
                            TextSearchPath = "ClientName";
                            break;
                        }
                    case (2):
                        {
                            ObjectsToSelect = new ObservableCollection<object>(Markets);
                            TextSearchPath = "MarketName";
                            break;
                        }
                    default:
                        break;
                }

                RaisePropertyChanged(nameof(SelectedIndexofSearch));
            }
        }

        private string _textSearchPath { get; set; }
        public string TextSearchPath
        {
            get { return _textSearchPath; }
            set
            {
                _textSearchPath = value;
                RaisePropertyChanged(nameof(TextSearchPath));
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

        private bool _showActiveProjects { get; set; } = true;
        public bool ShowActiveProjects
        {
            get { return _showActiveProjects; }
            set
            {
                _showActiveProjects = value;

                if (_showActiveProjects)
                {
                    Projects = new ObservableCollection<ProjectModel>(AllProjects.Where(x => x.IsActive = _showActiveProjects).ToList());
                }
                else
                {
                    Projects = new ObservableCollection<ProjectModel>(AllProjects);
                }

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
            this.GoToAddSubProject = new RelayCommand<object>(this.ExecuteRunAddSubProjectDialog);

            this.ArchiveProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.ArchiveClient = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.ArchiveMarket = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.DeleteSubProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);

            this.ClearComboBox = new RelayCommand(this.ClearSearchableComboBox);
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.ExportDataCommand = new RelayCommand(this.RunExport);

            LoadProjects();
            LoadClients();
            LoadMarkets();
            LoadProjectManagers();
            SelectedIndexofSearch = 0;
        }

        private void ClearInputsandReload()
        {
            ObjectofInterest = null;
            SearchableText = "";

            Projects = new ObservableCollection<ProjectModel>(AllProjects);
            
        }

        private void RunExport()
        {
            StringBuilder csv = new StringBuilder();

            string CSVHeader = $"Project Data";
            csv.AppendLine(CSVHeader);

            string DateHeader = $"{DateTime.Now.ToString("MM/dd/yyyy")}";
            csv.AppendLine(DateHeader);

            string header = $"Number,Name,PM,Client,Percent Complete,Total Budget,Budget Spent,Budget Left,Hours Spent,Hours Left";
            csv.AppendLine(header);

            foreach (ProjectModel pm in Projects)
            {
                if (!pm.Formatted)
                {
                    pm.FormatData(false);
                }
                string appendeddata = $"{pm.ProjectNumber},{pm.ProjectName.Replace(",","")},{pm.ProjectManager.FullName},{pm.Client.ClientName.Replace(",", "")}[{pm.Client.ClientNumber}],%{pm.PercentComplete},{pm.TotalBudget},{pm.BudgetSpent}[%{pm.PercentBudgetSpent}],{pm.BudgetLeft},{pm.HoursSpent},{pm.HoursLeft}";
                csv.AppendLine(appendeddata);

            }

            //after your loop
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads\\DataExport.csv");

            try
            {
                File.WriteAllText(pathDownload, csv.ToString());
                Process.Start(pathDownload);
            }
            catch
            {

            }
        }

        private void RunSearch()
        {
            List<ProjectModel> pmnew = new List<ProjectModel>();

            if (_objectofInterest == null)
            {
                if (String.IsNullOrEmpty(_searchableText))
                {
                    ClearInputsandReload();
                    return;
                }
                else
                {
                    foreach (ProjectModel pm in AllProjects)
                    {
                        if (pm.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || pm.ProjectNumber.ToString().Contains(_searchableText))
                        {
                            pmnew.Add(pm);
                        }
                    }
                }
            }
            else
            {
                foreach (ProjectModel pm in AllProjects)
                {
                    switch (_selectedIndexofSearch)
                    {
                        case (0):
                            {
                                EmployeeModel em = (EmployeeModel)_objectofInterest;

                                if (pm.ProjectManager.Id == em.Id)
                                {
                                    if (!String.IsNullOrEmpty(_searchableText))
                                    {
                                        if (pm.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || pm.ProjectNumber.ToString().Contains(_searchableText))
                                        {
                                            pmnew.Add(pm);
                                        }
                                    }
                                    else
                                    {
                                        pmnew.Add(pm);
                                    }
                                }
                                break;
                            }
                        case (1):
                            {
                                ClientModel cm = (ClientModel)_objectofInterest;

                                if (pm.Client.Id == cm.Id)
                                {
                                    if (!String.IsNullOrEmpty(_searchableText))
                                    {
                                        if (pm.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || pm.ProjectNumber.ToString().Contains(_searchableText))
                                        {
                                            pmnew.Add(pm);
                                        }
                                    }
                                    else
                                    {
                                        pmnew.Add(pm);
                                    }
                                }
                                break;
                            }
                        case (2):
                            {
                                MarketModel mm = (MarketModel)_objectofInterest;

                                if (pm.Market.Id == mm.Id)
                                {
                                    if (!String.IsNullOrEmpty(_searchableText))
                                    {
                                        if (pm.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || pm.ProjectNumber.ToString().Contains(_searchableText))
                                        {
                                            pmnew.Add(pm);
                                        }
                                    }
                                    else
                                    {
                                        pmnew.Add(pm);
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            
            Projects = new ObservableCollection<ProjectModel>(pmnew);
        }

        private void ClearSearchableComboBox()
        {
            ObjectofInterest = null;
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

        private async void ExecuteRunAddSubProjectDialog(object o)
        {
            ProjectModel pm = (ProjectModel)o;

            if (pm != null)
            {
                AddSubProjectVM aspvm = new AddSubProjectVM(pm);
                var view = new AddSubProjectView();
                view.DataContext = aspvm;

                //show the dialog
                var result = await DialogHost.Show(view, "RootDialog");
                AddSubProjectVM vm = view.DataContext as AddSubProjectVM;
                bool resultvm = vm.result;
                if (resultvm)
                {
                    pm.LoadSubProjects();
                    pm.UpdateSubProjects();
                }
                //LoadProjects();
            }
        }

        private async void ExecuteRunAddClientDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddClientView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
            if (result != null)
            {
                LoadClients();
            }
        }

        private async void ExecuteRunAddMarketDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddMarketView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
            if (result != null)
            {
                LoadMarkets();
            }
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM();
            switch (o)
            {
                case ProjectModel pm:
                    aysvm = new AreYouSureVM(pm);
                    break;
                case ClientModel cm:
                    aysvm = new AreYouSureVM(cm);
                    break;
                case MarketModel mm:
                    aysvm = new AreYouSureVM(mm);
                    break;
                case SubProjectModel spm:
                    aysvm = new AreYouSureVM(spm);
                    break;
                default:
                    return;
                    // code block
            }

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");

            //show the dialog

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
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
                    case SubProjectModel spm:
                        SQLAccess.ArchiveSubProject(spm.Id);
                        ProjectModel prj = Projects.Where(x => x.Id == spm.ProjectNumber).FirstOrDefault();
                        prj.LoadSubProjects();
                        prj.UpdateSubProjects();
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
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            ProjectModel[] ProjectArray = new ProjectModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];

                if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
                {
                    ProjectModel pm = new ProjectModel(pdb, IsEditable);
                    ProjectArray[i] = pm;
                }
            }
            );

            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            AllProjects = ProjectArray.ToList();
            Projects = new ObservableCollection<ProjectModel>(ProjectArray.ToList());

            List<ProjectModel> activeprojects = Projects.Where(x => x.IsActive == true).ToList();
            NumProjects = activeprojects.Count;
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
