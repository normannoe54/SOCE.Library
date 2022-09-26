using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                if (_currentEmployee.Status == AuthEnum.Admin)
                {
                    CanAddProject = true;
                }

                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }
        public ICommand GoToAddProject { get; set; }
        public ICommand GoToAddClient { get; set; }
        public ICommand GoToAddMarket { get; set; }
        public ICommand DeleteProject { get; set; }

        private bool _canAddProject = true;
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

        private bool _isEditable = true;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }



        public ProjectVM()
        {
            this.GoToAddProject = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.GoToAddClient = new RelayCommand<object>(this.ExecuteRunAddClientDialog);
            this.GoToAddMarket = new RelayCommand<object>(this.ExecuteRunAddMarketDialog);
            this.DeleteProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
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
            EmployeeModel em = o as EmployeeModel;
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM(em);

            view.DataContext = aysvm;

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandlerProjects);

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
            {
                SQLAccess.DeleteEmployee(em.Id);
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
                    submembers.Add(new SubProjectModel() { Id = sdb.Id, ProjectNumber = pdb.ProjectNumber, PointNumber = sdb.PointNumber, Description = sdb.Description, Fee = sdb.Fee });
                }

                ProjectModel pm = new ProjectModel(pdb);
                pm.SubProjects = submembers;
                members.Add(new ProjectModel(pdb));
            }

            Projects = members;
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
