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
    public class EmployeeInfoVM : BaseVM
    {
        public ICommand GoToTimesheetCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private EmployeeModel _selectedEmployee = new EmployeeModel();
        public EmployeeModel SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        private string _textEmployees;
        public string TextEmployees
        {
            get { return _textEmployees; }
            set
            {
                _textEmployees = value;
                RaisePropertyChanged(nameof(TextEmployees));
            }
        }

        private bool _rateTitleVisible;
        public bool RateTitleVisible
        {
            get { return _rateTitleVisible; }
            set
            {
                _rateTitleVisible = value;
                RaisePropertyChanged(nameof(RateTitleVisible));
            }
        }

        public EmployeeInfoVM(EmployeeModel employee)
        {
            SelectedEmployee = employee;

            this.GoToTimesheetCommand = new RelayCommand<object>(GoToTimesheet);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void GoToTimesheet(object o)
        {
            TimesheetSubmissionModel tsm = (TimesheetSubmissionModel)o;
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;
            portAI.GoToTimesheetByDate(tsm.Date);

            DialogHost.Close("RootDialog");
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
