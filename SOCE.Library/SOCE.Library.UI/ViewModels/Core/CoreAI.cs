using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Reflection;
using System.IO;

namespace SOCE.Library.UI.ViewModels
{
    public class CoreAI : BaseVM, ICoreAI
    {
        private int _widthRef;
        public int WidthRef
        {
            get
            {
                return _widthRef;
            }
            set
            {
                _widthRef = value;
                RaisePropertyChanged(nameof(WidthRef));
            }
        }

        private int _heightRef;
        public int HeightRef
        {
            get
            {
                return _heightRef;
            }
            set
            {
                _heightRef = value;
                RaisePropertyChanged(nameof(HeightRef));
            }
        }


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
                DetermineIcon();
                RaisePropertyChanged(nameof(WindowType));
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

        public ICommand MaximizeWindowCommand { get; private set; }
        public ICommand MinusCommand { get; set; }

        private string _versionNumber = "0";
        public string VersionNumber
        {
            get
            {
                return _versionNumber;
            }
            set
            {
                _versionNumber = value;
                RaisePropertyChanged("VersionNumber");
            }
        }

        private bool _isConfusingStuffVisible = true;
        public bool IsConfusingStuffVisible
        {
            get
            {
                return _isConfusingStuffVisible;
            }
            set
            {
                _isConfusingStuffVisible = value;
                RaisePropertyChanged("IsConfusingStuffVisible");
            }
        }

        private bool _isBlurProgressVisible = false;
        public bool IsBlurProgressVisible
        {
            get
            {
                return _isBlurProgressVisible;
            }
            set
            {
                _isBlurProgressVisible = value;
                RaisePropertyChanged("IsBlurProgressVisible");
            }
        }

        private bool _isWindowEnabled = true;
        public bool IsWindowEnabled
        {
            get
            {
                return _isWindowEnabled;
            }
            set
            {
                _isWindowEnabled = value;
                RaisePropertyChanged("IsWindowEnabled");
            }
        }


        private double _blurRadius = 0;
        public double BlurRadius
        {
            get
            {
                return _blurRadius;
            }
            set
            {
                _blurRadius = value;
                RaisePropertyChanged("BlurRadius");
            }
        }

        public CoreAI()
        {
            //version number
            //get version number
            CollectVersionNumber();

            GoToLogin();
            CurrentPage = IoCLogin.Application as BaseAI;
            LoginAI login = (LoginAI)CurrentPage;
            login.GoToLogin();

            CloseCommand = new RelayCommand(CloseWindow);
            MaximizeWindowCommand = new RelayCommand(MaximizeWindowCom);
            MinusCommand = new RelayCommand(MinusWindow);
            DetermineIcon();
        }

        public void MakeBlurry()
        {
            BlurRadius = 20;
            IsWindowEnabled = false;
            IsBlurProgressVisible = true;
        }

        public void MakeClear()
        {
            
            BlurRadius = 0;
            IsWindowEnabled = true;
            IsBlurProgressVisible = false;
        }


        public void CollectVersionNumber()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                string newPath = Path.GetFullPath(Path.Combine(directory, @"..\"));

                string[] files = Directory.GetFiles(newPath);
                foreach (string file in files)
                {
                    if (Path.GetFileName(file) == "VersionChecker.xml")
                    {
                        string text = File.ReadAllText(file);

                        int pFrom = text.IndexOf("<version>") + "<version>".Length;
                        int pTo = text.LastIndexOf("</version>");

                        string result = text.Substring(pFrom, pTo - pFrom);

                        double val=0;

                        bool succ = Double.TryParse(result, out val);

                        if (succ)
                        {
                            VersionNumber = val.ToString();
                        }

                        break;
                    }
                }
            }
            
        }

        public void GoToLogin()
        {
            CurrentPage = IoCLogin.Application as BaseAI;
            WindowType = WindowState.Normal;
            DetermineIcon();
            WidthRef = 900;
            HeightRef = 700;
        }

        public void GoToPortal(EmployeeModel employee)
        {
            CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;
            portAI.Initiate(employee);
            //portAI.CurrentPage = new TimesheetVM(employee);
            portAI.GoToPage(PortalPage.Timesheet);
            WindowType = WindowState.Maximized;
            DetermineIcon();
            WidthRef = 1900;
            HeightRef = 1000;
        }

        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }

        public void MinusWindow()
        {
            WindowType = WindowState.Minimized;
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
