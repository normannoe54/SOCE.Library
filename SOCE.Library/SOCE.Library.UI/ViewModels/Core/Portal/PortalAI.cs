using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace SOCE.Library.UI.ViewModels
{
    public class PortalAI : BaseAI, IPortalAI
    {
        private bool _pMVisible { get; set; }
        public bool PMVisible
        {
            get
            {
                return _pMVisible;
            }
            set
            {
                _pMVisible = value;
                RaisePropertyChanged(nameof(PMVisible));
            }
        }

        private bool _adminvisible { get; set; }
        public bool AdminVisible
        {
            get
            {
                return _adminvisible;
            }
            set
            {
                _adminvisible = value;
                RaisePropertyChanged(nameof(AdminVisible));
            }
        }

        private EmployeeModel _loggedInEmployee { get; set; }
        public EmployeeModel LoggedInEmployee
        {
            get
            {
                return _loggedInEmployee;
            }
            set
            {
                _loggedInEmployee = value;
                RaisePropertyChanged(nameof(LoggedInEmployee));
                WelcomeMessage = "Welcome " + _loggedInEmployee.FullName;
                PMVisible = LoggedInEmployee.Status != AuthEnum.Standard;
                AdminVisible = LoggedInEmployee.Status == AuthEnum.Admin || LoggedInEmployee.Status == AuthEnum.Principal;

            }
        }

        private string _welcomeMessage { get; set; }
        public string WelcomeMessage
        {
            get
            {
                return _welcomeMessage;
            }
            set
            {
                _welcomeMessage = value;
                RaisePropertyChanged(nameof(WelcomeMessage));
            }
        }

        private bool _rightDrawerOpen = false;
        public bool RightDrawerOpen
        {
            get
            {
                return _rightDrawerOpen;
            }
            set
            {
                _rightDrawerOpen = value;
                RaisePropertyChanged(nameof(RightDrawerOpen));
            }
        }

        private UserControl _rightViewToShow = new UserControl();
        public UserControl RightViewToShow
        {
            get { return _rightViewToShow; }
            set
            {
                _rightViewToShow = value;
                RaisePropertyChanged(nameof(RightViewToShow));
            }
        }


        private EmployeeVM employeeVM;
        private TimesheetVM timesheetVM;
        private TimesheetReviewVM2 timesheetReviewVM;
        //private ProjectDataVM projectDataVM;
        private ProjectVM projectVM;
        private PortalPage currentPage;

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand GoToLoginCommand { get; set; }

        public PortalAI()
        {
            //LoggedInEmployee = employee;
            //CurrentPage = new TimesheetVM(employee);
            GoToNewViewCommand = new RelayCommand<PortalPage>(GoToPage);
            GoToLoginCommand = new RelayCommand(GoToLogin);
            
        }

        public void Initiate(EmployeeModel employee)
        {
            LoggedInEmployee = employee;
            employeeVM = new EmployeeVM(employee);
            timesheetVM = new TimesheetVM(employee);
            timesheetReviewVM = new TimesheetReviewVM2(employee);
            //projectDataVM = new ProjectDataVM(employee);
            projectVM = new ProjectVM(employee);
        }

        public void RefreshViews()
        {
            Initiate(LoggedInEmployee);
        }

        public void GoToPage(PortalPage page)
        {   if (currentPage == page )
            {
                return;
            }
            switch (page)
            {
                //case PortalPage.Home:
                //    CurrentPage = new EmployeeVM(LoggedInEmployee);
                //    break;
                case PortalPage.Employee:
                    CurrentPage = employeeVM;
                    break;
                case PortalPage.Timesheet:
                    timesheetVM.LoadCurrentTimesheet(DateTime.Now);
                    timesheetVM.SearchFilter = false;
                    CurrentPage = timesheetVM;
                    break;
                case PortalPage.TimesheetReview:
                    CurrentPage = timesheetReviewVM;
                    break;
                case PortalPage.ProjectData:
                    //CurrentPage = projectDataVM;
                    break;
                //case PortalPage.LicenseManager:
                //    CurrentPage = new LicenseManagerVM(LoggedInEmployee);
                //    break;
                case PortalPage.Projects:
                    CurrentPage = projectVM;
                    break;
            }

            currentPage = page;
        }

        public void GoToTimesheetByDate(DateTime date)
        {
            currentPage = PortalPage.Timesheet;
            CurrentPage = new TimesheetVM(LoggedInEmployee,date);
        }

        public void GoToLogin()
        {
            currentPage = PortalPage.Default;
            CoreAI globalwindow = (CoreAI)IoCCore.Application;
            globalwindow.GoToLogin();

            //IoCCore.Application.CurrentPage = IoCLogin.Application as BaseAI;          
        }
    }
}
