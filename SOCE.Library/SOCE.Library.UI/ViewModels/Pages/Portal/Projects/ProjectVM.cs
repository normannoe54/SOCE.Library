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
        public ICommand GoToAddProject { get; set; }
        public ICommand GoToAddSubProject { get; set; }

        private ObservableCollection<ProjectModel> _projects;
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

        private ObservableCollection<SubProjectModel> _subprojects;
        public ObservableCollection<SubProjectModel> SubProjects
        {
            get { return _subprojects; }
            set
            {
                _subprojects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private ProjectModel _selectedProject;

        public ProjectModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;

                //collect subprojects
                NavigationStore.ProjectVM = this;
                CollectSubProjects();
                RaisePropertyChanged(nameof(SelectedProject));
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        private string _textProjects = "0 Total";
        public string TextProjects
        {
            get { return _textProjects; }
            set
            {
                _textProjects = value;
                RaisePropertyChanged(nameof(TextProjects));
            }
        }

        public ProjectVM()
        {
            this.GoToAddProject = new RelayCommand<object>(this.ExecuteRunDialog);
            this.GoToAddSubProject = new RelayCommand<object>(this.ExecuteRunSubDialog);

            LoadProjects();
        }

        private async void ExecuteRunDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            AddProjectView view = new AddProjectView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

        }

        private async void ExecuteRunSubDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            AddSubProjectView view = new AddSubProjectView();
            view.DataContext = new AddSubProjectVM();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingSubEventHandler);

        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadProjects();
        }

        private void ClosingSubEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            CollectSubProjects();
           
        }

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            foreach (ProjectDbModel pdb in dbprojects)
            {
                members.Add(new ProjectModel(pdb));
            }

            Projects = members;
        }

        private void CollectSubProjects()
        {
            if (SelectedProject == null)
            {
                return;
            }

            int id = SelectedProject.Id;

            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjects(id);

            ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

            foreach (SubProjectDbModel sdb in subdbprojects)
            {
                members.Add(new SubProjectModel() { Id = sdb.Id, ProjectNumber = SelectedProject.ProjectNumber, PointNumber = sdb.PointNumber, Description = sdb.Description, Fee = sdb.Fee });
            }

            SubProjects = members;
        }
    }
}
