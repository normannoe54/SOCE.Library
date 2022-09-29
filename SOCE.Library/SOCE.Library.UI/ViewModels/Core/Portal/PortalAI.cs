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
        public EmployeeModel LoggedInEmployee { get; set; }

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
                //case PortalPage.Home:
                //    CurrentPage = new HomeViewVM(LoggedInEmployee);
                //    break;
                case PortalPage.Employee:
                    CurrentPage = new EmployeeVM(LoggedInEmployee);
                    break;
                case PortalPage.Timesheet:
                    CurrentPage = new TimesheetVM(LoggedInEmployee);
                    break;
                //case PortalPage.Resources:
                //    CurrentPage = new ResourcesVM(LoggedInEmployee);
                //    break;
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
            IoCCore.Application.CurrentPage = IoCLogin.Application as BaseAI;          
        }
    }
}
