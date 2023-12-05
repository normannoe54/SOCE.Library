using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SOCE.Library.UI.ViewModels
{
    public class SelectionBoxVM : BaseVM
    {
        public bool Result { get; set; }
        public string Message { get; set; } = "";

        private bool _isSubVisible = false;
        public bool IsSubVisible
        {
            get
            {
                return _isSubVisible;
            }
            set
            {
                _isSubVisible = value;
                RaisePropertyChanged(nameof(IsSubVisible));
            }
        }

        private string _subMessage = "";
        public string SubMessage
        {
            get
            {
                return _subMessage;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsSubVisible = true;
                }
                _subMessage = value;
                RaisePropertyChanged(nameof(SubMessage));
            }
        }


        private ObservableCollection<SubProjectLowResModel> _subprojects = new ObservableCollection<SubProjectLowResModel>();
        public ObservableCollection<SubProjectLowResModel> SubProjects
        {
            get { return _subprojects; }
            set
            {
                _subprojects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private SubProjectLowResModel _selectedSubproject;
        public SubProjectLowResModel SelectedSubproject
        {
            get
            {
                return _selectedSubproject;
            }
            set
            {
                _selectedSubproject = value;
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public SelectionBoxVM()
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public void YesDoTheAction()
        {
            Result = true;
            //do stuff

            CloseWindow();
        }

        private void CancelCommand()
        {
            Result = false;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            bool val = DialogHost.IsDialogOpen("RootDialog");
            DialogHost.Close("RootDialog");
        }
    }
}
