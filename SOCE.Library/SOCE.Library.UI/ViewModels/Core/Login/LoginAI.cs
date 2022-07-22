using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class LoginAI : BaseAI, ILoginAI
    {
        //private BaseVM _currentPage { get; set; }
        //public BaseVM CurrentPage
        //{
        //    get
        //    {
        //        return _currentPage;
        //    }
        //    set
        //    {
        //        _currentPage = value;
        //        RaisePropertyChanged(nameof(CurrentPage));
        //    }
        //}

        public ICommand UpdateMWCommand { get; set; }

        public LoginAI()
        {
            CurrentPage = new LoginVM();
            UpdateMWCommand = new RelayCommand<LoginPage>(GoToPage);
        }

        public void GoToPage(LoginPage page)
        {
            switch (page)
            {
                case LoginPage.Login:
                    CurrentPage = new LoginVM();
                    break;
                case LoginPage.ForgotPassword:
                    CurrentPage = new ForgotPasswordVM();
                    break;
                case LoginPage.Signup:
                    CurrentPage = new SignupVM();
                    break;
            }
        }
    }
}
