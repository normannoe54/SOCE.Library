using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

                if (_currentEmployee.Status == AuthEnum.Admin || _currentEmployee.Status == AuthEnum.Principal)
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


        public ICommand DeleteProject { get; set; }
        public ICommand DeleteMarket { get; set; }
        public ICommand DeleteClient { get; set; }

        public ICommand DeleteSubProject { get; set; }

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


        public ProjectVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            this.GoToAddProject = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.GoToAddClient = new RelayCommand<object>(this.ExecuteRunAddClientDialog);
            this.GoToAddMarket = new RelayCommand<object>(this.ExecuteRunAddMarketDialog);
            this.GoToAddSubProject = new RelayCommand<object>(this.ExecuteRunAddSubProjectDialog);

            this.DeleteProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.DeleteClient = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.DeleteMarket = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.DeleteSubProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);

            LoadProjects();
            LoadClients();
            LoadMarkets();
            LoadProjectManagers();
        }

        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddProjectView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandlerProjects);
        }

        private async void ExecuteRunAddSubProjectDialog(object o)
        {
            ProjectModel pm = (ProjectModel)o;

            if (pm!=null)
            {
                AddSubProjectVM aspvm = new AddSubProjectVM(pm);
                var view = new AddSubProjectView();
                view.DataContext = aspvm;

                //show the dialog
                var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandlerProjects);
            }
 
        }

        private async void ExecuteRunAddClientDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddClientView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandlerClients);
        }

        private async void ExecuteRunAddMarketDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddMarketView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandlerMarkets);
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM() ;
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
                    break;
            }

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");

            ////show the dialog


            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
            {
                switch (o)
                {
                    case ProjectModel pm:
                        SQLAccess.DeleteProject(pm.Id);
                        LoadProjects();
                        break;
                    case ClientModel cm:
                        SQLAccess.DeleteClient(cm.Id);
                        LoadClients();
                        break;
                    case MarketModel mm:
                        SQLAccess.DeleteMarket(mm.Id);
                        LoadMarkets();
                        break;
                    case SubProjectModel spm:
                        SQLAccess.DeleteSubProject(spm.Id);
                        LoadProjects();
                        break;
                    default:
                        // code block
                        break;
                }
            }
        }

        private void ClosingEventHandlerProjects(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadProjects();
        }
        private void ClosingEventHandlerClients(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadClients();
        }

        private void ClosingEventHandlerMarkets(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadMarkets();
        }

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            foreach (ProjectDbModel pdb in dbprojects)
            {
                List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(pdb.Id);

                ObservableCollection<SubProjectModel> submembers = new ObservableCollection<SubProjectModel>();

                foreach (SubProjectDbModel sdb in subdbprojects)
                {
                    submembers.Add(new SubProjectModel(sdb));
                //submembers.Add(new SubProjectModel() { Id = sdb.Id, ProjectNumber = pdb.Id, PointNumber = sdb.PointNumber, Description = sdb.Description, Fee = sdb.Fee });
                }

                ProjectModel pm = new ProjectModel(pdb);
                pm.SubProjects = submembers;
                members.Add(pm);
            }

            Projects = members;

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
