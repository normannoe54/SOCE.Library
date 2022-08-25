using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AddSubProjectVM : BaseVM
    {
        private string _projectNameInp;
        public string ProjectNameInp
        {
            get { return _projectNameInp; }
            set
            {
                _projectNameInp = value;
                RaisePropertyChanged("ProjectNameInp");
            }
        }

        private double _projectNumberInp;
        public double ProjectNumberInp
        {
            get { return _projectNumberInp; }
            set
            {
                _projectNumberInp = value;
                RaisePropertyChanged("ProjectNumberInp");
            }
        }

        private int _projectId;
        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                _projectId = value;
                RaisePropertyChanged("ProjectId");
            }
        }


        private int _pointNumberInp;
        public int PointNumberInp
        {
            get { return _pointNumberInp; }
            set
            {
                _pointNumberInp = value;
                RaisePropertyChanged("PointNumberInp");
            }
        }

        private string _descriptionInp;
        public string DescriptionInp
        {
            get { return _descriptionInp; }
            set
            {
                _descriptionInp = value;
                RaisePropertyChanged("DescriptionInp");
            }
        }

        private int _feeInp;
        public int FeeInp
        {
            get { return _feeInp; }
            set
            {
                _feeInp = value;
                RaisePropertyChanged("FeeInp");
            }
        }

        public ICommand AddProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddSubProjectVM()
        {
            this.AddProjectCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            ProjectNameInp = NavigationStore.ProjectVM.SelectedProject.ProjectName;
            ProjectNumberInp = NavigationStore.ProjectVM.SelectedProject.ProjectNumber;
            ProjectId = NavigationStore.ProjectVM.SelectedProject.Id;
        }

        public void AddSubProject()
        {

            SubProjectDbModel subproject = new SubProjectDbModel()
            { ProjectId = ProjectId, PointNumber = PointNumberInp, Description = DescriptionInp, Fee = FeeInp };

            SQLAccess.AddSubProject(subproject);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
