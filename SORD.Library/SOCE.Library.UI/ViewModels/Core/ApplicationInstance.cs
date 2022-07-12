using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class ApplicationInstance : BaseVM, IApplicationInstance
    {
        public BaseVM CurrentPage { get; set; }

        public ICommand UpdateMWCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ApplicationInstance()
        {
            CurrentPage = new LoginVM();
            UpdateMWCommand = new RelayCommand<ApplicationPage>(GoToPage);
            CloseCommand = new RelayCommand(CloseWindow);
        }

        public void GoToPage(ApplicationPage page)
        {
            switch (page)
            {
                case ApplicationPage.Login:
                    CurrentPage = new LoginVM();
                    break;
                case ApplicationPage.ForgotPassword:
                    CurrentPage = new ForgotPasswordVM();
                    break;
                case ApplicationPage.Signup:
                    CurrentPage = new SignupVM();
                    break;
            }
        }

        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
