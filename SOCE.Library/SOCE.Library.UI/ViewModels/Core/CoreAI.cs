using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class CoreAI : BaseVM, ICoreAI
    {
        private WindowState _windowType { get; set; }
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

        public ICommand CloseCommand { get; set; }

        public ICommand MinusCommand { get; set; }

        public CoreAI()
        {
            CurrentPage = IoCLogin.Application as BaseAI;
            CloseCommand = new RelayCommand(CloseWindow);
            MinusCommand = new RelayCommand(MinusWindow);
        }

        public void GoToPage(CorePage page)
        {
            switch (page)
            {
                case CorePage.Login:
                    CurrentPage = IoCLogin.Application as BaseAI;
                    WindowType = WindowState.Normal;
                    break;
                case CorePage.Portal:
                    CurrentPage = IoCPortal.Application as BaseAI;
                    WindowType = WindowState.Maximized;
                    break;
            }
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
