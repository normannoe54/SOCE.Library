using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace SOCE.Library.UI.ViewModels
{
    public class CoreAI : BaseVM, ICoreAI
    {
        private WindowState _windowType { get; set; } = WindowState.Normal;
        public WindowState WindowType
        {
            get
            {
                return _windowType;
            }
            set
            {
                _windowType = value;
                RaisePropertyChanged(nameof(WindowType));
            }
        }

        private BaseAI _currentPage { get; set; }
        public BaseAI CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        private PackIconKind _windowButton;
        public PackIconKind WindowButton
        {
            get
            {
                return _windowButton;
            }
            set
            {
                _windowButton = value;
                RaisePropertyChanged("WindowButton");
            }
        }

        public ICommand CloseCommand { get; set; }
        public ICommand MinusCommand { get; set; }

        public CoreAI()
        {
            GoToLogin();
            CurrentPage = IoCLogin.Application as BaseAI;
            LoginAI login = (LoginAI)CurrentPage;
            login.GoToLogin();

            CloseCommand = new RelayCommand(CloseWindow);
            MinusCommand = new RelayCommand(MinusWindow);
        }


        public void GoToLogin()
        {
            CurrentPage = IoCLogin.Application as BaseAI;

            WindowType = WindowState.Normal;
        }

        public void GoToPortal(EmployeeModel employee)
        {
            CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;
            portAI.LoggedInEmployee = employee;
            portAI.CurrentPage = new TimesheetVM(employee);

            WindowType = WindowState.Maximized;
        }

        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }

        public void MinusWindow()
        {
            WindowType = WindowState.Minimized;
        }
    }
}
