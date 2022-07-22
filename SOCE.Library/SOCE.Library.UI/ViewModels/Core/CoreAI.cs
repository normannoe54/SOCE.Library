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

        public CoreAI()
        {
            CurrentPage = IoCLogin.Application as BaseAI;
            CloseCommand = new RelayCommand(CloseWindow);
        }

        public void GoToPage(CorePage page)
        {
            switch (page)
            {
                case CorePage.Login:
                    CurrentPage = IoCLogin.Application as BaseAI;
                    break;
                case CorePage.Portal:
                    CurrentPage = IoCPortal.Application as BaseAI;
                    break;
            }
        }

        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
