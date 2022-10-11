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

        public LoginAI()
        {

        }

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
