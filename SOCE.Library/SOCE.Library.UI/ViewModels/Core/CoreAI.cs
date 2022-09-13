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

        private string _maxorRestoreTooltip;

        public string MaxorRestoreTooltip
        {
            get
            {
                return _maxorRestoreTooltip;
            }
            set
            {
                _maxorRestoreTooltip = value;
                RaisePropertyChanged(nameof(MaxorRestoreTooltip));
            }
        }

        public ICommand CloseCommand { get; set; }

        public ICommand MaximizeWindowCommand { get; set; }
        public ICommand MinusCommand { get; set; }

        public CoreAI()
        {
            CurrentPage = IoCLogin.Application as BaseAI;
            CloseCommand = new RelayCommand(CloseWindow);
            MaximizeWindowCommand = new RelayCommand(MaximizeWindowCom);
            MinusCommand = new RelayCommand(MinusWindow);
            DetermineIcon();
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

        public void MaximizeWindowCom()
        {
            if (WindowType == WindowState.Maximized)
            {
                WindowType = WindowState.Normal;
            }
            else
            {
                WindowType = WindowState.Maximized;
            }

            DetermineIcon();
        }

        public void MinusWindow()
        {
            WindowType = WindowState.Minimized;
        }

        private void DetermineIcon()
        {
            if (WindowType == WindowState.Maximized)
            {
                MaxorRestoreTooltip = "Restore";
                WindowButton = PackIconKind.WindowRestore;
            }
            else
            {
                MaxorRestoreTooltip = "Maximize";
                WindowButton = PackIconKind.WindowMaximize;
            }
        }
    }
}
