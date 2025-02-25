using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

                if (_rightDrawerOpen)
                {
                    CoreAI CurrentPage = IoCCore.Application as CoreAI;
                    CurrentPage.IsConfusingStuffVisible = false;
                }
                else
                {
                    CoreAI CurrentPage = IoCCore.Application as CoreAI;
                    CurrentPage.IsConfusingStuffVisible = true;
                }

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
        private TimesheetReviewVM timesheetReviewVM;
        private ProjectScheduleVM projectScheduleVM;
        private NetworkSearchVM networkSearchVM;
        private InvoicingVM invoicingVM;
        //private ProjectDataVM projectDataVM;
        private ProjectVM projectVM;
        private ProposalsVM proposalsVM;
        private PortalPage currentPage;

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand GoToLoginCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public PortalAI()
        {
            //LoggedInEmployee = employee;
            //CurrentPage = new TimesheetVM(employee);
            GoToNewViewCommand = new RelayCommand<PortalPage>(GoToPage);
            GoToLoginCommand = new RelayCommand(GoToLogin);
            ReloadCommand = new RelayCommand(Reload);

        }

        public async Task Initiate(EmployeeModel employee)
        {
            LoggedInEmployee = employee;
            timesheetVM = new TimesheetVM(employee);
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                employeeVM = new EmployeeVM(employee);

                if (employee.Status != AuthEnum.Standard)
                {
                    timesheetReviewVM = new TimesheetReviewVM(employee);
                }

                networkSearchVM = new NetworkSearchVM(employee);

                projectScheduleVM = new ProjectScheduleVM(employee);
                projectVM = new ProjectVM(employee);
                proposalsVM = new ProposalsVM(employee);
                if (employee.Status == AuthEnum.Admin || employee.Status == AuthEnum.Principal)
                {
                    invoicingVM = new InvoicingVM(employee);
                }
            }));
        }

        public async void Reload()
        {
            PortalPage curpage = currentPage;
            //Task.Run(async () => { await Initiate(LoggedInEmployee); }).Wait();
            await Initiate(LoggedInEmployee);
            //task.Wait();
            //var result = task.IsCompleted;
            GoToPage(curpage);
        }

        public void RefreshViews()
        {
            Initiate(LoggedInEmployee);
        }

        public async void GoToPage(PortalPage page)
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            if (currentPage == page)
            {
                ButtonInAction = true;
                return;
            }

            CoreAI CurrentPage2 = IoCCore.Application as CoreAI;
            CurrentPage2.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
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
                        timesheetVM.Message = "";
                        //timesheetVM.SearchFilter = false;
                        CurrentPage = timesheetVM;
                        break;
                    case PortalPage.TimesheetReview:
                        CurrentPage = timesheetReviewVM;
                        break;
                    case PortalPage.ProjectSchedule:
                        CurrentPage = projectScheduleVM;
                        break;
                    case PortalPage.NetworkSearch:
                        CurrentPage = networkSearchVM;
                        break;
                    case PortalPage.Proposals:
                        CurrentPage = proposalsVM;
                        break;
                    case PortalPage.Invoicing:
                        CurrentPage = invoicingVM;
                        break;
                    case PortalPage.Projects:
                        projectVM.Reload();
                        CurrentPage = projectVM;
                        break;
                }
                currentPage = page;
            }
            ));
            await Task.Run(() => Task.Delay(600));
            CurrentPage2.MakeClear();
            ButtonInAction = true;
        }

        public void GoToTimesheetByDate(DateTime date)
        {
            currentPage = PortalPage.Timesheet;
            CurrentPage = new TimesheetVM(LoggedInEmployee, date);

        }

        public void GoToLogin()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            currentPage = PortalPage.Default;
            CoreAI globalwindow = (CoreAI)IoCCore.Application;
            globalwindow.GoToLogin();

            ButtonInAction = true;
            //IoCCore.Application.CurrentPage = IoCLogin.Application as BaseAI;          
        }
    }
}
