﻿using System;
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


            LoadProjects();
        }

        private async void ExecuteRunDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddProjectView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

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
                members.Add(new ProjectModel(pdb));
            }

            Projects = members;
        }
    }
}