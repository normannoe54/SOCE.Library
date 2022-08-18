using System;
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

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand GoToLoginCommand { get; set; }

        public PortalAI()
        {
            CurrentPage = new HomeViewVM();
            GoToNewViewCommand = new RelayCommand<PortalPage>(GoToPage);
            GoToLoginCommand = new RelayCommand(GoToLogin);
        }

        public void GoToPage(PortalPage page)
        {
            switch (page)
            {
                case PortalPage.Home:
                    CurrentPage = new HomeViewVM();
                    break;
                case PortalPage.Employee:
                    CurrentPage = new EmployeeVM();
                    break;
                case PortalPage.EmployeeData:
                    CurrentPage = new EmployeeDataVM();
                    break;
                case PortalPage.Timesheet:
                    CurrentPage = new TimesheetVM();
                    break;
                case PortalPage.Resources:
                    CurrentPage = new ResourcesVM();
                    break;
                case PortalPage.ProjectData:
                    CurrentPage = new ProjectDataVM();
                    break;
                case PortalPage.LicenseManager:
                    CurrentPage = new LicenseManagerVM();
                    break;
                case PortalPage.Projects:
                    CurrentPage = new ProjectVM();
                    break;
            }
        }

        public void GoToLogin()
        {
            IoCCore.Application.CurrentPage = IoCLogin.Application as BaseAI;          
        }
    }
}
