﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

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
                AdminVisible = LoggedInEmployee.Status == AuthEnum.Admin;

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

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand GoToLoginCommand { get; set; }

        public PortalAI()
        {
            //LoggedInEmployee = employee;
            //CurrentPage = new TimesheetVM(employee);
            GoToNewViewCommand = new RelayCommand<PortalPage>(GoToPage);
            GoToLoginCommand = new RelayCommand(GoToLogin);
        }

        public void GoToPage(PortalPage page)
        {
            switch (page)
            {
                //case PortalPage.Home:
                //    CurrentPage = new EmployeeVM(LoggedInEmployee);
                //    break;
                case PortalPage.Employee:
                    CurrentPage = new EmployeeVM(LoggedInEmployee);
                    break;
                case PortalPage.Timesheet:
                    CurrentPage = new TimesheetVM(LoggedInEmployee);
                    break;
                case PortalPage.TimesheetReview:
                    CurrentPage = new TimesheetReviewVM(LoggedInEmployee);
                    break;
                case PortalPage.ProjectData:
                    CurrentPage = new ProjectDataVM(LoggedInEmployee);
                    break;
                //case PortalPage.LicenseManager:
                //    CurrentPage = new LicenseManagerVM(LoggedInEmployee);
                //    break;
                case PortalPage.Projects:
                    CurrentPage = new ProjectVM(LoggedInEmployee);
                    break;
            }
        }

        public void GoToLogin()
        {
            CoreAI globalwindow = (CoreAI)IoCCore.Application;
            globalwindow.GoToLogin();

            //IoCCore.Application.CurrentPage = IoCLogin.Application as BaseAI;          
        }
    }
}
