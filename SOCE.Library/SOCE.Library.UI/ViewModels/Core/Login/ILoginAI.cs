using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SOCE.Library.UI.ViewModels
{
    public interface ILoginAI : IBaseAI
    {
        void GoToLogin();

        //void GoToPage(LoginPage loginPage);


        void ForgotPassword();

        void InsertCode(string code, EmployeeDbModel employee);

        void ResetPassword(EmployeeDbModel employee);

        //ICommand GoToLoginCommand { get; set; }
        //ICommand GoToForgotPassword { get; set; }
        //ICommand GoToInsertCodeCommand { get; set; }

        //ICommand GoToResetPasswordCommand { get; set; }
    }
}
