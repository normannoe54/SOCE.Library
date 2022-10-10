using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class LoginAI : BaseAI, ILoginAI
    {
        //public ICommand GoToLoginCommand { get; set; }
        //public ICommand GoToForgotPassword { get; set; }
        //public ICommand GoToInsertCodeCommand { get; set; }

        //public ICommand GoToResetPasswordCommand { get; set; }

        public LoginAI()
        {
            //CurrentPage = new LoginVM();
            //GoToLoginCommand = new RelayCommand(GoToLogin);
            //GoToForgotPassword = new RelayCommand(ForgotPassword);
            //GoToInsertCodeCommand = new RelayCommand(InsertCode);
            //GoToResetPasswordCommand = new RelayCommand(ResetPassword);

        }

        //public void GoToPage(LoginPage page)
        //{
        //    switch (page)
        //    {
        //        case LoginPage.Login:
        //            CurrentPage = new LoginVM();
        //            break;
        //        case LoginPage.ForgotPassword:
        //            CurrentPage = new ForgotPasswordVM();
        //            break;
        //    }
        //}

        public void GoToLogin()
        {
            CurrentPage = new LoginVM();
        }

        public void ForgotPassword()
        {
            CurrentPage = new ForgotPasswordVM();
        }

        public void InsertCode(string code, EmployeeDbModel employee)
        {
            CurrentPage = new CodeInsertVM(code, employee);
        }

        public void ResetPassword(EmployeeDbModel employee)
        {
            CurrentPage = new ResetPasswordVM(employee);
        }
    }
}
