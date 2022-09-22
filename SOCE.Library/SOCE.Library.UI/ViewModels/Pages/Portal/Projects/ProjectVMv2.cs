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
    public class ProjectVMv2 : BaseVM
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



        public ProjectVMv2()
        {
            this.GoToAddProject = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.GoToAddClient = new RelayCommand<object>(this.ExecuteRunAddClientDialog);
            this.GoToAddMarket = new RelayCommand<object>(this.ExecuteRunAddMarketDialog);
            this.DeleteProject = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            LoadProjects();
        }

        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddProjectView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        private async void ExecuteRunAddClientDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddClientView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        private async void ExecuteRunAddMarketDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddMarketView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            EmployeeModel em = o as EmployeeModel;
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM(em);

            view.DataContext = aysvm;

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
            {
                SQLAccess.DeleteEmployee(em.Id);
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadProjects();
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
    }
}
