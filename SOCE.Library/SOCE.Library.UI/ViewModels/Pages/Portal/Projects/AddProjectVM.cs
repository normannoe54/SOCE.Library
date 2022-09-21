using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AddProjectVM : BaseVM
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

        private int _projectNumberInp;
        public int ProjectNumberInp
        {
            get { return _projectNumberInp; }
            set
            {
                _projectNumberInp = value;
                RaisePropertyChanged("ProjectNumberInp");
            }
        }

        private string _clientInp;
        public string ClientInp
        {
            get { return _clientInp; }
            set
            {
                _clientInp = value;
                RaisePropertyChanged("ClientInp");
            }
        }

        private double _totalFeeInp;
        public double TotalFeeInp
        {
            get { return _totalFeeInp; }
            set
            {
                _totalFeeInp = value;
                RaisePropertyChanged("TotalFeeInp");
            }
        }

        public ICommand AddProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddProjectVM()
        {
            this.AddProjectCommand = new RelayCommand(this.AddProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddProject()
        {
            ProjectDbModel project = new ProjectDbModel();
            //{ ProjectName = ProjectNameInp, ProjectNumber = ProjectNumberInp, Client = ClientInp, Fee = TotalFeeInp };

            SQLAccess.AddProject(project);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
