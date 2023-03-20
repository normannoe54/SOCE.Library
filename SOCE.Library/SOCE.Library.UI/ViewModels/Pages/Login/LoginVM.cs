using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using SOCE.Library.Db;
using System.Windows.Controls;

namespace SOCE.Library.UI.ViewModels
{
    public class LoginVM : BaseVM
    {
        private string _email = "";
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        //private string _password = "";
        //public string Password
        //{
        //    get
        //    {
        //        return _password;
        //    }
        //    set
        //    {
        //        _password = value;
        //        RaisePropertyChanged(nameof(Password));
        //    }
        //}

        public ICommand GoToForgotPassword { get; set; }

        public ICommand LoginCommand { get; set; }

        private string _loginMessage { get; set; }
        public string LoginMessage
        {
            get
            {
                return _loginMessage;
            }
            set
            {
                _loginMessage = value;
                RaisePropertyChanged(nameof(LoginMessage));
            }
        }

        public LoginVM()
        {
            //LoginRequest.Password = "pass123";
            //LoginMessage = "";
            this.GoToForgotPassword = new RelayCommand(IoCLogin.Application.ForgotPassword);
            this.LoginCommand = new RelayCommand<object>(LoginCom);
        }

        /// <summary>
        /// Login command
        /// </summary>
        /// <param name="loginrequest"></param>
        public void LoginCom(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;

            //IoCCore.Application.CurrentPage = IoCPortal.Application as BaseAI;

            //check email
            //string emailcheck = Email.Substring(Email.LastIndexOf('@') + 1);

            //if (emailcheck != "shirkodonovan.com")
            //{
            //    LoginMessage = $"Shirk & O'Donovan email must be {Environment.NewLine}included to login to the application";
            //    return;
            //}

            string emailinput = Email + "@shirkodonovan.com";

            //tbd
            EmployeeDbModel em = SQLAccess.LoadEmployeeByUserandPassword(emailinput.ToLower(), password);

            if (em != null)
            {
                EmployeeModel employee = new EmployeeModel(em);
                CoreAI globalwindow = (CoreAI)IoCCore.Application;
                globalwindow.GoToPortal(employee);
            }
            else
            {
                LoginMessage = $"Could not find username or password,{Environment.NewLine}please try again";
                return;
            }

        }
    }
}